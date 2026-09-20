using System.Windows;
using DailyPlanner.Services;
using DailyPlanner.ViewModels;

namespace DailyPlanner.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(DatabaseService dbService)
        {
            InitializeComponent();
            DataContext = new LoginViewModel(dbService);
        }
    }
}
