using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Trips
{
    public class UpdateTripMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;
        private readonly ViewTripsMenuAction _viewTripsMenuAction;

        public UpdateTripMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService, ViewTripsMenuAction viewTripsMenuAction)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
            _viewTripsMenuAction = viewTripsMenuAction;
        }

        public string Title => "Update Trip";
        public string Description => "Update an existing trip in the system";
        public char KeyChar => 'U';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("         UPDATE TRIP               ");
                Console.WriteLine("===================================");

                await _viewTripsMenuAction.ExecuteAsync();
                
                var trips = await _dataService.GetAllTripsAsync();
                if (trips == null || trips.Count == 0)
                {
                    return true;
                }

                Console.Write("\nEnter the ID of the trip to update: ");
                if (int.TryParse(Console.ReadLine(), out int tripId))
                {
                    var existingTrip = await _dataService.GetTripByIdAsync(tripId);
                    if (existingTrip != null)
                    {
                        Console.WriteLine($"\nUpdating Trip ID: {existingTrip.Id}");
                        Console.WriteLine("===================================");

                        Console.WriteLine("\nCurrent Information:");
                        Console.WriteLine($"From: {existingTrip.FromLocation.City}, {existingTrip.FromLocation.Country}");
                        Console.WriteLine($"To: {existingTrip.ToLocation.City}, {existingTrip.ToLocation.Country}");
                        Console.WriteLine($"Status: {existingTrip.TripStatus}");

                        Console.WriteLine("\n--- Update From Location (Press Enter to keep current value) ---");
                        Console.Write($"City [{existingTrip.FromLocation.City}]: ");
                        string input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingTrip.FromLocation.City = input;
                        }

                        Console.Write($"Country [{existingTrip.FromLocation.Country}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingTrip.FromLocation.Country = input;
                        }

                        Console.Write($"Latitude [{existingTrip.FromLocation.Latitude}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && double.TryParse(input, out double fromLat))
                        {
                            existingTrip.FromLocation.Latitude = fromLat;
                        }

                        Console.Write($"Longitude [{existingTrip.FromLocation.Longitude}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && double.TryParse(input, out double fromLng))
                        {
                            existingTrip.FromLocation.Longitude = fromLng;
                        }

                        Console.WriteLine("\n--- Update To Location (Press Enter to keep current value) ---");
                        Console.Write($"City [{existingTrip.ToLocation.City}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingTrip.ToLocation.City = input;
                        }

                        Console.Write($"Country [{existingTrip.ToLocation.Country}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingTrip.ToLocation.Country = input;
                        }

                        Console.Write($"Latitude [{existingTrip.ToLocation.Latitude}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && double.TryParse(input, out double toLat))
                        {
                            existingTrip.ToLocation.Latitude = toLat;
                        }

                        Console.Write($"Longitude [{existingTrip.ToLocation.Longitude}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && double.TryParse(input, out double toLng))
                        {
                            existingTrip.ToLocation.Longitude = toLng;
                        }

                        Console.WriteLine("\n--- Update Trip Status ---");
                        Console.WriteLine("1. IN_PROGRESS");
                        Console.WriteLine("2. COMPLETED");
                        Console.WriteLine("3. CANCELLED");
                        Console.Write($"Current Status: {existingTrip.TripStatus}. Enter new status (1-3) or press Enter to keep current: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int statusChoice))
                        {
                            switch (statusChoice)
                            {
                                case 1:
                                    existingTrip.TripStatus = TripStatus.IN_PROGRESS;
                                    break;
                                case 2:
                                    existingTrip.TripStatus = TripStatus.COMPLETED;
                                    break;
                                case 3:
                                    existingTrip.TripStatus = TripStatus.CANCELLED;
                                    break;
                            }
                        }

                        bool success = await _dataService.UpdateTripAsync(existingTrip);
                        if (success)
                        {
                            Console.WriteLine($"\nTrip with ID {tripId} has been successfully updated.");
                        }
                        else
                        {
                            Console.WriteLine($"\nFailed to update trip with ID {tripId}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nTrip with ID {tripId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid trip ID entered.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in UpdateTripMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
