using System.ComponentModel.DataAnnotations;

namespace ProjectTrackr.Models
{
    public class Comment
    {
        [Key]
        public Guid id { get; set; }
        [Required]
        public string content { get; set; }
        public DateTime createdAt { get; set; }

        [Required]
        public Guid taskId { get; set; }
        public TaskItem task { get; set; }
        [Required]
        public Guid authorId { get; set; }
        public User author { get; set; }
    }
}
