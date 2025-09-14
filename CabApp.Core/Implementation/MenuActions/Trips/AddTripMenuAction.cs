using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Trips
{
    public class AddTripMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public AddTripMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "Add Trip";
        public string Description => "Add a new trip to the system";
        public char KeyChar => 'A';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("          ADD NEW TRIP             ");
                Console.WriteLine("===================================");

                var trip = new TripDetail();

                Console.WriteLine("\n--- From Location ---");
                Console.Write("City: ");
                trip.FromLocation.City = Console.ReadLine() ?? string.Empty;

                Console.Write("Country: ");
                trip.FromLocation.Country = Console.ReadLine() ?? string.Empty;

                Console.Write("Latitude: ");
                if (double.TryParse(Console.ReadLine(), out double fromLat))
                {
                    trip.FromLocation.Latitude = fromLat;
                }

                Console.Write("Longitude: ");
                if (double.TryParse(Console.ReadLine(), out double fromLng))
                {
                    trip.FromLocation.Longitude = fromLng;
                }

                Console.WriteLine("\n--- To Location ---");
                Console.Write("City: ");
                trip.ToLocation.City = Console.ReadLine() ?? string.Empty;

                Console.Write("Country: ");
                trip.ToLocation.Country = Console.ReadLine() ?? string.Empty;

                Console.Write("Latitude: ");
                if (double.TryParse(Console.ReadLine(), out double toLat))
                {
                    trip.ToLocation.Latitude = toLat;
                }

                Console.Write("Longitude: ");
                if (double.TryParse(Console.ReadLine(), out double toLng))
                {
                    trip.ToLocation.Longitude = toLng;
                }

                Console.WriteLine("\n--- Trip Status ---");
                Console.WriteLine("1. IN_PROGRESS");
                Console.WriteLine("2. COMPLETED");
                Console.WriteLine("3. CANCELLED");
                Console.Write("Select status (1-3): ");
                if (int.TryParse(Console.ReadLine(), out int statusChoice))
                {
                    switch (statusChoice)
                    {
                        case 1:
                            trip.TripStatus = TripStatus.IN_PROGRESS;
                            break;
                        case 2:
                            trip.TripStatus = TripStatus.COMPLETED;
                            break;
                        case 3:
                            trip.TripStatus = TripStatus.CANCELLED;
                            break;
                        default:
                            trip.TripStatus = TripStatus.IN_PROGRESS;
                            break;
                    }
                }

                bool success = await _dataService.AddTripAsync(trip);
                
                if (success)
                {
                    Console.WriteLine($"\nTrip added successfully with ID: {trip.Id}");
                }
                else
                {
                    Console.WriteLine("\nFailed to add trip. Please try again.");
                }

                return success;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in AddTripMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
