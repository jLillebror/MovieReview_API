using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MovieReview_API.Models
{
    public class MovieReview
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Movie")]
        public int MovieId { get; set; }

        [ForeignKey("CustomUser")]
        public string? UserId { get; set; }

        public string Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        
        [ValidateNever]
        public CustomUser? User { get; set; } // Navigation property to CustomUser entity

        [ValidateNever]
        public Movie Movie { get; set; } // Navigation property to Movie entity

    }
}
