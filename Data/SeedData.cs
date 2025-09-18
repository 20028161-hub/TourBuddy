using Microsoft.EntityFrameworkCore;
using TourBuddy.Models;
using TourBuddy.Services;

namespace TourBuddy.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                var passwordService = serviceProvider.GetRequiredService<IPasswordService>();

                if (await context.Users.AnyAsync())
                {
                    return;
                }

                passwordService.CreatePasswordHash("Password123!", out byte[] agencyHash, out byte[] agencySalt);
                var agencyUser = new User
                {
                    FullName = "Global Treks Inc.",
                    Email = "agency@tourbuddy.com",
                    PasswordHash = agencyHash,
                    PasswordSalt = agencySalt,
                    Role = "Agency"
                };

                passwordService.CreatePasswordHash("Password123!", out byte[] touristHash, out byte[] touristSalt);
                var touristUser = new User
                {
                    FullName = "John Traveler",
                    Email = "tourist@tourbuddy.com",
                    PasswordHash = touristHash,
                    PasswordSalt = touristSalt,
                    Role = "Tourist"
                };

                await context.Users.AddRangeAsync(agencyUser, touristUser);
                await context.SaveChangesAsync();

                var tours = new List<Tour>
                {
                    new Tour { Name = "Serene Temples of Kyoto",
                        Description = "Experience the tranquility and ancient culture of Japan's imperial capital.", Price = 1250, DurationInDays = 5, MaxGroupSize = 10,
                        ImageUrls="https://www.2aussietravellers.com/wp-content/uploads/2020/09/Golden-Pavilion-2.jpg,https://www.datocms-assets.com/101439/1705405476-golden-pavilion.webp", AgencyId = agencyUser.Id },
                    new Tour { Name = "Majestic Swiss Alps Hike", Description = "A breathtaking journey through the world's most iconic mountains.", Price = 950, DurationInDays = 7, MaxGroupSize = 12,
                        ImageUrls="https://rawtravel.com/nitropack_static/fdxuGitHONbTaIgMEUKDhflvoQKespQj/assets/images/optimized/rev-3720e6d/rawtravel.com/wp-content/uploads/2024/10/Haute-Route-Switzerland0000-960x609.jpg", AgencyId = agencyUser.Id },
                    new Tour { Name = "Santorini Sunset Escape", Description = "Witness the world-famous sunsets from the beautiful cliffs of Oia.", Price = 800, DurationInDays = 4, MaxGroupSize = 15, 
                        ImageUrls="https://santorinihelitours.com/wp-content/uploads/2024/03/santorini.jpg,https://luxuryescapes.com/inspiration/wp-content/uploads/2023/07/shutterstock_641784133-1024x644.jpg", AgencyId = agencyUser.Id },
                    new Tour { Name = "Ancient Wonders of Rome", Description = "Step back in time and explore the historic heart of the Roman Empire.", Price = 1100, DurationInDays = 6, MaxGroupSize = 20,
                        ImageUrls="https://www.romecabs.com/upload/CONF99/20210621/Seven_Wonders_Ancient_Rome_Tour_Colosseum_Visit_RomeCabs.JPG,https://www.romecabs.com/upload/CONF99/20210621/Rome_Limo_Tours_from_Civitavecchia_Shore_Excursions_Pantheon_RomeCabs.jpg", AgencyId = agencyUser.Id },
                    new Tour { Name = "Bali Spiritual Retreat", Description = "Find your inner peace among the lush rice paddies and temples of Ubud.", Price = 750, DurationInDays = 8, MaxGroupSize = 10,
                        ImageUrls="https://i0.wp.com/hideoutbali.com/wp-content/uploads/2025/04/hideout-bali-spiritual-retreat.png?fit=1920%2C1080&ssl=1,https://www.luxewellnessclub.com/wp-content/uploads/2019/01/Five-Elements-Wellness-Resort-Bali-spa.jpg", AgencyId = agencyUser.Id },
                    new Tour { Name = "Inca Trail to Machu Picchu", Description = "Hike the legendary path to the lost city of the Incas in the Andes mountains.", Price = 2100, DurationInDays = 4, MaxGroupSize = 16, 
                        ImageUrls="https://cdn.kimkim.com/files/a/content_articles/featured_photos/fe7918e9abc72ff850344ff33792a51c393ceae4/big-10099011f3f84e7bd08a0148d26b13f9.jpg,https://media.tacdn.com/media/attractions-splice-spp-674x446/13/d1/81/f1.jpg", AgencyId = agencyUser.Id },
                    new Tour { Name = "New York City Explorer", Description = "Discover the vibrant energy of the city that never sleeps, from Times Square to Central Park.", Price = 1300, DurationInDays = 5, MaxGroupSize = 25,
                        ImageUrls="https://upload.wikimedia.org/wikipedia/commons/thumb/4/47/New_york_times_square-terabass.jpg/1200px-New_york_times_square-terabass.jpg", AgencyId = agencyUser.Id },
                    new Tour { Name = "Coastal California Road Trip", Description = "Drive along the stunning Pacific Coast Highway from San Francisco to Los Angeles.", Price = 1500, DurationInDays = 7,
                        MaxGroupSize = 8, ImageUrls="https://www.visittheusa.com/sites/default/files/styles/16_9_770x433/public/images/hero_media_image/2016-10/Getty_552482735_TripStop_PCHRoad_Cambria_FinalCrop_10_14.jpg?h=4d4f3e60&itok=-twd6SNL,https://i0.wp.com/localloveandwanderlust.com/wp-content/uploads/2023/02/big-sur-23-1440x960.webp?resize=1240%2C827&ssl=1", AgencyId = agencyUser.Id },
                    new Tour { Name = "African Safari Adventure", Description = "Track the 'Big Five' in the vast plains of the Serengeti and witness the Great Migration.", Price = 3500, 
                        DurationInDays = 10, MaxGroupSize = 6, ImageUrls="https://www.nkuringosafaris.com/wp-content/uploads/2024/07/dream_african_safari_couple__elephants.jpg", AgencyId = agencyUser.Id },
                    new Tour { Name = "Northern Lights in Iceland", Description = "Chase the aurora borealis and explore Iceland's glaciers, waterfalls, and geothermal lagoons.", Price = 2800, DurationInDays = 6, MaxGroupSize = 14, ImageUrls="https://www.travelandleisure.com/thmb/QK4CuWpFdm2dR3NDznbBQtsAoN0=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/northern-lights-kirkjufell-mountain-snaefellsnes-iceland-ICELANDLIGHTS1218-824f48715748425f828f05aa2a28dfe0.jpg", AgencyId = agencyUser.Id }
                };

                await context.Tours.AddRangeAsync(tours);
                await context.SaveChangesAsync();

                var bookings = new List<Booking>
                {
                    new Booking { TourId = tours[0].Id, TouristId = touristUser.Id, BookingDate = DateTime.UtcNow.AddDays(-60), TourDate = DateTime.UtcNow.AddDays(-30), NumberOfGuests = 2, TotalPrice = 2500, Status = "Completed" },
                    new Booking { TourId = tours[3].Id, TouristId = touristUser.Id, BookingDate = DateTime.UtcNow.AddDays(-40), TourDate = DateTime.UtcNow.AddDays(-15), NumberOfGuests = 1, TotalPrice = 1100, Status = "Completed" },

                    new Booking { TourId = tours[1].Id, TouristId = touristUser.Id, BookingDate = DateTime.UtcNow.AddDays(-10), TourDate = DateTime.UtcNow.AddDays(20), NumberOfGuests = 1, TotalPrice = 950, Status = "Confirmed" },
                    new Booking { TourId = tours[8].Id, TouristId = touristUser.Id, BookingDate = DateTime.UtcNow.AddDays(-5), TourDate = DateTime.UtcNow.AddDays(45), NumberOfGuests = 2, TotalPrice = 7000, Status = "Confirmed" }
                };

                await context.Bookings.AddRangeAsync(bookings);
                await context.SaveChangesAsync();
            }
        }
    }
}