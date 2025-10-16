using CasherSystem.Data;
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
        private readonly AppDbContext dbContext;
        private decimal _todaySalesTotal = 0;
        private int _todaySalesCount = 0;
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


        public ObservableCollection<TopSellingProductData> TopSellingProductsList
        {
            get => _topSellingProductsList;
            set => _topSellingProductsList = value;
        }

        public DashboardPage()
        {
            InitializeComponent();
            DataContext = this;

            dbContext = App.GetService<AppDbContext>();

            var todaySales = dbContext.Sales.Where(s => s.Date.Date == DateTime.Now.Date).ToList();

            var todayReturns = dbContext.Returns.Where(r => r.Date.Date == DateTime.Now.Date).ToList();

            TodaySalesTotal = todaySales.Sum(s => s.NetTotal) - todayReturns.Sum(s=>s.TotalRefund);

            TodaySalesCount = todaySales.Count();
            
            LoadTopSellingProducts();
        }

        private void LoadTopSellingProducts()
        {
            var top5Sales = dbContext.Products
                               .OrderByDescending(s => s.counterOfSell)
                               .Take(5)
                               .ToList();
            TopSellingProductsList.Clear();
            TopSellingProductsList = new ObservableCollection<TopSellingProductData>(
                top5Sales.Select((p, index) => new TopSellingProductData
                {
                    Rank = index + 1,
                    ProductName = p.Name,
                    QuantitySold = p.counterOfSell,
                    Revenue = p.counterOfSell * p.SellPrice
                })
            );
            for (int i = 0; i < TopSellingProductsList.Count; i++)
            {
                var productNameControl = FindName($"Product{i + 1}Name") as TextBlock;
                var quantityControl = FindName($"Product{i + 1}QuantitySold") as TextBlock;


                productNameControl.Text = TopSellingProductsList[i].ProductName;

                quantityControl.Text = TopSellingProductsList[i].QuantitySold.ToString();


            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadTopSellingProducts();
        }
    }
}