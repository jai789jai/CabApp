using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class ChangeCabLocationMenuAction : IMenuAction
    {
        private readonly IDataService _dataService;
        private readonly IHelper _helper;
        private readonly IAppLogger _appLogger;

        public ChangeCabLocationMenuAction(IDataService dataService, IAppLogger appLogger, IHelper helper)
        {
            _dataService = dataService;
            _appLogger = appLogger;
            _helper = helper;
        }

        public string Title => "Change Cab Location";
        public string Description => "Change the current location of a cab";
        public char KeyChar => 'L';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.WriteLine("=== Change Cab Location ===");

                // Display all cabs
                var cabs = await _dataService.GetAllCabsAsync();
                if (!cabs.Any())
                {
                    Console.WriteLine("No cabs found. Please add cabs first.");
                    return false;
                }

                Console.WriteLine("Available Cabs:");
                foreach (var cab in cabs)
                {
                    var location = await _dataService.GetLocationByIdAsync(cab.CurrentLocationId);
                    var locationName = location?.City ?? "Unknown Location";
                    Console.WriteLine($"ID: {cab.Id}, Car ID: {cab.CarId}, Current Location: {locationName}, State: {cab.CurrentWorkState}");
                }

                // Get cab ID from user
                Console.Write("Enter Cab ID to change location: ");
                if (!int.TryParse(Console.ReadLine(), out int cabId))
                {
                    Console.WriteLine("Invalid cab ID.");
                    return false;
                }

                var selectedCab = await _dataService.GetCabByIdAsync(cabId);
                if (selectedCab == null)
                {
                    Console.WriteLine("Cab not found.");
                    return false;
                }

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

                // Get new location ID from user
                Console.Write("Enter New Location ID: ");
                if (!int.TryParse(Console.ReadLine(), out int newLocationId))
                {
                    Console.WriteLine("Invalid location ID.");
                    return false;
                }

                // Change the cab location
                var success = await _helper.ChangeCabLocationAsync(cabId, newLocationId);
                if (success)
                {
                    var newLocation = await _dataService.GetLocationByIdAsync(newLocationId);
                    Console.WriteLine($"Cab {cabId} location successfully changed to {newLocation?.City}.");
                }
                else
                {
                    Console.WriteLine("Failed to change cab location. Please check if the location exists.");
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Error in ChangeCabLocationMenuAction", ex);
                Console.WriteLine("An error occurred while changing cab location.");
                return false;
            }
            return true;
        }
    }
}
