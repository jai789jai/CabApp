using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class UpdateCabMenuAction: IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;
        private readonly ViewCabsMenuAction _viewCabsMenuAction;

        public UpdateCabMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService, ViewCabsMenuAction viewCabsMenuAction)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
            _viewCabsMenuAction = viewCabsMenuAction;
        }

        public string Title => "Update Cab";
        public string Description => "Update Existing Cab in the system";
        public char KeyChar => 'U';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("           UPDATE CAB              ");
                Console.WriteLine("===================================");

                // Use ViewCabsMenuAction to list all cabs
                await _viewCabsMenuAction.ExecuteAsync();
                
                var cabs = await _dataService.GetAllCabsAsync();
                if (cabs == null || cabs.Count == 0)
                {
                    return true;
                }

                Console.Write("Enter the ID of the cab to update: ");
                if (int.TryParse(Console.ReadLine(), out int cabId))
                {
                    var existingCab = await _dataService.GetCabByIdAsync(cabId);
                    if (existingCab != null)
                    {
                        Console.WriteLine($"Updating Cab ID: {existingCab.Id}");
                        Console.WriteLine("===================================");

                        // Get related data for display
                        var cars = await _dataService.GetAllCarsAsync();
                        var drivers = await _dataService.GetAllDriversAsync();
                        var currentCar = cars.FirstOrDefault(c => c.CarId == existingCab.CarId);
                        var currentDriver = drivers.FirstOrDefault(d => d.EmpId == existingCab.DriverId);

                        // Display current information
                        Console.WriteLine("Current Information:");
                        Console.WriteLine($"Car: {currentCar?.ModelName ?? "Unknown"} (ID: {existingCab.CarId})");
                        Console.WriteLine($"Driver: {currentDriver?.FirstName ?? "Unknown"} {currentDriver?.LastName ?? ""} (ID: {existingCab.DriverId})");
                        Console.WriteLine($"Work State: {existingCab.CurrentWorkState}");

                        // Update Car Assignment
                        Console.WriteLine("--- Available Cars ---");
                        foreach (var car in cars)
                        {
                            Console.WriteLine($"ID: {car.CarId} - {car.ManufactureName} {car.ModelName}");
                        }
                        Console.Write($"Current Car ID [{existingCab.CarId}]: ");
                        string input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int carId))
                        {
                            var selectedCar = await _dataService.GetCarByIdAsync(carId);
                            if (selectedCar != null)
                            {
                                existingCab.CarId = carId;
                            }
                        }

                        // Update Driver Assignment
                        Console.WriteLine("--- Available Drivers ---");
                        foreach (var driver in drivers)
                        {
                            Console.WriteLine($"ID: {driver.EmpId} - {driver.FirstName} {driver.LastName}");
                        }
                        Console.Write($"Current Driver ID [{existingCab.DriverId}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int driverId))
                        {
                            var selectedDriver = await _dataService.GetDriverByIdAsync(driverId);
                            if (selectedDriver != null)
                            {
                                existingCab.DriverId = driverId;
                            }
                        }

                        // Update Work State
                        Console.WriteLine("--- Update Work State ---");
                        Console.WriteLine("1. IDLE");
                        Console.WriteLine("2. ON_TRIP");
                        Console.WriteLine("3. GROUNDED");
                        Console.Write($"Current Work State: {existingCab.CurrentWorkState}. Enter new state (1-3) or press Enter to keep current: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int workStateChoice))
                        {
                            switch (workStateChoice)
                            {
                                case 1:
                                    existingCab.CurrentWorkState = WorkState.IDLE;
                                    break;
                                case 2:
                                    existingCab.CurrentWorkState = WorkState.ON_TRIP;
                                    break;
                                case 3:
                                    existingCab.CurrentWorkState = WorkState.GROUNDED;
                                    break;
                            }
                        }

                        // Save changes
                        bool success = await _dataService.UpdateCabAsync(existingCab);
                        if (success)
                        {
                            Console.WriteLine($"Cab with ID {cabId} has been successfully updated.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to update cab with ID {cabId}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Cab with ID {cabId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid cab ID entered.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occured in UpdateCabMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
