using CasherSystem.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using CasherSystem.Data;

namespace CasherSystem.Views
{
    public partial class PurchaseFormPage : Page, INotifyPropertyChanged
    {
        private readonly AppDbContext dbContext;
        private Purchase _currentPurchase = new Purchase();
        private string _formTitle = "إضافة شراء جديد";
        private string _formSubtitle = "أدخل بيانات عملية الشراء الجديدة";
        private string _formMode = "وضع الإضافة";
        private string _statusMessage = "جاهز";
        private bool _canSave = true;
        private ObservableCollection<string> _validationErrors = new ObservableCollection<string>();
        private bool _hasValidationErrors = false;
        private bool _isEditMode = false;

        public Purchase CurrentPurchase
        {
            get => _currentPurchase;
            set { _currentPurchase = value; OnPropertyChanged(); }
        }

        public string FormTitle
        {
            get => _formTitle;
            set { _formTitle = value; OnPropertyChanged(); }
        }

        public string FormSubtitle
        {
            get => _formSubtitle;
            set { _formSubtitle = value; OnPropertyChanged(); }
        }

        public string FormMode
        {
            get => _formMode;
            set { _formMode = value; OnPropertyChanged(); }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        public bool CanSave
        {
            get => _canSave;
            set { _canSave = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> ValidationErrors
        {
            get => _validationErrors;
            set { _validationErrors = value; OnPropertyChanged(); }
        }

        public bool HasValidationErrors
        {
            get => _hasValidationErrors;
            set { _hasValidationErrors = value; OnPropertyChanged(); }
        }

        public decimal Total
        {
            get => CurrentPurchase.Total;
            set 
            { 
                CurrentPurchase.Total = value; 
                OnPropertyChanged();
                CalculateRemainingAmount();
            }
        }

        public decimal PaidAmount
        {
            get => CurrentPurchase.PaidAmount;
            set 
            { 
                CurrentPurchase.PaidAmount = value; 
                OnPropertyChanged();
                CalculateRemainingAmount();
            }
        }

        public decimal RemainingAmount
        {
            get => CurrentPurchase.RemainingAmount;
            set 
            { 
                CurrentPurchase.RemainingAmount = value; 
                OnPropertyChanged();
            }
        }

        public PurchaseFormPage()
        {
            InitializeComponent();
            DataContext = this;
            dbContext = App.GetService<AppDbContext>();
            InitializeNewPurchase();
        }

        public PurchaseFormPage(Purchase purchaseToEdit)
        {
            InitializeComponent();
            DataContext = this;
            dbContext = App.GetService<AppDbContext>();
            LoadPurchaseForEdit(purchaseToEdit);
        }

        private void InitializeNewPurchase()
        {
            CurrentPurchase = new Purchase
            {
                Date = DateTime.Now,
                Supplier = new Supplier(),
                Total = 0,
                PaidAmount = 0,
                RemainingAmount = 0
            };
            FormTitle = "إضافة شراء جديد";
            FormSubtitle = "أدخل بيانات عملية الشراء الجديدة";
            FormMode = "وضع الإضافة";
        }

        private void LoadPurchaseForEdit(Purchase purchase)
        {
            CurrentPurchase = purchase;
            _isEditMode = true;
            FormTitle = "تعديل بيانات الشراء";
            FormSubtitle = "قم بتعديل بيانات عملية الشراء المحددة";
            FormMode = "وضع التعديل";
            CalculateRemainingAmount();
        }

        private void CalculateRemainingAmount()
        {
            RemainingAmount = Total - PaidAmount;
        }

        private void SavePurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateForm())
            {
                StatusMessage = "يرجى تصحيح الأخطاء قبل الحفظ";
                return;
            }

            try
            {
                // Calculate remaining amount
                CurrentPurchase.RemainingAmount = CurrentPurchase.Total - CurrentPurchase.PaidAmount;

                if (_isEditMode)
                {
                    // Update existing purchase
                    var existingPurchase = dbContext.Purchases.Find(CurrentPurchase.Id);
                    if (existingPurchase != null)
                    {
                        existingPurchase.Date = CurrentPurchase.Date;
                        existingPurchase.Supplier = CurrentPurchase.Supplier;
                        existingPurchase.Total = CurrentPurchase.Total;
                        existingPurchase.PaidAmount = CurrentPurchase.PaidAmount;
                        existingPurchase.RemainingAmount = CurrentPurchase.RemainingAmount;
                        
                        dbContext.SaveChanges();
                        
                        StatusMessage = "تم تحديث بيانات الشراء بنجاح";
                        MessageBox.Show("تم تحديث عملية الشراء بنجاح!", "نجاح",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        StatusMessage = "لم يتم العثور على عملية الشراء للتحديث";
                        MessageBox.Show("لم يتم العثور على عملية الشراء المحددة!", "خطأ",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }
                else
                {
                    // Add new purchase
                    dbContext.Purchases.Add(CurrentPurchase);
                    dbContext.SaveChanges();
                    
                    StatusMessage = "تم حفظ بيانات الشراء بنجاح";
                    MessageBox.Show("تم حفظ عملية الشراء بنجاح!", "نجاح",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Navigate back to purchases list
                NavigationService?.GoBack();
            }
            catch (Exception ex)
            {
                StatusMessage = "خطأ في حفظ البيانات";
                MessageBox.Show($"حدث خطأ أثناء حفظ البيانات: {ex.Message}", "خطأ",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void ClearFormButton_Click(object sender, RoutedEventArgs e)
        {
            _isEditMode = false;
            InitializeNewPurchase();
            StatusMessage = "تم تفريغ الحقول";
        }

        private bool ValidateForm()
        {
            ValidationErrors.Clear();
            HasValidationErrors = false;

            if (string.IsNullOrWhiteSpace(CurrentPurchase.Supplier?.Name))
                ValidationErrors.Add("اسم المورد مطلوب");

            if (string.IsNullOrWhiteSpace(CurrentPurchase.Supplier?.PhoneNumber))
                ValidationErrors.Add("رقم هاتف المورد مطلوب");

            if (CurrentPurchase.Total <= 0)
                ValidationErrors.Add("إجمالي الفاتورة يجب أن يكون أكبر من الصفر");

            if (CurrentPurchase.PaidAmount < 0)
                ValidationErrors.Add("المبلغ المدفوع لا يمكن أن يكون سالب");

            if (CurrentPurchase.PaidAmount > CurrentPurchase.Total)
                ValidationErrors.Add("المبلغ المدفوع لا يمكن أن يكون أكبر من الإجمالي");

            HasValidationErrors = ValidationErrors.Count > 0;

            return !HasValidationErrors;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}