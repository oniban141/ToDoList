using System;
using System.Windows;
using DailyPlanner.Services;
using DailyPlanner.Views;

namespace DailyPlanner
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            try
            {
                // Get connection string from App.config
                string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["DailyPlannerDB"].ConnectionString;
                
                // Create database service
                var dbService = new DatabaseService(connectionString);
                
                // Show login window
                var loginWindow = new LoginWindow(dbService);
                loginWindow.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при запуске приложения: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                Current.Shutdown();
            }
        }
        
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
