using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class CompleteTripMenuAction : IMenuAction
    {
        private readonly IDataService _dataService;
        private readonly IHelper _helper;
        private readonly IAppLogger _appLogger;

        public CompleteTripMenuAction(IDataService dataService, IAppLogger appLogger, IHelper helper)
        {
            _dataService = dataService;
            _appLogger = appLogger;
            _helper = helper;
        }

        public string Title => "Complete Trip";
        public string Description => "Mark a trip as completed";
        public char KeyChar => 'T';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.WriteLine("=== Complete Trip ===");

                // Display active trips
                var allTrips = await _dataService.GetAllTripsAsync();
                var activeTrips = allTrips.Where(t => t.TripStatus == TripStatus.IN_PROGRESS && t.AssignedCabId.HasValue).ToList();

                if (!activeTrips.Any())
                {
                    Console.WriteLine("No active trips found.");
                    return false;
                }

                Console.WriteLine("Active Trips:");
                foreach (var trip in activeTrips)
                {
                    var cab = await _dataService.GetCabByIdAsync(trip.AssignedCabId.Value);
                    Console.WriteLine($"Trip ID: {trip.Id}, Cab ID: {trip.AssignedCabId}, " +
                                    $"From: {trip.FromLocation.City}, To: {trip.ToLocation.City}, " +
                                    $"Start Time: {trip.StartTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A"}");
                }

                // Get trip ID from user
                Console.Write("Enter Trip ID to complete: ");
                if (!int.TryParse(Console.ReadLine(), out int tripId))
                {
                    Console.WriteLine("Invalid trip ID.");
                    return false;
                }

                var selectedTrip = await _dataService.GetTripByIdAsync(tripId);
                if (selectedTrip == null)
                {
                    Console.WriteLine("Trip not found.");
                    return false;
                }

                if (selectedTrip.TripStatus != TripStatus.IN_PROGRESS)
                {
                    Console.WriteLine($"Trip is not active. Current status: {selectedTrip.TripStatus}");
                    return false;
                }

                if (!selectedTrip.AssignedCabId.HasValue)
                {
                    Console.WriteLine("Trip is not assigned to any cab.");
                    return false;
                }

                // Complete the trip
                var success = await _helper.CompleteTripAsync(tripId);
                if (success)
                {
                    var cab = await _dataService.GetCabByIdAsync(selectedTrip.AssignedCabId.Value);
                    Console.WriteLine($"Trip completed successfully!");
                    Console.WriteLine($"Trip ID: {tripId}");
                    Console.WriteLine($"Cab ID: {selectedTrip.AssignedCabId}");
                    Console.WriteLine($"From: {selectedTrip.FromLocation.City}");
                    Console.WriteLine($"To: {selectedTrip.ToLocation.City}");
                    Console.WriteLine($"Start Time: {selectedTrip.StartTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"End Time: {selectedTrip.EndTime:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"Cab {selectedTrip.AssignedCabId} is now available at {selectedTrip.ToLocation.City}");
                }
                else
                {
                    Console.WriteLine("Failed to complete trip.");
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Error in CompleteTripMenuAction", ex);
                Console.WriteLine("An error occurred while completing trip.");
                return false;
            }
            return true;
        }
    }
}
