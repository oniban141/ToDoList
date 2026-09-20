using System;

namespace DailyPlanner.Models
{
    public class Task
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string Priority { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public virtual User User { get; set; }
        public string TaskTitle => Title;
    }
}
