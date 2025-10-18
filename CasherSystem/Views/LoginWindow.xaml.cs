using System.Windows;
using System.Windows.Controls;

namespace CasherSystem.Views
{
    public partial class LoginWindow : Window
    {
        private string _username = string.Empty;
        private string _password = string.Empty;
        private string _errorMessage = string.Empty;

        public string Username
        {
            get => _username;
            set => _username = value;
        }

        public string Password
        {
            get => _password;
            set => _password = value;
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                // Update UI if needed
                if (ErrorMessageTextBlock != null)
                {
                    ErrorMessageTextBlock.Text = value;
                    ErrorMessageTextBlock.Visibility = string.IsNullOrEmpty(value) ? Visibility.Collapsed : Visibility.Visible;
                }
            }
        }

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Password = passwordBox.Password;
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorMessage = string.Empty;

            if (Username == "admin" && Password == "fightClub12345678#")
            {
                try
                {
                    // Open main window
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    this.Close();
                }
                catch (Exception ex)
                {
                    ErrorMessage = $"خطأ في فتح النافذة الرئيسية: {ex.Message}";
                }
            }
            else
            {
                ErrorMessage = "اسم المستخدم أو كلمة المرور غير صحيحة. يرجى المحاولة مرة أخرى.";
            }
        }
    }
}