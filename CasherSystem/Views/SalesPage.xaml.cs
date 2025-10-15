using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CasherSystem.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CasherSystem.Data;

namespace CasherSystem.Views
{



    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CartItem : INotifyPropertyChanged
    {
        private int _productId;
        private Product _product = null!;
        private int _quantity;
        private decimal _price;
        private decimal _total;

        public int ProductId
        {
            get => _productId;
            set { _productId = value; OnPropertyChanged(); }
        }

        public Product Product
        {
            get => _product;
            set { _product = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public decimal Price
        {
            get => _price;
            set { _price = value; OnPropertyChanged(); }
        }

        public decimal Total
        {
            get => _total;
            set { _total = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public partial class SalesPage : Page, INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext;
        private string _searchTerm = string.Empty;
        private Product? _selectedProduct;
        private int _quantity = 1;
        private decimal _discount = 0;
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<CartItem> _cartItems = new();
        private decimal _total;
        private decimal _netTotal;

        // New payment properties
        private List<PaymentMethod> _paymentMethods = new();
        private PaymentMethod? _selectedPaymentMethod;
        private string _customerName = string.Empty;
        private string _customerPhone = string.Empty;
        private int? _paidAmount = 0;
        private int? _remainingAmount = 0;

        public string SearchTerm
        {
            get => _searchTerm;
            set { _searchTerm = value; OnPropertyChanged(); }
        }

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public int Quantity
        {
            get => _quantity;
            set { _quantity = value; OnPropertyChanged(); }
        }

        public decimal Discount
        {
            get => _discount;
            set { _discount = value; OnPropertyChanged(); CalculateTotals(); }
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set { _products = value; OnPropertyChanged(); }
        }

        public ObservableCollection<CartItem> CartItems
        {
            get => _cartItems;
            set { _cartItems = value; OnPropertyChanged(); }
        }

        public decimal Total
        {
            get => _total;
            set { _total = value; OnPropertyChanged(); }
        }

        public decimal NetTotal
        {
            get => _netTotal;
            set { _netTotal = value; OnPropertyChanged(); CalculateRemainingAmount(); }
        }

        // New payment properties
        public List<PaymentMethod> PaymentMethods
        {
            get => _paymentMethods;
            set { _paymentMethods = value; OnPropertyChanged(); }
        }

        public PaymentMethod? SelectedPaymentMethod
        {
            get => _selectedPaymentMethod;
            set
            {
                _selectedPaymentMethod = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsDebtPayment)); // This is crucial!
            }
        }

        public bool IsDebtPayment => SelectedPaymentMethod?.Name == "Debt";

        public string CustomerName
        {
            get => _customerName;
            set { _customerName = value; OnPropertyChanged(); }
        }

        public string CustomerPhone
        {
            get => _customerPhone;
            set { _customerPhone = value; OnPropertyChanged(); }
        }

        public int? PaidAmount
        {
            get => _paidAmount;
            set { _paidAmount = value; OnPropertyChanged(); CalculateRemainingAmount(); }
        }

        public int? RemainingAmount
        {
            get => _remainingAmount;
            set { _remainingAmount = value; OnPropertyChanged(); }
        }

        public SalesPage()
        {
            InitializeComponent();
            DataContext = this;

            dbContext = App.GetService<AppDbContext>();

            // Initialize payment methods
            PaymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { Id = 1, Name = "Cash" },
                new PaymentMethod { Id = 2, Name = "Debt" }
            };
            SelectedPaymentMethod = PaymentMethods.First(); // Default to Cash

            // Initialize with sample data
            LoadData();
        }

        private string GenerateMixedCaseCode(int length = 8)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            string code = "";

            for (int i = 0; i < length; i++)
            {
                code += chars[random.Next(chars.Length)];
            }

            return code;
        }

        private void LoadData()
        {
            var allProducts = dbContext.Products.ToList();

            for (int i = 0; i < allProducts.Count; i++) {
                Products.Add(allProducts[i]);
            }
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null || Quantity <= 0) return;

            var existingItem = CartItems.FirstOrDefault(item => item.ProductId == SelectedProduct.Id);
            if (existingItem != null)
            {
                existingItem.Quantity = existingItem.Quantity + Quantity;
                existingItem.Total = existingItem.Quantity * existingItem.Price;
                OnPropertyChanged(nameof(CartItems));
            }
            else
            {
                var newItem = new CartItem
                {
                    ProductId = SelectedProduct.Id,
                    Product = SelectedProduct,
                    Quantity = Quantity,
                    Price = SelectedProduct.SellPrice,
                    Total = Quantity * SelectedProduct.SellPrice
                };
                CartItems.Add(newItem);
            }

            CalculateTotals();
            Quantity = 1;
        }

