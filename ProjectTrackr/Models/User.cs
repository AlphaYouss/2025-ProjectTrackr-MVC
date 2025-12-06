using System.ComponentModel.DataAnnotations;

namespace ProjectTrackr.Models
{
    public class User
    {
        [Key]
        public Guid id { get; set; }
        [Required, MaxLength(20)]
        public string userName { get; set; }
        [Required, MaxLength(200)]
        public string email { get; set; }
        [Required]
        public string passwordHash { get; set; }
        public DateTime createdAt { get; set; }

        public ICollection<Project> projects { get; set; }
        public ICollection<TaskItem> tasks { get; set; }
        public ICollection<Comment> comments { get; set; }
        public ICollection<ActivityLog> activityLogs { get; set; }
    }
}
