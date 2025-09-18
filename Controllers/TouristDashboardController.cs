using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TourBuddy.Data;
using TourBuddy.Models;
using TourBuddy.ViewModels;

namespace TourBuddy.Controllers
{
    [Authorize(Roles = "Tourist")]
    public class TouristDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TouristDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var touristId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var allBookings = await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => b.TouristId == touristId)
                .OrderByDescending(b => b.TourDate)
                .ToListAsync();

            var upcomingBookings = allBookings.Where(b => b.TourDate >= DateTime.Today).ToList();
            var pastBookings = allBookings.Where(b => b.TourDate < DateTime.Today).ToList();

            ViewBag.UpcomingBookings = upcomingBookings;
            ViewBag.PastBookings = pastBookings.Take(3).ToList();

            ViewBag.TotalSpent = allBookings.Sum(b => b.TotalPrice);
            ViewBag.TripsTaken = pastBookings.Count;
            ViewBag.UpcomingTripsCount = upcomingBookings.Count;

            return View();
        }

        public async Task<IActionResult> MyBookings()
        {
            var touristId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var upcomingBookings = await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => b.TouristId == touristId && b.TourDate >= DateTime.Today)
                .OrderBy(b => b.TourDate)
                .ToListAsync();

            var pastBookings = await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => b.TouristId == touristId && b.TourDate < DateTime.Today)
                .OrderByDescending(b => b.TourDate)
                .ToListAsync();

            ViewBag.UpcomingBookings = upcomingBookings;
            ViewBag.PastBookings = pastBookings;

            return View();
        }

        public async Task<IActionResult> BookingDetails(int? id)
        {
            if (id == null) return NotFound();

            var touristId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == id && b.TouristId == touristId);

            if (booking == null) return Unauthorized();

            return View(booking);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBooking(BookingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var touristId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var tour = await _context.Tours.FindAsync(model.TourId);
                if (tour == null) return NotFound();

                var booking = new Booking
                {
                    TourId = tour.Id,
                    TouristId = touristId,
                    TourDate = model.TourDate,
                    NumberOfGuests = model.NumberOfGuests,
                    BookingDate = DateTime.UtcNow,
                    TotalPrice = tour.Price * model.NumberOfGuests,
                    Status = "Confirmed"
                };

                _context.Bookings.Add(booking);
                await _context.SaveChangesAsync();

                return RedirectToAction("BookingDetails", new { id = booking.Id });
            }

            var tourForRedirect = await _context.Tours.FindAsync(model.TourId);
            return RedirectToAction("Details", "Tours", new { id = tourForRedirect.Id });
        }

        [HttpGet]
        public async Task<IActionResult> LeaveReview(int bookingId)
        {
            var touristId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == bookingId && b.TouristId == touristId);

            if (booking == null || booking.TourDate >= DateTime.Today) return Unauthorized();

            var model = new ReviewViewModel
            {
                BookingId = booking.Id,
                TourName = booking.Tour.Name
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LeaveReview(ReviewViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var touristId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var booking = await _context.Bookings.FindAsync(model.BookingId);

                if (booking == null || booking.TouristId != touristId) return Unauthorized();

                var review = new Review
                {
                    TourId = booking.TourId,
                    TouristId = touristId,
                    Rating = model.Rating,
                    Comment = model.Comment,
                    ReviewDate = DateTime.UtcNow
                };

                _context.Reviews.Add(review);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(MyBookings));
            }

            var bookingForName = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == model.BookingId);
            model.TourName = bookingForName.Tour.Name;

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var touristId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(touristId);

            var model = new ProfileViewModel
            {
                FullName = user.FullName,
                Email = user.Email
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var touristId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _context.Users.FindAsync(touristId);
                user.FullName = model.FullName;

                _context.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}