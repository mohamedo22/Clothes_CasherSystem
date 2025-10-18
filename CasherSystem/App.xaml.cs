using System.Configuration;
using System.Data;
using System.Windows;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using CasherSystem.Data;
using CasherSystem.Views;

namespace CasherSystem
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost? _host;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // Configure services
            _host = Host.CreateDefaultBuilder()
                .ConfigureServices((context, services) =>
                {
                    // Database
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite("Data Source=system_database.db"));

                    //services.AddDbContext<AppDbContext>(options =>
                    //    options.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=casherSys;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

                    // Views
                    services.AddTransient<LoginWindow>();
                    services.AddTransient<MainWindow>();
                    services.AddTransient<DashboardPage>();
                    services.AddTransient<SalesPage>();
                    services.AddTransient<PurchasesPage>();
                    services.AddTransient<ReturnsPage>();
                    services.AddTransient<ReportsPage>();
                    services.AddTransient<SaleDetailsPage>();
                })
                .Build();

            // Initialize database asynchronously
            Task.Run(async () =>
            {
                try
                {
                    using (var scope = _host.Services.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        await context.Database.MigrateAsync();
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Database initialization failed: {ex.Message}", "Error", 
                        System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                }
            });

            // Show login window
            var loginWindow = _host.Services.GetRequiredService<LoginWindow>();
            loginWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _host?.Dispose();
            base.OnExit(e);
        }

        public static T GetService<T>() where T : class
        {
            return ((App)Current)._host!.Services.GetRequiredService<T>();
        }
    }
}
