using CasherSystem.Data;
using CasherSystem.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CasherSystem.Views
{
    public partial class Products_Page : Page, INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext = App.GetService<AppDbContext>();
        private Product _currentProduct = new Product();
        private string _formTitle = "إضافة منتج";
        private string _formSubtitle = "أدخل بيانات المنتج";
        private string _statusMessage = "جاهز";
        private bool _isEditMode = false;

        public Product CurrentProduct
        {
            get => _currentProduct;
            set { _currentProduct = value; OnPropertyChanged(); }
        }

        public string FormTitle
        {
            get => _formTitle;
            set { _formTitle = value; OnPropertyChanged(); }
        }

        public string FormSubtitle
        {
            get => _formSubtitle;
            set { _formSubtitle = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public Products_Page()
        {
            InitializeComponent();
            DataContext = this;
            InitializeNewProduct();
        }

        public Products_Page(Product productToEdit)
        {
            InitializeComponent();
            DataContext = this;
            LoadProductForEdit(productToEdit);
        }

        private void InitializeNewProduct()
        {
            CurrentProduct = new Product
            {
                Name = string.Empty,
                Color = string.Empty,
                Size = string.Empty,
                Barcode = string.Empty,
                CostPrice = 0m,
                Quantity = 0,
                SellPrice = 0m
            };
            _isEditMode = false;
            FormTitle = "إضافة منتج";
            FormSubtitle = "أدخل بيانات المنتج";
            StatusMessage = "جاهز";
        }

        private void LoadProductForEdit(Product product)
        {
            CurrentProduct = product;
            _isEditMode = true;
            FormTitle = "تعديل منتج";
            FormSubtitle = "قم بتحديث بيانات المنتج";
            StatusMessage = "وضع التعديل";
        }

        private void SaveProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                MessageBox.Show("يرجى إدخال الحقول الإلزامية بشكل صحيح.", "تحقق", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (_isEditMode)
            {
                var existing = dbContext.syProducts.Find(CurrentProduct.Id);
                if (existing != null)
                {
                    existing.Name = CurrentProduct.Name;
                    existing.Color = CurrentProduct.Color;
                    existing.Size = CurrentProduct.Size;
                    existing.Quantity = CurrentProduct.Quantity;
                    existing.SellPrice = CurrentProduct.SellPrice;
                    existing.CostPrice = CurrentProduct.CostPrice;
                    existing.Barcode = CurrentProduct.Barcode;
                    dbContext.SaveChanges();
                    MessageBox.Show("تم تحديث المنتج بنجاح", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                dbContext.syProducts.Add(CurrentProduct);
                dbContext.SaveChanges();
                MessageBox.Show("تم حفظ المنتج", "نجاح", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            NavigationService?.GoBack();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(CurrentProduct.Name)) return false;
            if (CurrentProduct.Quantity < 0) return false;
            if (CurrentProduct.SellPrice < 0) return false;
            return true;
        }

        private void ClearFormButton_Click(object sender, RoutedEventArgs e)
        {
            InitializeNewProduct();
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


