using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProjectTrackr.Models.ViewModels
{
    public class TaskItemViewModel
    {
        public enum TaskStatus
        {
            ToDo,
            InProgress,
            Done
        }

        [AllowNull]
        public Guid id { get; set; }
        [Required]
        public string title { get; set; }
        [StringLength(2000)]
        public string description { get; set; }
        [Required]
        public TaskStatus status { get; set; } = TaskStatus.ToDo;
        public DateTime createdAt { get; set; } = DateTime.Now;
        public Guid projectId { get; set; }
    }
}