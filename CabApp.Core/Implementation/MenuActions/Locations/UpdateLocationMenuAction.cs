using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Locations
{
    public class UpdateLocationMenuAction: IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public UpdateLocationMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "Update Location";
        public string Description => "Update an existing location in the system";
        public char KeyChar => 'U';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("        UPDATE LOCATION            ");
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
                    Console.WriteLine("No locations available. Please add locations first.");
                    return false;
                }

                Console.Write("Enter Location ID to update: ");
                if (int.TryParse(Console.ReadLine(), out int locationId))
                {
                    var existingLocation = await _dataService.GetLocationByIdAsync(locationId);
                    if (existingLocation != null)
                    {
                        Console.WriteLine($"Current details:");
                        Console.WriteLine($"City: {existingLocation.City}");
                        Console.WriteLine($"Country: {existingLocation.Country}");
                        Console.WriteLine($"Latitude: {existingLocation.Latitude}");
                        Console.WriteLine($"Longitude: {existingLocation.Longitude}");

                        Console.WriteLine("Enter new details (press Enter to keep current value):");

                        Console.Write($"New City [{existingLocation.City}]: ");
                        var newCity = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newCity))
                        {
                            existingLocation.City = newCity;
                        }

                        Console.Write($"New Country [{existingLocation.Country}]: ");
                        var newCountry = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newCountry))
                        {
                            existingLocation.Country = newCountry;
                        }

                        Console.Write($"New Latitude [{existingLocation.Latitude}]: ");
                        var latitudeInput = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(latitudeInput) && double.TryParse(latitudeInput, out double newLatitude))
                        {
                            existingLocation.Latitude = newLatitude;
                        }

                        Console.Write($"New Longitude [{existingLocation.Longitude}]: ");
                        var longitudeInput = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(longitudeInput) && double.TryParse(longitudeInput, out double newLongitude))
                        {
                            existingLocation.Longitude = newLongitude;
                        }

                        // Update location in system
                        bool success = await _dataService.UpdateLocationAsync(existingLocation);
                        
                        if (success)
                        {
                            Console.WriteLine($"Location updated successfully!");
                        }
                        else
                        {
                            Console.WriteLine("Failed to update location. Please try again.");
                        }

                        return success;
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
                _appLogger.LogError("Exception occurred in UpdateLocationMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
