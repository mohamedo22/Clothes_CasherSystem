using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CasherSystem.Models;

namespace CasherSystem.Views
{
    public class PurchaseItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal CostPrice { get; set; }
        public decimal Total { get; set; }
    }

    public partial class PurchasesPage : Page
    {
        private Product? _selectedProduct;
        private int _quantity = 1;
        private decimal _costPrice;
        private string _newProductName = string.Empty;
        private string _newProductBarcode = string.Empty;
        private string _newProductSize = string.Empty;
        private string _newProductColor = string.Empty;
        private decimal _newProductSellPrice;
        private string _supplierName = string.Empty;
        private decimal _paidAmount;
        private decimal _remainingAmount;
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<PurchaseItem> _purchaseItems = new();
        private decimal _total;

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => _selectedProduct = value;
        }

        public int Quantity
        {
            get => _quantity;
            set => _quantity = value;
        }

        public decimal CostPrice
        {
            get => _costPrice;
            set => _costPrice = value;
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set => _products = value;
        }

        public ObservableCollection<PurchaseItem> PurchaseItems
        {
            get => _purchaseItems;
            set => _purchaseItems = value;
        }

        public decimal Total
        {
            get => _total;
            set => _total = value;
        }

        public string NewProductName
        {
            get => _newProductName;
            set => _newProductName = value;
        }

        public string NewProductBarcode
        {
            get => _newProductBarcode;
            set => _newProductBarcode = value;
        }

        public string NewProductSize
        {
            get => _newProductSize;
            set => _newProductSize = value;
        }

        public string NewProductColor
        {
            get => _newProductColor;
            set => _newProductColor = value;
        }

        public decimal NewProductSellPrice
        {
            get => _newProductSellPrice;
            set => _newProductSellPrice = value;
        }

        public string SupplierName
        {
            get => _supplierName;
            set => _supplierName = value;
        }

        public decimal PaidAmount
        {
            get => _paidAmount;
            set => _paidAmount = value;
        }

        public decimal RemainingAmount
        {
            get => _remainingAmount;
            set => _remainingAmount = value;
        }

        public PurchasesPage()
        {
            InitializeComponent();
            DataContext = this;

            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Add some sample products
            Products.Add(new Product { Id = 1, Name = "قميص قطني", Barcode = "TSH001", Size = "M", Color = "أزرق", CostPrice = 15.00m, SellPrice = 25.00m, Quantity = 50 });
            Products.Add(new Product { Id = 2, Name = "جينز", Barcode = "JEA001", Size = "L", Color = "أزرق", CostPrice = 35.00m, SellPrice = 60.00m, Quantity = 30 });
            Products.Add(new Product { Id = 3, Name = "هودي", Barcode = "HOO001", Size = "XL", Color = "أسود", CostPrice = 40.00m, SellPrice = 75.00m, Quantity = 20 });
        }

        private void AddToPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedProduct == null || Quantity <= 0 || CostPrice <= 0) return;

            var existingItem = PurchaseItems.FirstOrDefault(item => item.ProductId == SelectedProduct.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += Quantity;
                existingItem.CostPrice = CostPrice;
                existingItem.Total = existingItem.Quantity * existingItem.CostPrice;
            }
            else
            {
                var newItem = new PurchaseItem
                {
                    ProductId = SelectedProduct.Id,
                    Product = SelectedProduct,
                    Quantity = Quantity,
                    CostPrice = CostPrice,
                    Total = Quantity * CostPrice
                };
                PurchaseItems.Add(newItem);
            }

            CalculateTotal();
            Quantity = 1;
        }

        private void RemoveFromPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is PurchaseItem item)
            {
                PurchaseItems.Remove(item);
                CalculateTotal();
            }
        }

        private void CalculateTotal()
        {
            Total = PurchaseItems.Sum(item => item.Total);
        }

        private void SavePurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (PurchaseItems.Count > 0 && Total > 0)
            {
                System.Windows.MessageBox.Show("تم حفظ المشتريات بنجاح!", "نجح", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                
                ClearPurchase();
            }
        }

        private void ClearPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            ClearPurchase();
        }

        private void ClearPurchase()
        {
            PurchaseItems.Clear();
            Total = 0;
            Quantity = 1;
        }

        private void CreateNewProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewProductName) && 
                !string.IsNullOrWhiteSpace(NewProductBarcode) &&
                CostPrice > 0 && 
                NewProductSellPrice > 0)
            {
                System.Windows.MessageBox.Show("New product created successfully!", "Success", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                
                ClearNewProductForm();
            }
        }

        private void ClearNewProductForm()
        {
            NewProductName = string.Empty;
            NewProductBarcode = string.Empty;
            NewProductSize = string.Empty;
            NewProductColor = string.Empty;
            NewProductSellPrice = 0;
            CostPrice = 0;
        }
    }
}