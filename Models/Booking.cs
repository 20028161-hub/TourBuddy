using System;
using System.ComponentModel.DataAnnotations;

namespace TourBuddy.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime TourDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public int TourId { get; set; }
        public Tour Tour { get; set; }
        public int TouristId { get; set; }
        public User Tourist { get; set; }
    }
}