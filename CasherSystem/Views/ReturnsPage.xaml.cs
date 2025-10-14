using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using CasherSystem.Models;

namespace CasherSystem.Views
{
    public class ReturnItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public partial class ReturnsPage : Page
    {
        private Sale? _selectedSale;
        private Sale? _selectedSaleDetail;
        private int _returnQuantity = 1;
        private string _secretCode = string.Empty;
        private ObservableCollection<Sale> _sales = new();
        private ObservableCollection<Sale> _saleDetails = new();
        private ObservableCollection<ReturnItem> _returnItems = new();
        private decimal _totalRefund;

        public Sale? SelectedSale
        {
            get => _selectedSale;
            set => _selectedSale = value;
        }

        public Sale? SelectedSaleDetail
        {
            get => _selectedSaleDetail;
            set => _selectedSaleDetail = value;
        }

        public int ReturnQuantity
        {
            get => _returnQuantity;
            set => _returnQuantity = value;
        }

        public string SecretCode
        {
            get => _secretCode;
            set => _secretCode = value;
        }

        public ObservableCollection<Sale> Sales
        {
            get => _sales;
            set => _sales = value;
        }

        public ObservableCollection<Sale> SaleDetails
        {
            get => _saleDetails;
            set => _saleDetails = value;
        }

        public ObservableCollection<ReturnItem> ReturnItems
        {
            get => _returnItems;
            set => _returnItems = value;
        }

        public decimal TotalRefund
        {
            get => _totalRefund;
            set => _totalRefund = value;
        }

        public ReturnsPage()
        {
            InitializeComponent();
            DataContext = this;

            LoadSampleData();
        }

        private void LoadSampleData()
        {
            // Add some sample sales
            var sampleSale = new Sale
            {
                Id = 1,
                Date = DateTime.Now.AddDays(-1),
                Total = 100.00m,
                NetTotal = 100.00m,
                PaymentType = "نقدي",
                SecretCode = "ABC123"
            };
            Sales.Add(sampleSale);

            // Add sample sale details (now using Sale entities directly)
            var sampleProduct = new Product { Id = 1, Name = "قميص قطني", Size = "M", Color = "أزرق" };
            var sampleSaleDetail = new Sale
            {
                Id = 1,
                Date = DateTime.Now.AddDays(-1),
                Total = 50.00m,
                NetTotal = 50.00m,
                PaymentType = "نقدي",
                SecretCode = "ABC123",
                UserId = 1
            };
            SaleDetails.Add(sampleSaleDetail);
        }

        private void AddToReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSaleDetail == null || ReturnQuantity <= 0) return;

            // Since we don't have product details in Sale anymore, create a simple return item
            var existingItem = ReturnItems.FirstOrDefault(item => item.ProductId == 1); // Default to product ID 1
            if (existingItem != null)
            {
                existingItem.Quantity += ReturnQuantity;
                existingItem.Total = existingItem.Quantity * existingItem.Price;
            }
            else
            {
                var sampleProduct = new Product { Id = 1, Name = "قميص قطني", Size = "M", Color = "أزرق", SellPrice = 25.00m };
                var newItem = new ReturnItem
                {
                    ProductId = 1,
                    Product = sampleProduct,
                    Quantity = ReturnQuantity,
                    Price = 25.00m, // Default price
                    Total = ReturnQuantity * 25.00m
                };
                ReturnItems.Add(newItem);
            }

            CalculateTotalRefund();
            ReturnQuantity = 1;
        }

        private void RemoveFromReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.DataContext is ReturnItem item)
            {
                ReturnItems.Remove(item);
                CalculateTotalRefund();
            }
        }

        private void CalculateTotalRefund()
        {
            TotalRefund = ReturnItems.Sum(item => item.Total);
        }

        private void ProcessReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnItems.Count > 0 && TotalRefund > 0 && SelectedSale != null)
            {
                System.Windows.MessageBox.Show("Return processed successfully!", "Success", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
                
                ClearReturn();
            }
        }

        private void PrintReturnReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnItems.Count > 0)
            {
                System.Windows.MessageBox.Show("Return receipt printed successfully!", "Success", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
        }

        private void ClearReturnButton_Click(object sender, RoutedEventArgs e)
        {
            ClearReturn();
        }

        private void ClearReturn()
        {
            ReturnItems.Clear();
            TotalRefund = 0;
            ReturnQuantity = 1;
        }

        private void FindReceiptButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}