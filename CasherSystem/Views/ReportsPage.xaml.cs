using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CasherSystem.Views
{
    public partial class ReportsPage : UserControl
    {
        private DateTime _startDate = DateTime.Today.AddDays(-30);
        private DateTime _endDate = DateTime.Today;
        private decimal _salesTotal;
        private int _salesCount;
        private decimal _purchasesTotal;
        private decimal _returnsTotal;
        private decimal _netProfit;
        private ObservableCollection<(string ProductName, int QuantitySold)> _topSellingProducts = new();

        public DateTime StartDate
        {
            get => _startDate;
            set => _startDate = value;
        }

        public DateTime EndDate
        {
            get => _endDate;
            set => _endDate = value;
        }

        public decimal SalesTotal
        {
            get => _salesTotal;
            set => _salesTotal = value;
        }

        public int SalesCount
        {
            get => _salesCount;
            set => _salesCount = value;
        }

        public decimal PurchasesTotal
        {
            get => _purchasesTotal;
            set => _purchasesTotal = value;
        }

        public decimal ReturnsTotal
        {
            get => _returnsTotal;
            set => _returnsTotal = value;
        }

        public decimal NetProfit
        {
            get => _netProfit;
            set => _netProfit = value;
        }

        public ObservableCollection<(string ProductName, int QuantitySold)> TopSellingProducts
        {
            get => _topSellingProducts;
            set => _topSellingProducts = value;
        }

        public ReportsPage()
        {
            InitializeComponent();
            DataContext = this;

            GenerateReport();
        }

        private void GenerateReportButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateReport();
        }

        private void GenerateReport()
        {
            // Placeholder for report generation
            SalesTotal = 1500.00m;
            SalesCount = 25;
            PurchasesTotal = 800.00m;
            ReturnsTotal = 50.00m;
            NetProfit = SalesTotal - PurchasesTotal - ReturnsTotal;
            
            // Add sample top selling products
            TopSellingProducts.Clear();
            TopSellingProducts.Add(("قميص قطني", 15));
            TopSellingProducts.Add(("جينز", 12));
            TopSellingProducts.Add(("هودي", 8));
            TopSellingProducts.Add(("فستان صيفي", 6));
            TopSellingProducts.Add(("حذاء رياضي", 4));
        }

        private void PrintReportButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("ميزة طباعة التقرير قيد التطوير", "معلومات", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("ميزة تصدير PDF قيد التطوير", "معلومات", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("ميزة تصدير Excel قيد التطوير", "معلومات", 
                System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }
}