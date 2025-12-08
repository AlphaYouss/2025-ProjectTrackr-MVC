using System.ComponentModel.DataAnnotations;

namespace ProjectTrackr.Models
{
    public class ActivityLog
    {
        [Key]
        public Guid id { get; set; }
        [Required]
        public string action { get; set; }
        [Required]
        [DataType(DataType.DateTime)]
        public DateTime createdAt { get; set; }
        [Required]
        public Guid userId { get; set; }
        public User user { get; set; }

        public Guid? projectId { get; set; }
        public Project project { get; set; }

        public Guid? taskId { get; set; }
        public TaskItem task { get; set; }
    }
}