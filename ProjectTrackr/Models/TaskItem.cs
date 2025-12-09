using System.ComponentModel.DataAnnotations;

namespace ProjectTrackr.Models
{
    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }

    public class TaskItem
    {
        [Key]
        public Guid id { get; set; }
        [Required, MaxLength(200)]
        public string title { get; set; }
        public string description { get; set; }
        public TaskStatus status { get; set; } = TaskStatus.ToDo;
        public DateTime createdAt { get; set; } = DateTime.Now;

        [Required]
        public Guid projectId { get; set; }
    }
}