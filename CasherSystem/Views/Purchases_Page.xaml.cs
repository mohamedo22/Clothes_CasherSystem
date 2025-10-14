using CasherSystem.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace CasherSystem.Views
{
    public partial class PurchaseFormPage : Page, INotifyPropertyChanged
    {
        private Purchase _currentPurchase = new Purchase();
        private string _formTitle = "إضافة شراء جديد";
        private string _formSubtitle = "أدخل بيانات عملية الشراء الجديدة";
        private string _formMode = "وضع الإضافة";
        private string _statusMessage = "جاهز";
        private bool _canSave = true;
        private ObservableCollection<string> _validationErrors = new ObservableCollection<string>();
        private bool _hasValidationErrors = false;

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

        public PurchaseFormPage()
        {
            InitializeComponent();
            DataContext = this;
            InitializeNewPurchase();
        }

        public PurchaseFormPage(Purchase purchaseToEdit)
        {
            InitializeComponent();
            DataContext = this;
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
            FormTitle = "تعديل بيانات الشراء";
            FormSubtitle = "قم بتعديل بيانات عملية الشراء المحددة";
            FormMode = "وضع التعديل";
        }

        private void SavePurchaseButton_Click(object sender, RoutedEventArgs e)
        {
            if (ValidateForm())
            {
                // Save logic here
                StatusMessage = "تم حفظ بيانات الشراء بنجاح";
                MessageBox.Show("تم حفظ عملية الشراء بنجاح!", "نجاح",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                StatusMessage = "يوجد أخطاء في البيانات المدخلة";
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back or clear form
            if (MessageBox.Show("هل تريد إلغاء العملية؟", "تأكيد الإلغاء",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                InitializeNewPurchase();
                StatusMessage = "تم إلغاء العملية";
            }
        }

        private void ClearFormButton_Click(object sender, RoutedEventArgs e)
        {
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
            CanSave = !HasValidationErrors;

            return !HasValidationErrors;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}