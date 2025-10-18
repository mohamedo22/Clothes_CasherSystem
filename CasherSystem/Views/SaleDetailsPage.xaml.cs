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
    public partial class SaleDetailsPage : Page, INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext = App.GetService<AppDbContext>();
        private Sale? _sale;
        private int _totalProductsCount;
        private int _totalQuantitySold;

        public Sale? Sale
        {
            get => _sale;
            set
            {
                _sale = value;
                OnPropertyChanged();
                CalculateSummary();
            }
        }

        public int TotalProductsCount
        {
            get => _totalProductsCount;
            set
            {
                _totalProductsCount = value;
                OnPropertyChanged();
            }
        }

        public int TotalQuantitySold
        {
            get => _totalQuantitySold;
            set
            {
                _totalQuantitySold = value;
                OnPropertyChanged();
            }
        }

        public SaleDetailsPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public SaleDetailsPage(Sale sale) : this()
        {
            LoadSaleDetails(sale.Id);
        }

        public SaleDetailsPage(int saleId) : this()
        {
            LoadSaleDetails(saleId);
        }

        private void LoadSaleDetails(int saleId)
        {
            try
            {
                var sale = dbContext.Sales
                    .Include(s => s.User)
                    .Include(s => s.products)
                        .ThenInclude(sp => sp.Product)
                    .FirstOrDefault(s => s.Id == saleId);

                if (sale != null)
                {
                    Sale = sale;
                }
                else
                {
                    MessageBox.Show("لم يتم العثور على الفاتورة المطلوبة", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل تفاصيل الفاتورة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateSummary()
        {
            if (Sale?.products != null)
            {
                TotalProductsCount = Sale.products.Count;
                TotalQuantitySold = Sale.products.Sum(sp => sp.saledQuantity);
            }
            else
            {
                TotalProductsCount = 0;
                TotalQuantitySold = 0;
            }
        }


        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}