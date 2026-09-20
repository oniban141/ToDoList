using System;
using System.Collections.Generic;

namespace DailyPlanner.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation property
        public virtual User User { get; set; }
        public virtual ICollection<TaskTag> TaskTags { get; set; }
    }
    
    public class TaskTag
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int TagId { get; set; }
        
        // Navigation properties
        public virtual Task Task { get; set; }
        public virtual Tag Tag { get; set; }
    }
}
