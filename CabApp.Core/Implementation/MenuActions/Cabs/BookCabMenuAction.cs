using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class BookCabMenuAction : IMenuAction
    {
        private readonly IDataService _dataService;
        private readonly IAppLogger _appLogger;

        public BookCabMenuAction(IDataService dataService, IAppLogger appLogger)
        {
            _dataService = dataService;
            _appLogger = appLogger;
        }

        public string Title => "Book Cab";
        public string Description => "Book a cab for a trip";
        public char KeyChar => 'B';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.WriteLine("=== Book Cab ===");

                // Display available locations
                var locations = await _dataService.GetAllLocationsAsync();
                if (!locations.Any())
                {
                    Console.WriteLine("No locations found. Please add locations first through the Location Management menu.");
                    return false;
                }

                Console.WriteLine("Available Locations:");
                foreach (var location in locations)
                {
                    Console.WriteLine($"ID: {location.Id}, City: {location.City}, Country: {location.Country}");
                }

                // Get from location
                Console.Write("Enter From Location ID: ");
                if (!int.TryParse(Console.ReadLine(), out int fromLocationId))
                {
                    Console.WriteLine("Invalid from location ID.");
                    return false;
                }

                var fromLocation = await _dataService.GetLocationByIdAsync(fromLocationId);
                if (fromLocation == null)
                {
                    Console.WriteLine("From location not found.");
                    return false;
                }

                // Get to location
                Console.Write("Enter To Location ID: ");
                if (!int.TryParse(Console.ReadLine(), out int toLocationId))
                {
                    Console.WriteLine("Invalid to location ID.");
                    return false;
                }

                var toLocation = await _dataService.GetLocationByIdAsync(toLocationId);
                if (toLocation == null)
                {
                    Console.WriteLine("To location not found.");
                    return false;
                }

                if (fromLocationId == toLocationId)
                {
                    Console.WriteLine("From and to locations cannot be the same.");
                    return false;
                }

                // Check for available cabs at the from location
                var availableCabs = await _dataService.GetAvailableCabsAtLocationAsync(fromLocationId);
                if (!availableCabs.Any())
                {
                    Console.WriteLine($"No available cabs at {fromLocation.City}. Please try a different location.");
                    return false;
                }

                Console.WriteLine($"Found {availableCabs.Count} available cab(s) at {fromLocation.City}:");

                // Create a new trip
                var trip = new TripDetail
                {
                    FromLocation = fromLocation,
                    ToLocation = toLocation,
                    TripStatus = TripStatus.IN_PROGRESS
                };

                // Add the trip first
                var tripAdded = await _dataService.AddTripAsync(trip);
                if (!tripAdded)
                {
                    Console.WriteLine("Failed to create trip.");
                    return false;
                }

                // Book a cab for the trip
                var assignedCab = await _dataService.BookCabForTripAsync(trip.Id, fromLocationId);
                if (assignedCab != null)
                {
                    Console.WriteLine($"✅ Trip booked successfully!");
                    Console.WriteLine($"Trip ID: {trip.Id}");
                    Console.WriteLine($"Assigned Cab ID: {assignedCab.Id}");
                    Console.WriteLine($"From: {fromLocation.City}");
                    Console.WriteLine($"To: {toLocation.City}");
                    Console.WriteLine($"Booking Time: {trip.BookingTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"Start Time: {trip.StartTime:yyyy-MM-dd HH:mm:ss}");
                }
                else
                {
                    Console.WriteLine("Failed to book cab for the trip.");
                    // Clean up the trip if cab booking failed
                    await _dataService.RemoveTripAsync(trip.Id);
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Error in BookCabMenuAction", ex);
                Console.WriteLine("An error occurred while booking cab.");
                return false;
            }
            return true;
        }
    }
}
