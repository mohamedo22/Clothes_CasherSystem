using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CasherSystem.Views
{
    public class TopSellingProductData
    {
        public int Rank { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int QuantitySold { get; set; }
        public decimal Revenue { get; set; }
    }

    public partial class DashboardPage : UserControl
    {
        private decimal _todaySalesTotal = 0;
        private int _todaySalesCount = 0;
        private string _topProductName = "لا توجد بيانات";
        private ObservableCollection<TopSellingProductData> _topSellingProductsList = new();

        public decimal TodaySalesTotal
        {
            get => _todaySalesTotal;
            set => _todaySalesTotal = value;
        }

        public int TodaySalesCount
        {
            get => _todaySalesCount;
            set => _todaySalesCount = value;
        }

        public string TopProductName
        {
            get => _topProductName;
            set => _topProductName = value;
        }

        public ObservableCollection<TopSellingProductData> TopSellingProductsList
        {
            get => _topSellingProductsList;
            set => _topSellingProductsList = value;
        }

        public DashboardPage()
        {
            InitializeComponent();
            DataContext = this;

            // Initialize with sample data
            TodaySalesTotal = 1250.50m;
            TodaySalesCount = 15;
            TopProductName = "قميص قطني";
            
            LoadTopSellingProducts();
        }

        private void LoadTopSellingProducts()
        {
            TopSellingProductsList.Add(new TopSellingProductData
            {
                Rank = 1,
                ProductName = "قميص قطني",
                QuantitySold = 25,
                Revenue = 625.00m
            });

            TopSellingProductsList.Add(new TopSellingProductData
            {
                Rank = 2,
                ProductName = "جينز",
                QuantitySold = 18,
                Revenue = 1080.00m
            });

            TopSellingProductsList.Add(new TopSellingProductData
            {
                Rank = 3,
                ProductName = "هودي",
                QuantitySold = 12,
                Revenue = 900.00m
            });

            TopSellingProductsList.Add(new TopSellingProductData
            {
                Rank = 4,
                ProductName = "فستان صيفي",
                QuantitySold = 10,
                Revenue = 450.00m
            });

            TopSellingProductsList.Add(new TopSellingProductData
            {
                Rank = 5,
                ProductName = "حذاء رياضي",
                QuantitySold = 8,
                Revenue = 720.00m
            });
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for refresh functionality
            // In a real application, this would load data from the database
        }
    }
}