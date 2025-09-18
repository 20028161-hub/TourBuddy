using System.ComponentModel.DataAnnotations;

namespace TourBuddy.ViewModels
{
    public class ReviewViewModel
    {
        public int BookingId { get; set; }
        public string TourName { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string Comment { get; set; }
    }
}