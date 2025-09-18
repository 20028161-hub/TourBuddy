using System;
using System.ComponentModel.DataAnnotations;

namespace TourBuddy.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime ReviewDate { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }
        public int TouristId { get; set; }
        public User Tourist { get; set; }
    }
}