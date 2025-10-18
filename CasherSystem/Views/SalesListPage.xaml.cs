using CasherSystem.Data;
using CasherSystem.Models;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace CasherSystem.Views
{
    public partial class SalesListPage : Page, INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext = App.GetService<AppDbContext>();
        private ObservableCollection<Sale> _salesList = new();
        private Sale? _selectedSale;
        private string _currentFilter = "all";

        public ObservableCollection<Sale> SalesList
        {
            get => _salesList;
            set
            {
                _salesList = value;
                OnPropertyChanged();
            }
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

        public SalesListPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadSales();
        }

        private void LoadSales()
        {
            try
            {
                var query = dbContext.Sales
                    .Include(s => s.User)
                    .Include(s => s.products)
                        .ThenInclude(sp => sp.Product)
                    .AsQueryable();

                // Apply filter based on current selection
                switch (_currentFilter)
                {
                    case "cash":
                        query = query.Where(s => s.PaymentType.ToLower() == "cash");
                        break;
                    case "debt":
                        query = query.Where(s => s.PaymentType.ToLower() != "cash");
                        break;
                    // "all" - no additional filtering
                }

                var sales = query.OrderByDescending(s => s.Date).ToList();
                SalesList = new ObservableCollection<Sale>(sales);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في تحميل المبيعات: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdateFilterButtons(string selectedFilter)
        {
            // Reset all buttons to default style
            AllSalesButton.Style = (Style)FindResource("FilterButtonStyle");
            CashSalesButton.Style = (Style)FindResource("FilterButtonStyle");
            DebtSalesButton.Style = (Style)FindResource("FilterButtonStyle");

            // Highlight selected button
            switch (selectedFilter)
            {
                case "all":
                    AllSalesButton.Style = (Style)FindResource("ActiveFilterButtonStyle");
                    break;
                case "cash":
                    CashSalesButton.Style = (Style)FindResource("ActiveFilterButtonStyle");
                    break;
                case "debt":
                    DebtSalesButton.Style = (Style)FindResource("ActiveFilterButtonStyle");
                    break;
            }
        }

        private void AllSalesButton_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter = "all";
            UpdateFilterButtons(_currentFilter);
            LoadSales();
        }

        private void CashSalesButton_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter = "cash";
            UpdateFilterButtons(_currentFilter);
            LoadSales();
        }

        private void DebtSalesButton_Click(object sender, RoutedEventArgs e)
        {
            _currentFilter = "debt";
            UpdateFilterButtons(_currentFilter);
            LoadSales();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadSales();
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Sale sale)
            {
                NavigationService?.Navigate(new SaleDetailsPage(sale));
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Sale sale)
            {
                var result = MessageBox.Show(
                    $"هل أنت متأكد من حذف الفاتورة رقم {sale.Id}؟\nهذا الإجراء لا يمكن التراجع عنه.",
                    "تأكيد الحذف",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteSale(sale);
                }
            }
        }



        private void DeleteSale(Sale sale)
        {
            try
            {
                // Remove related SaledProducts first
                var saledProducts = dbContext.Set<SaledProduct>().Where(sp => sp.SaleId == sale.Id);
                dbContext.Set<SaledProduct>().RemoveRange(saledProducts);

                // Remove the sale
                dbContext.Sales.Remove(sale);
                dbContext.SaveChanges();

                // Refresh the list
                LoadSales();

                MessageBox.Show("تم حذف الفاتورة بنجاح", "تم الحذف", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"خطأ في حذف الفاتورة: {ex.Message}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
