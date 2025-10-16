using CasherSystem.Data;
using CasherSystem.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CasherSystem.Views
{
    public partial class ProductsListPage : Page, INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext = App.GetService<AppDbContext>();
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<Product> _allProducts = new();
        private Product? _selectedProduct;
        private string _statusMessage = "جاهز";

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public ProductsListPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadProducts();
        }

        private void LoadProducts()
        {
            var items = dbContext.Products.ToList();
            _allProducts = new ObservableCollection<Product>(items);
            Products = new ObservableCollection<Product>(items);
            StatusMessage = "تم تحميل المنتجات";
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var term = searchBox.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(term))
            {
                Products = new ObservableCollection<Product>(_allProducts);
                return;
            }

            term = term.ToLowerInvariant();
            var filtered = _allProducts.Where(p =>
                (p.Name ?? string.Empty).ToLowerInvariant().Contains(term) ||
                (p.Color ?? string.Empty).ToLowerInvariant().Contains(term) ||
                (p.Size ?? string.Empty).ToLowerInvariant().Contains(term)
            ).ToList();
            Products = new ObservableCollection<Product>(filtered);
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Products_Page());
        }

        private void EditProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Product product)
            {
                NavigationService?.Navigate(new Products_Page(product));
            }
        }

        private void DeleteProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Product product)
            {
                if (MessageBox.Show($"حذف المنتج {product.Name}?", "تأكيد", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    dbContext.Products.Remove(product);
                    dbContext.SaveChanges();
                    _allProducts.Remove(product);
                    Products.Remove(product);
                    StatusMessage = "تم حذف المنتج";
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


