using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Locations
{
    public class RemoveLocationMenuAction: IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public RemoveLocationMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "Remove Location";
        public string Description => "Remove a location from the system";
        public char KeyChar => 'R';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("        REMOVE LOCATION            ");
                Console.WriteLine("===================================");

                // Show available locations
                Console.WriteLine("--- Available Locations ---");
                var locations = await _dataService.GetAllLocationsAsync();
                if (locations != null && locations.Count > 0)
                {
                    foreach (var location in locations)
                    {
                        Console.WriteLine($"ID: {location.Id} - {location.City}, {location.Country}");
                    }
                }
                else
                {
                    Console.WriteLine("No locations available to remove.");
                    return false;
                }

                Console.Write("Enter Location ID to remove: ");
                if (int.TryParse(Console.ReadLine(), out int locationId))
                {
                    var existingLocation = await _dataService.GetLocationByIdAsync(locationId);
                    if (existingLocation != null)
                    {
                        Console.WriteLine($"Location to be removed:");
                        Console.WriteLine($"ID: {existingLocation.Id}");
                        Console.WriteLine($"City: {existingLocation.City}");
                        Console.WriteLine($"Country: {existingLocation.Country}");
                        Console.WriteLine($"Latitude: {existingLocation.Latitude}");
                        Console.WriteLine($"Longitude: {existingLocation.Longitude}");

                        Console.Write("Are you sure you want to remove this location? (y/n): ");
                        var confirmation = Console.ReadLine()?.ToLower();
                        
                        if (confirmation == "y" || confirmation == "yes")
                        {
                            // Remove location from system
                            bool success = await _dataService.RemoveLocationAsync(locationId);
                            
                            if (success)
                            {
                                Console.WriteLine($"Location removed successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Failed to remove location. Please try again.");
                            }

                            return success;
                        }
                        else
                        {
                            Console.WriteLine("Location removal cancelled.");
                            return true;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Location not found with the given ID.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Location ID entered.");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in RemoveLocationMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
