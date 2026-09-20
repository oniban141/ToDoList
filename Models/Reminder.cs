using System;

namespace DailyPlanner.Models
{
    public class Reminder
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public DateTime ReminderTime { get; set; }
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public virtual Task Task { get; set; }
        public virtual User User { get; set; }
        public string TaskTitle => Task?.Title ?? "Unknown Task";
    }
}
