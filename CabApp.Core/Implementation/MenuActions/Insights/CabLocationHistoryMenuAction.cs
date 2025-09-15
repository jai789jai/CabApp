using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Insights
{
    public class CabLocationHistoryMenuAction: IMenuAction
    {
        private readonly IDataService _dataService;
        private readonly IAppLogger _appLogger;

        public CabLocationHistoryMenuAction(IDataService dataService, IAppLogger appLogger)
        {
            _dataService = dataService;
            _appLogger = appLogger;
        }

        public string Title => "Cab Location History";
        public string Description => "Show a cab's location history from completed trips";
        public char KeyChar => '2';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("=== Cab Location History ===");
                Console.WriteLine();

                // Get all cabs to display for selection
                var cabs = await _dataService.GetAllCabsAsync();
                if (cabs.Count == 0)
                {
                    Console.WriteLine("No cabs found in the system.");
                    return false;
                }

                // Display available cabs
                Console.WriteLine("Available Cabs:");
                Console.WriteLine("===============");
                foreach (var cab in cabs)
                {
                    Console.WriteLine($"Cab ID: {cab.Id} (Driver ID: {cab.DriverId}, Car ID: {cab.CarId})");
                }
                Console.WriteLine();

                // Get cab selection from user
                Console.Write("Enter Cab ID to view location history: ");
                if (!int.TryParse(Console.ReadLine(), out int selectedCabId))
                {
                    Console.WriteLine("Invalid cab ID format.");
                    return false;
                }

                var selectedCab = cabs.FirstOrDefault(c => c.Id == selectedCabId);
                if (selectedCab == null)
                {
                    Console.WriteLine($"Cab with ID {selectedCabId} not found.");
                    return false;
                }

                Console.WriteLine();
                Console.WriteLine($"Location History for Cab ID: {selectedCabId}");
                Console.WriteLine("==========================================");

                // Get all completed trips for this cab
                var allTrips = await _dataService.GetAllTripsAsync();
                var completedTrips = allTrips.Where(t => 
                    t.AssignedCabId == selectedCabId && 
                    t.TripStatus == TripStatus.COMPLETED &&
                    t.StartTime.HasValue && 
                    t.EndTime.HasValue)
                    .OrderBy(t => t.StartTime)
                    .ToList();

                if (completedTrips.Count == 0)
                {
                    Console.WriteLine("No completed trips found for this cab.");
                    return false;
                }

                // Get all locations for reference
                var allLocations = await _dataService.GetAllLocationsAsync();
                var locationDict = allLocations.ToDictionary(l => l.Id, l => l);

                Console.WriteLine($"Total Completed Trips: {completedTrips.Count}");
                Console.WriteLine();

                // Display location history
                Console.WriteLine($"{"Trip ID",-8} {"From Location",-25} {"To Location",-25} {"Start Time",-20} {"End Time",-20} {"Duration",-12}");
                Console.WriteLine(new string('-', 120));

                foreach (var trip in completedTrips)
                {
                    var fromLocation = locationDict.ContainsKey(trip.FromLocation.Id) 
                        ? $"{locationDict[trip.FromLocation.Id].City}, {locationDict[trip.FromLocation.Id].Country}"
                        : $"Unknown (ID: {trip.FromLocation.Id})";

                    var toLocation = locationDict.ContainsKey(trip.ToLocation.Id) 
                        ? $"{locationDict[trip.ToLocation.Id].City}, {locationDict[trip.ToLocation.Id].Country}"
                        : $"Unknown (ID: {trip.ToLocation.Id})";

                    var duration = trip.EndTime.Value - trip.StartTime.Value;
                    var durationStr = FormatTimeSpan(duration);

                    Console.WriteLine($"{trip.Id,-8} {fromLocation,-25} {toLocation,-25} " +
                                    $"{trip.StartTime.Value:yyyy-MM-dd HH:mm:ss,-20} " +
                                    $"{trip.EndTime.Value:yyyy-MM-dd HH:mm:ss,-20} {durationStr,-12}");
                }

                Console.WriteLine();

                // Generate location statistics
                Console.WriteLine("Location Statistics:");
                Console.WriteLine("===================");

                // Count visits to each location
                var locationVisits = new Dictionary<int, int>();
                var locationDepartures = new Dictionary<int, int>();

                foreach (var trip in completedTrips)
                {
                    // Count arrivals (to location)
                    if (locationVisits.ContainsKey(trip.ToLocation.Id))
                        locationVisits[trip.ToLocation.Id]++;
                    else
                        locationVisits[trip.ToLocation.Id] = 1;

                    // Count departures (from location)
                    if (locationDepartures.ContainsKey(trip.FromLocation.Id))
                        locationDepartures[trip.FromLocation.Id]++;
                    else
                        locationDepartures[trip.FromLocation.Id] = 1;
                }

                // Most visited locations
                var mostVisited = locationVisits.OrderByDescending(x => x.Value).Take(5);
                Console.WriteLine("Most Visited Locations (Arrivals):");
                foreach (var visit in mostVisited)
                {
                    var location = locationDict.ContainsKey(visit.Key) 
                        ? $"{locationDict[visit.Key].City}, {locationDict[visit.Key].Country}"
                        : $"Unknown (ID: {visit.Key})";
                    Console.WriteLine($"  {location}: {visit.Value} times");
                }

                Console.WriteLine();

                // Most departed locations
                var mostDeparted = locationDepartures.OrderByDescending(x => x.Value).Take(5);
                Console.WriteLine("Most Departed Locations:");
                foreach (var departure in mostDeparted)
                {
                    var location = locationDict.ContainsKey(departure.Key) 
                        ? $"{locationDict[departure.Key].City}, {locationDict[departure.Key].Country}"
                        : $"Unknown (ID: {departure.Key})";
                    Console.WriteLine($"  {location}: {departure.Value} times");
                }

                Console.WriteLine();

                // Trip distance analysis (if we had distance data)
                var totalTrips = completedTrips.Count;
                var uniqueFromLocations = completedTrips.Select(t => t.FromLocation.Id).Distinct().Count();
                var uniqueToLocations = completedTrips.Select(t => t.ToLocation.Id).Distinct().Count();

                Console.WriteLine("Trip Summary:");
                Console.WriteLine($"  Total Trips: {totalTrips}");
                Console.WriteLine($"  Unique From Locations: {uniqueFromLocations}");
                Console.WriteLine($"  Unique To Locations: {uniqueToLocations}");
                var avgTripsPerDay = totalTrips > 0 ? (double)totalTrips / Math.Max(1, (DateTime.Now - completedTrips.First().StartTime.Value).TotalDays) : 0;
                Console.WriteLine($"  Average Trips Per Day: {avgTripsPerDay:F1}");

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in CabLocationHistoryMenuAction", ex);
                Console.WriteLine("An error occurred while retrieving cab location history.");
                return false;
            }
        }

        private string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalHours >= 1)
            {
                return $"{timeSpan.Hours}h {timeSpan.Minutes}m";
            }
            else
            {
                return $"{timeSpan.Minutes}m {timeSpan.Seconds}s";
            }
        }
    }
}
