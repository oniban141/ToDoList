using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using DailyPlanner.Models;
using DailyPlanner.Services;

namespace DailyPlanner.ViewModels
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _dbService;
        private string _username;
        private string _password;
        private string _errorMessage;
        
        public event PropertyChangedEventHandler PropertyChanged;
        
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }
        
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }
        
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }
        
        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        
        public LoginViewModel(DatabaseService dbService)
        {
            _dbService = dbService;
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
        }
        
        private void Login(object parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Пожалуйста, введите имя пользователя и пароль";
                    return;
                }
                
                // In a real app, you would hash the password and compare with stored hash
                var user = _dbService.GetUserByUsername(Username);
                if (user != null && user.PasswordHash == HashPassword(Password))
                {
                    ErrorMessage = string.Empty;
                    // Login successful - close this window and open main window
                    Application.Current.MainWindow = new MainWindow(user, _dbService);
                    Application.Current.MainWindow.Show();
                    
                    // Close login window
                    if (parameter is Window loginWindow)
                    {
                        loginWindow.Close();
                    }
                }
                else
                {
                    ErrorMessage = "Неправильное имя пользователя или пароль";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка при входе: " + ex.Message;
            }
        }
        
        private void Register(object parameter)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
                {
                    ErrorMessage = "Пожалуйста, введите имя пользователя и пароль";
                    return;
                }
                
                var existingUser = _dbService.GetUserByUsername(Username);
                if (existingUser != null)
                {
                    ErrorMessage = "Пользователь с таким именем уже существует";
                    return;
                }
                
                var newUser = new User
                {
                    Username = Username,
                    PasswordHash = HashPassword(Password),
                    Email = Username + "@example.com",
                    RoleId = 2, // Default to User role
                    GenderId = 1, // Default to Male
                    CreatedAt = DateTime.Now
                };
                
                int userId = _dbService.AddUser(newUser);
                if (userId > 0)
                {
                    ErrorMessage = string.Empty;
                    newUser.Id = userId;
                    
                    // Registration successful - close this window and open main window
                    Application.Current.MainWindow = new MainWindow(newUser, _dbService);
                    Application.Current.MainWindow.Show();
                    
                    // Close login window
                    if (parameter is Window loginWindow)
                    {
                        loginWindow.Close();
                    }
                }
                else
                {
                    ErrorMessage = "Не удалось зарегистрировать пользователя";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Ошибка при регистрации: " + ex.Message;
            }
        }
        
        private string HashPassword(string password)
        {
            // Simple hash for demo purposes - in production use proper hashing like BCrypt
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }
        
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
