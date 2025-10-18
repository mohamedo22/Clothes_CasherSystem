using CasherSystem.Data;
using CasherSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CasherSystem.Views
{
    public class ReturnItem : INotifyPropertyChanged
    {
        private int _quantity;
        private decimal _price;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SaleProductItem
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }

    public partial class ReturnsPage : Page , INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext = App.GetService<AppDbContext>();
        private Sale? _selectedSale;
        private SaleProductItem? _selectedSaleDetail;
        private Product? _selectedProduct;
        private int _returnQuantity = 1;
        private string _secretCode = string.Empty;
        private ObservableCollection<Sale> _sales = new();
        private ObservableCollection<SaleProductItem> _saleDetails = new();
        private ObservableCollection<ReturnItem> _returnItems = new();
        private decimal _totalRefund;

        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set { _selectedProduct = value; OnPropertyChanged(); }
        }

        public Sale? SelectedSale
        {
            get => _selectedSale;
            set
            {
                _selectedSale = value;
                OnPropertyChanged();
            }
        }

        public SaleProductItem? SelectedSaleDetail
        {
            get => _selectedSaleDetail;
            set
            {
                _selectedSaleDetail = value;
                SelectedProduct = value?.Product;
                OnPropertyChanged();
            }
        }

        public int ReturnQuantity
        {
            get => _returnQuantity;
            set
            {
                _returnQuantity = value;
                OnPropertyChanged();
            }
        }

        public string SecretCode
        {
            get => _secretCode;
            set
            {
                _secretCode = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Sale> Sales
        {
            get => _sales;
            set
            {
                _sales = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<SaleProductItem> SaleDetails
        {
            get => _saleDetails;
            set
            {
                _saleDetails = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ReturnItem> ReturnItems
        {
            get => _returnItems;
            set
            {
                _returnItems = value;
                OnPropertyChanged();
            }
        }

        public decimal TotalRefund
        {
            get => _totalRefund;
            set
            {
                _totalRefund = value;
                OnPropertyChanged();
            }
        }

        public ReturnsPage()
        {
            InitializeComponent();
            DataContext = this;
        }


        private void AddToReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedSaleDetail == null || SelectedSaleDetail.Product == null || ReturnQuantity <= 0) return;

            if(ReturnQuantity > SelectedSaleDetail.Quantity)
            {
                MessageBox.Show("كمية المرتجعات تتجاوز الكمية المباعة.", "قيمه خاطئه", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var product = SelectedSaleDetail.Product;
            var unitPrice = product.SellPrice;

            var existingItem = ReturnItems.FirstOrDefault(item => item.ProductId == product.Id);
            if (existingItem != null)
            {
                existingItem.Quantity += ReturnQuantity;
                SelectedSaleDetail.Quantity -= ReturnQuantity;
                existingItem.Price = existingItem.Quantity * unitPrice; // Price used as line total
            }
            else
            {
                ReturnItems.Add(new ReturnItem
                {
                    Product = product,
                    ProductId = product.Id,
                    Quantity = ReturnQuantity,
                    Price = unitPrice * ReturnQuantity, // Price used as line total
                });
                SelectedSaleDetail.Quantity -= ReturnQuantity;
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
            TotalRefund = ReturnItems.Sum(item => item.Price);
        }

        private void ProcessReturnButton_Click(object sender, RoutedEventArgs e)
        {
            if (ReturnItems.Count > 0 && TotalRefund > 0 && SelectedSale != null)
            {
                var returnRecord = new Return
                {
                    SaleId = SelectedSale.Id,
                    Date = DateTime.Now,
                    TotalRefund = TotalRefund,
                    products = ReturnItems.Select(ri => ri.Product).ToList()
                };
                dbContext.Returns.Add(returnRecord);
                dbContext.SaveChanges();
                foreach (var item in ReturnItems)
                {
                    var product = dbContext.syProducts.FirstOrDefault(p => p.Id == item.ProductId);
                    if (product != null)
                    {
                        product.Quantity += item.Quantity;
                        product.counterOfSell -= item.Quantity;
                        dbContext.syProducts.Update(product);
                        dbContext.SaveChanges();
                    }
                }
                var saleToUpdate = dbContext.Sales
                    .Include(s => s.products)
                    .FirstOrDefault(s => s.Id == SelectedSale.Id);
                if (saleToUpdate != null)
                {

                    foreach (var returnItem in ReturnItems)
                    {
                        var saleProduct = saleToUpdate.products
                            .FirstOrDefault(sp => sp.ProductId == returnItem.ProductId);
                        if (saleProduct != null)
                        {
                            saleProduct.saledQuantity -= returnItem.Quantity;
                            if (saleProduct.saledQuantity <= 0)
                            {
                                saleToUpdate.products.Remove(saleProduct);
                            }
                        }
                    }
                    dbContext.Sales.Update(saleToUpdate);
                    dbContext.SaveChanges();

                }

                MessageBox.Show("تم ارجاع المنتجات بنجاح", "تم", 
                  MessageBoxButton.OK, MessageBoxImage.Information);
                
                ClearReturn();
            }
        }


        private void ClearReturnButton_Click(object sender, RoutedEventArgs e)
        {
            ClearReturn();
        }

        private void ClearReturn()
        {
            foreach (var item in ReturnItems)
            {
                var productInSale = SaleDetails.FirstOrDefault(sd => sd.ProductId == item.ProductId);
                if (productInSale != null)
                {
                    productInSale.Quantity += item.Quantity;
                }

            }
            ReturnItems.Clear();
            TotalRefund = 0;
            ReturnQuantity = 1;
        }

        private void FindReceiptButton_Click(object sender, RoutedEventArgs e)
        {
            var inputCode = SecretCodeTextBox.Text.Trim();
            var foundSale = dbContext.Sales
                .Include(s => s.products)
                .FirstOrDefault(sale => sale.SecretCode == inputCode);
            
            if (foundSale != null)
            {
                SelectedSale = foundSale;
                SaleDetails.Clear();
                
                // Load products from the sale
                if (foundSale.products != null && foundSale.products.Any())
                {
                    foreach (var product in foundSale.products)
                    {
                        var saleProductItem = new SaleProductItem
                        {
                            ProductId = product.Id,
                            Product = product.Product,
                            Quantity = product.saledQuantity, 
                            Price = product.Product.SellPrice,
                            Total = product.Product.SellPrice
                        };
                        SaleDetails.Add(saleProductItem);
                    }
                }
                else
                {
                    // If no products found, show a message
                    MessageBox.Show("لا توجد منتجات في هذه الفاتورة", "فاتورة فارغة", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("لم يتم العثور علي الفاتورة", "غير موجودة", 
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                SelectedSale = null;
                SaleDetails.Clear();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}