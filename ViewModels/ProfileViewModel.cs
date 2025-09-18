using System.ComponentModel.DataAnnotations;

namespace TourBuddy.ViewModels
{
    public class ProfileViewModel
    {
        [Required]
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}