        private void RemoveFromCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CartItem item)
            {
                CartItems.Remove(item);
                CalculateTotals();
            }
        }

        private void CalculateTotals()
        {
            Total = CartItems.Sum(item => item.Total);
            NetTotal = Total - Discount;
            CalculateRemainingAmount();
        }

        private void CalculateRemainingAmount()
        {
            if (IsDebtPayment && PaidAmount.HasValue)
            {
                RemainingAmount = (int?)(NetTotal - PaidAmount);
            }
            else
            {
                RemainingAmount = 0;
            }
        }

        private void SaveSaleButton_Click(object sender, RoutedEventArgs e)
        {
            if (CartItems.Count > 0 && NetTotal > 0)
            {
                var newSale = new Sale
                {
                    Date = DateTime.Now,
                    Discount = Discount,
                    NetTotal = NetTotal,
                    paidAmount = PaidAmount.Value,
                    PaymentType = (SelectedPaymentMethod?.Name ?? "Cash").Length > 20 
                        ? (SelectedPaymentMethod?.Name ?? "Cash").Substring(0, 20) 
                        : (SelectedPaymentMethod?.Name ?? "Cash"),
                    remainingAmount = RemainingAmount.Value,
                    SecretCode = GenerateMixedCaseCode(),
                    Total = Total

                };
                // Ensure products list is initialized before adding items
                if (newSale.products == null)
                {
                    newSale.products = new List<Product>();
                }
                foreach (var item in CartItems)
                {
                    var product = dbContext.Products.FirstOrDefault(p => p.Id == item.Product.Id);
                    if (product != null)
                    {
                        product.counterOfSell += 1;
                        dbContext.Products.Update(product);
                        dbContext.SaveChanges();
                    }    
                    newSale.products.Add(item.Product);
                }
                if (IsDebtPayment)
                {
                    newSale.User = new UserInfo
                    {
                        PhoneNumber = CustomerPhone,
                        Username = CustomerName,
                    };
                }

                dbContext.Sales.Add(newSale);
                dbContext.SaveChanges();

                MessageBox.Show("تم إتمام البيع بنجاح!", "نجح",
                    MessageBoxButton.OK, MessageBoxImage.Information);

                ClearCart();
            }
        }

        private void PrintReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            if (CartItems.Count > 0)
            {
                MessageBox.Show("Receipt printed successfully!", "Success",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearCartButton_Click(object sender, RoutedEventArgs e)
        {
            ClearCart();
        }

        private void ClearCart()
        {
            CartItems.Clear();
            Total = 0;
            NetTotal = 0;
            Discount = 0;
            PaidAmount = 0;
            RemainingAmount = 0;
            CustomerName = string.Empty;
            CustomerPhone = string.Empty;
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            var searchedItem = dbContext.Products.FirstOrDefault(p => p.Barcode == searchTextBox.Text);

            if (searchedItem == null)
            {
                MessageBox.Show("لم يتم العثور علي المنتج", "فشل",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Products.Clear();
                Products.Add(searchedItem);
            }

        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}