using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CasherSystem.Data;
using CasherSystem.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace CasherSystem.Views
{
    public partial class PurchasesListPage : Page, INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext;
        private ObservableCollection<Purchase> _purchases = new();
        private Purchase? _selectedPurchase;
        private string _searchTerm = string.Empty;
        private string _statusMessage = "جاهز";

        public ObservableCollection<Purchase> Purchases
        {
            get => _purchases;
            set { _purchases = value; OnPropertyChanged(); }
        }

        public Purchase? SelectedPurchase
        {
            get => _selectedPurchase;
            set { _selectedPurchase = value; OnPropertyChanged(); }
        }

        public string SearchTerm
        {
            get => _searchTerm;
            set { _searchTerm = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public PurchasesListPage()
        {
            InitializeComponent();
            DataContext = this;
            dbContext = App.GetService<AppDbContext>();
            LoadPurchases();
        }

        private void LoadPurchases()
        {
            try
            {
                var purchases = dbContext.Purchases.ToList();
                Purchases.Clear();
                foreach (var purchase in purchases)
                {
                    Purchases.Add(purchase);
                }
                StatusMessage = $"تم تحميل {Purchases.Count} عملية شراء";
            }
            catch (Exception ex)
            {
                StatusMessage = "خطأ في تحميل البيانات";
                MessageBox.Show($"خطأ في تحميل المشتريات: {ex.Message}", "خطأ", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handlers
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                LoadPurchases();
                return;
            }

            try
            {
                if (int.TryParse(SearchTerm, out int purchaseId))
                {
                    var searchedItem = dbContext.Purchases.FirstOrDefault(p => p.Id == purchaseId);
                    if (searchedItem == null)
                    {
                        MessageBox.Show("لم يتم العثور على الفاتورة", "غير موجود", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                        StatusMessage = "لم يتم العثور على الفاتورة";
                        return;
                    }

                    Purchases.Clear();
                    Purchases.Add(searchedItem);
                    StatusMessage = "تم العثور على الفاتورة";
                }
                else
                {
                    MessageBox.Show("يرجى إدخال رقم صحيح للبحث", "خطأ في الإدخال", 
                        MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                StatusMessage = "خطأ في البحث";
                MessageBox.Show($"خطأ في البحث: {ex.Message}", "خطأ", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddNewPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Navigate to purchase form page
                var purchaseFormPage = new PurchaseFormPage(); // Using existing PurchasesPage
                NavigationService?.Navigate(purchaseFormPage);
                StatusMessage = "انتقال إلى صفحة إضافة شراء جديد";
            }
            catch (Exception ex)
            {
                StatusMessage = "خطأ في الانتقال";
                MessageBox.Show($"خطأ في الانتقال: {ex.Message}", "خطأ", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadPurchases();
                StatusMessage = "تم تحديث البيانات";
            }
            catch (Exception ex)
            {
                StatusMessage = "خطأ في التحديث";
                MessageBox.Show($"خطأ في تحديث البيانات: {ex.Message}", "خطأ", 
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditPurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Purchase purchase)
            {
                try
                {
                    SelectedPurchase = purchase;
                    
                    // Navigate to edit page with the selected purchase
                    var purchaseFormPage = new PurchaseFormPage(purchase);
                    NavigationService?.Navigate(purchaseFormPage);
                    StatusMessage = $"انتقال إلى تعديل عملية الشراء رقم {purchase.Id}";
                }
                catch (Exception ex)
                {
                    StatusMessage = "خطأ في التعديل";
                    MessageBox.Show($"خطأ في تعديل عملية الشراء: {ex.Message}", "خطأ", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeletePurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is Purchase purchase)
            {
                try
                {
                    var result = MessageBox.Show($"هل أنت متأكد من حذف عملية الشراء رقم {purchase.Id}؟\n" +
                        "هذا الإجراء لا يمكن التراجع عنه.", 
                        "تأكيد الحذف", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    
                    if (result == MessageBoxResult.Yes)
                    {
                        dbContext.Purchases.Remove(purchase);
                        dbContext.SaveChanges();
                        
                        Purchases.Remove(purchase);
                        StatusMessage = $"تم حذف عملية الشراء رقم {purchase.Id} بنجاح";
                        
                        MessageBox.Show("تم حذف عملية الشراء بنجاح", "تم الحذف", 
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = "خطأ في الحذف";
                    MessageBox.Show($"خطأ في حذف عملية الشراء: {ex.Message}", "خطأ", 
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // INotifyPropertyChanged implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}