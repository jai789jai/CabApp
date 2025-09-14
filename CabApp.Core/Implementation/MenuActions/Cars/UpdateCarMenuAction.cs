using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cars
{
    public class UpdateCarMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;
        private readonly ViewCarsMenuAction _viewCarsMenuAction;

        public UpdateCarMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService, ViewCarsMenuAction viewCarsMenuAction)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
            _viewCarsMenuAction = viewCarsMenuAction;
        }

        public string Title => "Update Car";
        public string Description => "Update an existing car in the system";
        public char KeyChar => 'U';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("          UPDATE CAR               ");
                Console.WriteLine("===================================");

                await _viewCarsMenuAction.ExecuteAsync();
                
                var cars = await _dataService.GetAllCarsAsync();
                if (cars == null || cars.Count == 0)
                {
                    return true;
                }

                Console.Write("Enter the ID of the car to update: ");
                if (int.TryParse(Console.ReadLine(), out int carId))
                {
                    var existingCar = await _dataService.GetCarByIdAsync(carId);
                    if (existingCar != null)
                    {
                        Console.WriteLine($"Updating Car ID: {existingCar.CarId}");
                        Console.WriteLine("===================================");

                        Console.WriteLine("Current Information:");
                        Console.WriteLine($"Manufacturer: {existingCar.ManufactureName}");
                        Console.WriteLine($"Model: {existingCar.ModelName}");
                        Console.WriteLine($"Year: {existingCar.ManfactureYear}");

                        Console.WriteLine("--- Update Car Information (Press Enter to keep current value) ---");
                        Console.Write($"Manufacturer Name [{existingCar.ManufactureName}]: ");
                        string input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingCar.ManufactureName = input;
                        }

                        Console.Write($"Model Name [{existingCar.ModelName}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingCar.ModelName = input;
                        }

                        Console.Write($"Description [{existingCar.Description}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingCar.Description = input;
                        }

                        Console.Write($"Manufacture Year [{existingCar.ManfactureYear}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int year))
                        {
                            existingCar.ManfactureYear = year;
                        }

                        Console.Write($"KM Driven [{existingCar.KmDriven}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int kmDriven))
                        {
                            existingCar.KmDriven = kmDriven;
                        }

                        bool success = await _dataService.UpdateCarAsync(existingCar);
                        if (success)
                        {
                            Console.WriteLine($"Car with ID {carId} has been successfully updated.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to update car with ID {carId}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Car with ID {carId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid car ID entered.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in UpdateCarMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
