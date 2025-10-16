using CasherSystem.Data;
using CasherSystem.utilities;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CasherSystem.Views
{
    public partial class ReportsPage : UserControl, INotifyPropertyChanged
    {
        private InvoicePrinter _invoicePrinter = new InvoicePrinter();
        private PDFGenerator PDFGenerator = new PDFGenerator();
        private readonly AppDbContext dbContext = App.GetService<AppDbContext>();
        private DateTime _startDate = DateTime.Today.AddDays(-30);
        private DateTime _endDate = DateTime.Today;
        private decimal _salesTotal;
        private int _salesCount;
        private decimal _purchasesTotal;
        private decimal _returnsTotal;
        private decimal _netProfit;

        public DateTime StartDate
        {
            get => _startDate;
            set
            {
                _startDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public decimal SalesTotal
        {
            get => _salesTotal;
            set
            {
                _salesTotal = value;
                OnPropertyChanged();
            }
        }

        public int SalesCount
        {
            get => _salesCount;
            set
            {
                _salesCount = value;
                OnPropertyChanged();
            }
        }

        public decimal PurchasesTotal
        {
            get => _purchasesTotal;
            set
            {
                _purchasesTotal = value;
                OnPropertyChanged();
            }
        }

        public decimal ReturnsTotal
        {
            get => _returnsTotal;
            set
            {
                _returnsTotal = value;
                OnPropertyChanged();
            }
        }

        public decimal NetProfit
        {
            get => _netProfit;
            set
            {
                _netProfit = value;
                OnPropertyChanged();
            }
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

            var salesInRange = dbContext.Sales
                .Where(s => s.Date >= StartDate && s.Date < EndDate.AddDays(1))
                .ToList();
            var returnsInRange = dbContext.Returns
                .Where(r => r.Date >= StartDate && r.Date < EndDate.AddDays(1))
                .ToList();
            var purchasesInRange = dbContext.Purchases
                .Where(r => r.Date >= StartDate && r.Date < EndDate.AddDays(1))
                .ToList();

            SalesTotal = salesInRange.Sum(s => s.NetTotal);
            SalesCount = salesInRange.Count();
            PurchasesTotal = purchasesInRange.Sum(s => s.Total);
            ReturnsTotal = returnsInRange.Sum(s => s.TotalRefund);
            NetProfit = SalesTotal - PurchasesTotal - ReturnsTotal;

        }


        private void PrintReportButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += (sender, e) =>
            {
                _invoicePrinter.PrintStoreOwnerReport(
                    graphics: e.Graphics,
                    salesTotal: SalesTotal,
                    salesCount: SalesCount,
                    purchasesTotal: PurchasesTotal,
                    returnsTotal: ReturnsTotal,
                    netProfit: NetProfit,
                    startDate: StartDate.ToString("yyyy/MM/dd"),
                    endDate: EndDate.ToString("yyyy/MM/dd"),
                    width: e.PageBounds.Width
                );

                e.HasMorePages = false; // Important: indicate no more pages
            };

            // Print directly to default printer
            printDoc.Print();
        }

        private void ExportToPdfButton_Click(object sender, RoutedEventArgs e)
        {
          PDFGenerator.ExportToPdf(
                    salesTotal: SalesTotal,
                    salesCount: SalesCount,
                    purchasesTotal: PurchasesTotal,
                    returnsTotal: ReturnsTotal,
                    netProfit: NetProfit,
                    startDate: StartDate.ToString("yyyy/MM/dd"),
                    endDate: EndDate.ToString("yyyy/MM/dd")
                );
        }

        private void ExportToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("ميزة تصدير Excel قيد التطوير", "معلومات",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}