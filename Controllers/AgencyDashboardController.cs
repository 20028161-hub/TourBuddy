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
    [Authorize(Roles = "Agency")]
    public class AgencyDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AgencyDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var agencyTours = _context.Tours.Where(t => t.AgencyId == agencyId);

            var allBookings = await _context.Bookings
                .Include(b => b.Tour)
                .Include(b => b.Tourist)
                .Where(b => agencyTours.Contains(b.Tour))
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            var reviews = await _context.Reviews
                .Where(r => agencyTours.Contains(r.Tour))
                .ToListAsync();

            ViewBag.RecentBookings = allBookings.Take(5).ToList();
            ViewBag.TotalRevenue = allBookings.Sum(b => b.TotalPrice);
            ViewBag.TotalBookings = allBookings.Count;
            ViewBag.ActiveToursCount = await agencyTours.CountAsync();
            ViewBag.AverageRating = reviews.Any() ? reviews.Average(r => r.Rating).ToString("F1") : "N/A";

            return View();
        }

        public async Task<IActionResult> ManageTours()
        {
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tours = await _context.Tours.Where(t => t.AgencyId == agencyId).ToListAsync();
            return View(tours);
        }

        [HttpGet]
        public IActionResult CreateTour() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTour(TourViewModel model)
        {
            if (ModelState.IsValid)
            {
                var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var tour = new Tour
                {
                    Name = model.Name,
                    Description = model.Description,
                    Price = model.Price,
                    DurationInDays = model.DurationInDays,
                    MaxGroupSize = model.MaxGroupSize,
                    ImageUrls = model.ImageUrls,
                    AgencyId = agencyId
                };
                _context.Tours.Add(tour);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageTours));
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditTour(int? id)
        {
            if (id == null) return NotFound();
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tour = await _context.Tours.FirstOrDefaultAsync(t => t.Id == id && t.AgencyId == agencyId);
            if (tour == null) return Unauthorized();
            var model = new TourViewModel
            {
                Id = tour.Id,
                Name = tour.Name,
                Description = tour.Description,
                Price = tour.Price,
                DurationInDays = tour.DurationInDays,
                MaxGroupSize = tour.MaxGroupSize,
                ImageUrls = tour.ImageUrls
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTour(int id, TourViewModel model)
        {
            if (id != model.Id) return BadRequest();
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tourToUpdate = await _context.Tours.FirstOrDefaultAsync(t => t.Id == id && t.AgencyId == agencyId);
            if (tourToUpdate == null) return Unauthorized();

            if (ModelState.IsValid)
            {
                tourToUpdate.Name = model.Name; tourToUpdate.Description = model.Description;
                tourToUpdate.Price = model.Price; tourToUpdate.DurationInDays = model.DurationInDays;
                tourToUpdate.MaxGroupSize = model.MaxGroupSize; tourToUpdate.ImageUrls = model.ImageUrls;
                _context.Update(tourToUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(ManageTours));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTour(int id)
        {
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var tour = await _context.Tours
                .Include(t => t.Bookings)
                .Include(t => t.Reviews)
                .FirstOrDefaultAsync(t => t.Id == id && t.AgencyId == agencyId);

            if (tour == null)
            {
                return Unauthorized();
            }

            _context.Reviews.RemoveRange(tour.Reviews);
            _context.Bookings.RemoveRange(tour.Bookings);
            _context.Tours.Remove(tour);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(ManageTours));
        }

        public async Task<IActionResult> ViewBookings()
        {
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var agencyTours = _context.Tours.Where(t => t.AgencyId == agencyId);
            var bookings = await _context.Bookings
                .Include(b => b.Tour).Include(b => b.Tourist)
                .Where(b => agencyTours.Contains(b.Tour))
                .OrderByDescending(b => b.BookingDate).ToListAsync();
            return View(bookings);
        }

        public async Task<IActionResult> ManageReviews()
        {
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var agencyTours = _context.Tours.Where(t => t.AgencyId == agencyId);
            var reviews = await _context.Reviews
                .Include(r => r.Tour).Include(r => r.Tourist)
                .Where(r => agencyTours.Contains(r.Tour))
                .OrderByDescending(r => r.ReviewDate).ToListAsync();
            return View(reviews);
        }

        public async Task<IActionResult> Reports()
        {
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var agencyTours = _context.Tours.Where(t => t.AgencyId == agencyId);

            var bookings = await _context.Bookings
                .Include(b => b.Tour)
                .Where(b => agencyTours.Contains(b.Tour))
                .ToListAsync();

            ViewBag.TotalRevenue = bookings.Sum(b => b.TotalPrice);
            ViewBag.TotalBookings = bookings.Count;
            ViewBag.BookingsByTour = bookings
                .GroupBy(b => b.Tour.Name)
                .Select(g => new { TourName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToList();

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var user = await _context.Users.FindAsync(agencyId);
            var model = new ProfileViewModel { FullName = user.FullName, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateBookingStatus(int bookingId, string status)
        {
            var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var booking = await _context.Bookings
                .Include(b => b.Tour)
                .FirstOrDefaultAsync(b => b.Id == bookingId);

            if (booking == null)
            {
                return NotFound();
            }

            if (booking.Tour.AgencyId != agencyId)
            {
                return Unauthorized();
            }

            if (status == "Pending" || status == "Confirmed" || status == "Cancelled")
            {
                booking.Status = status;
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ViewBookings));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var agencyId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                var user = await _context.Users.FindAsync(agencyId);
                user.FullName = model.FullName;
                _context.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }
    }
}