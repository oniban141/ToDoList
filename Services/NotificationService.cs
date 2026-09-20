using System;
using System.Timers;
using System.Windows;
using DailyPlanner.Models;

namespace DailyPlanner.Services
{
    public class NotificationService : IDisposable
    {
        private Timer _timer;
        private DatabaseService _dbService;
        
        public NotificationService(DatabaseService dbService)
        {
            _dbService = dbService;
            _timer = new Timer(60000); // Check every minute
            _timer.Elapsed += CheckReminders;
            _timer.Start();
        }
        
        private void CheckReminders(object sender, ElapsedEventArgs e)
        {
            var reminders = _dbService.GetActiveReminders();
            foreach (var r in reminders)
            {
                Application.Current.Dispatcher.Invoke(() =>
                    MessageBox.Show($"{r.TaskTitle}\n{r.Message}", "Напоминание!"));
                _dbService.DeactivateReminder(r.Id);
            }
        }
        
        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
                _timer.Elapsed -= CheckReminders;
                _timer.Dispose();
            }
        }
    }
}
