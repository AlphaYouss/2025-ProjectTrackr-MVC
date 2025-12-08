using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ProjectTrackr.Models.ViewModels
{
    public class ProjectViewModel
    {
        [AllowNull]
        public Guid id { get; set; }

        [Required]
        public string name { get; set; }

        [StringLength(2000)]
        public string? description { get; set; }
    }
}