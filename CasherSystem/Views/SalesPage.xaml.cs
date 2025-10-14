using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CasherSystem.Models;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CasherSystem.Views
{
    public class PaymentMethod
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CartItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public partial class SalesPage : Page, INotifyPropertyChanged
    {
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

            // Initialize payment methods
            PaymentMethods = new List<PaymentMethod>
            {
                new PaymentMethod { Id = 1, Name = "Cash" },
                new PaymentMethod { Id = 2, Name = "Debt" }
            };
            SelectedPaymentMethod = PaymentMethods.First(); // Default to Cash

            // Initialize with sample data
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Add some sample products
            Products.Add(new Product { Id = 1, Name = "قميص قطني", Barcode = "TSH001", Size = "M", Color = "أزرق", CostPrice = 15.00m, SellPrice = 25.00m, Quantity = 50 });
            Products.Add(new Product { Id = 2, Name = "جينز", Barcode = "JEA001", Size = "L", Color = "أزرق", CostPrice = 35.00m, SellPrice = 60.00m, Quantity = 30 });
            Products.Add(new Product { Id = 3, Name = "هودي", Barcode = "HOO001", Size = "XL", Color = "أسود", CostPrice = 40.00m, SellPrice = 75.00m, Quantity = 20 });
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null || Quantity <= 0) return;

            var existingItem = CartItems.FirstOrDefault(item => item.ProductId == SelectedProduct.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += Quantity;
                existingItem.Total = existingItem.Quantity * existingItem.Price;
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
            // Search functionality can be implemented here
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}