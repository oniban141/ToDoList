using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DailyPlanner.Models;
using DailyPlanner.Services;
using DailyPlanner.ViewModels;

namespace DailyPlanner.Views
{
    public partial class MainWindow : Window
    {
        public static readonly List<string> StatusValues = new List<string> { "Todo", "InProgress", "Completed" };
        public static readonly List<string> PriorityValues = new List<string> { "Low", "Medium", "High" };
        
        public MainWindow(User user, DatabaseService dbService)
        {
            InitializeComponent();
            DataContext = new MainViewModel(user, dbService);
        }
        
        private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle calendar date selection
        }
    }
}
