using System.ComponentModel.DataAnnotations;

namespace TourBuddy.ViewModels
{
    public class TourViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Range(0.01, 99999.99)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Duration (Days)")]
        public int DurationInDays { get; set; }

        [Required]
        [Range(1, 200)]
        [Display(Name = "Max Group Size")]
        public int MaxGroupSize { get; set; }

        [Required]
        [Display(Name = "Image URLs (comma-separated)")]
        public string ImageUrls { get; set; }
    }
}