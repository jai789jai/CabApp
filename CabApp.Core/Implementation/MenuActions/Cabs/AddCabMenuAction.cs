using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class AddCabMenuAction: IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public AddCabMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "Register Cab";
        public string Description => "Register a new Cab in the system";
        public char KeyChar => 'R';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("         REGISTER NEW CAB          ");
                Console.WriteLine("===================================");

                var cab = new CabDetails();

                // Show available cars
                Console.WriteLine("\n--- Available Cars ---");
                var cars = await _dataService.GetAllCarsAsync();
                if (cars != null && cars.Count > 0)
                {
                    foreach (var car in cars)
                    {
                        Console.WriteLine($"ID: {car.CarId} - {car.ManufactureName} {car.ModelName}");
                    }
                }
                else
                {
                    Console.WriteLine("No cars available. Please add cars first.");
                    return false;
                }

                Console.Write("\nEnter Car ID: ");
                if (int.TryParse(Console.ReadLine(), out int carId))
                {
                    var selectedCar = await _dataService.GetCarByIdAsync(carId);
                    if (selectedCar != null)
                    {
                        cab.CarId = carId;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Car ID selected.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Car ID entered.");
                    return false;
                }

                // Show available drivers
                Console.WriteLine("\n--- Available Drivers ---");
                var drivers = await _dataService.GetAllDriversAsync();
                if (drivers != null && drivers.Count > 0)
                {
                    foreach (var driver in drivers)
                    {
                        Console.WriteLine($"ID: {driver.EmpId} - {driver.FirstName} {driver.LastName}");
                    }
                }
                else
                {
                    Console.WriteLine("No drivers available. Please add drivers first.");
                    return false;
                }

                Console.Write("\nEnter Driver ID: ");
                if (int.TryParse(Console.ReadLine(), out int driverId))
                {
                    var selectedDriver = await _dataService.GetDriverByIdAsync(driverId);
                    if (selectedDriver != null)
                    {
                        cab.DriverId = driverId;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Driver ID selected.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Driver ID entered.");
                    return false;
                }

                // Show available locations
                Console.WriteLine("\n--- Available Locations ---");
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

                Console.Write("\nEnter Location ID: ");
                if (int.TryParse(Console.ReadLine(), out int locationId))
                {
                    var selectedLocation = await _dataService.GetLocationByIdAsync(locationId);
                    if (selectedLocation != null)
                    {
                        cab.CurrentLocationId = locationId;
                    }
                    else
                    {
                        Console.WriteLine("Invalid Location ID selected.");
                        return false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Location ID entered.");
                    return false;
                }

                // Set default work state and initialize tracking
                cab.CurrentWorkState = WorkState.IDLE;
                cab.LastIdleTime = DateTime.UtcNow;
                cab.ComppletedTrips = new List<int>();

                // Add cab to system
                bool success = await _dataService.AddCabAsync(cab);
                
                if (success)
                {
                    Console.WriteLine($"\nCab registered successfully with ID: {cab.Id}");
                }
                else
                {
                    Console.WriteLine("\nFailed to register cab. Please try again.");
                }

                return success;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occured in AddCabMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
