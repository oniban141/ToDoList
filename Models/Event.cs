using System;

namespace DailyPlanner.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Location { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public virtual User User { get; set; }
    }
}
