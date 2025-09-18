using System.ComponentModel.DataAnnotations;

namespace TourBuddy.ViewModels
{
    public class BookingViewModel
    {
        [Required]
        public int TourId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime TourDate { get; set; }

        [Required]
        [Range(1, 20, ErrorMessage = "Number of guests must be between 1 and 20.")]
        public int NumberOfGuests { get; set; }
    }
}