using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Locations
{
    public class AddLocationMenuAction: IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public AddLocationMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "Add Location";
        public string Description => "Add a new location to the system";
        public char KeyChar => 'A';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("         ADD NEW LOCATION          ");
                Console.WriteLine("===================================");

                var location = new LocationDetail();

                Console.Write("Enter City: ");
                location.City = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(location.City))
                {
                    Console.WriteLine("City cannot be empty.");
                    return false;
                }

                Console.Write("Enter Country: ");
                location.Country = Console.ReadLine() ?? string.Empty;

                if (string.IsNullOrWhiteSpace(location.Country))
                {
                    Console.WriteLine("Country cannot be empty.");
                    return false;
                }

                Console.Write("Enter Latitude: ");
                if (double.TryParse(Console.ReadLine(), out double latitude))
                {
                    location.Latitude = latitude;
                }
                else
                {
                    Console.WriteLine("Invalid latitude entered.");
                    return false;
                }

                Console.Write("Enter Longitude: ");
                if (double.TryParse(Console.ReadLine(), out double longitude))
                {
                    location.Longitude = longitude;
                }
                else
                {
                    Console.WriteLine("Invalid longitude entered.");
                    return false;
                }

                // Add location to system
                bool success = await _dataService.AddLocationAsync(location);
                
                if (success)
                {
                    Console.WriteLine($"Location added successfully with ID: {location.Id}");
                }
                else
                {
                    Console.WriteLine("Failed to add location. Please try again.");
                }

                return success;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in AddLocationMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
