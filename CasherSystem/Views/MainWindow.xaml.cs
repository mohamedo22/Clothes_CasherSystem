using System.Windows;
using CasherSystem.Models;

namespace CasherSystem.Views
{
    public partial class MainWindow : Window
    {
        private object _currentPage = null!;

        public object CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                MainFrame.Content = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            // Default to Dashboard page
            NavigateToDashboard();
        }

        private void NavigateToDashboard()
        {
            var dashboardPage = new DashboardPage();
            CurrentPage = dashboardPage;
        }

        private void NavigateToSales()
        {
            var salesPage = new SalesPage();
            CurrentPage = salesPage;
        }

        public void NavigateToSalesList()
        {
            var salesListPage = new SalesListPage();
            CurrentPage = salesListPage;
        }


        private void NavigateToPurchases()
        {
            var purchasesPage = new PurchasesListPage();
            CurrentPage = purchasesPage;
        }

        private void NavigateToReturns()
        {
            var returnsPage = new ReturnsPage();
            CurrentPage = returnsPage;
        }

        private void NavigateToReports()
        {
            var reportsPage = new ReportsPage();
            CurrentPage = reportsPage;
        }

        private void NavigateToProducts()
        {
            var productsListPage = new ProductsListPage();
            CurrentPage = productsListPage;
        }

        private void Exit()
        {
            Application.Current.Shutdown();
        }

        // Event handlers for navigation buttons
        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToDashboard();
        }

        private void SalesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSales();
        }

        private void SalesListButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSalesList();
        }

        private void PurchasesButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToPurchases();
        }

        private void ReturnsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToReturns();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToReports();
        }

        private void ProductsButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateToProducts();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Exit();
        }
    }
}