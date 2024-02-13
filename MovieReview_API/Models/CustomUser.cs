using Microsoft.AspNetCore.Identity;

namespace MovieReview_API.Models
{
    public class CustomUser : IdentityUser
    {
        public ICollection<MovieReview>? Reviews { get; set; }
    }
}
