using System.ComponentModel.DataAnnotations;

namespace ProjectTrackr.Models
{
    public class Project
    {
        [Key]
        public Guid id { get; set; }
        [Required, MaxLength(200)]
        public string name { get; set; }
        public string description { get; set; }
        public DateTime createdAt { get; set; }

        [Required]
        public Guid ownerId { get; set; }
        public User owner { get; set; }

        public ICollection<TaskItem> tasks { get; set; }
        public ICollection<ActivityLog> activityLogs { get; set; }
    }
}
