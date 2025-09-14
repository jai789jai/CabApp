using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cars
{
    public class RemoveCarMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;
        private readonly ViewCarsMenuAction _viewCarsMenuAction;

        public RemoveCarMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService, ViewCarsMenuAction viewCarsMenuAction)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
            _viewCarsMenuAction = viewCarsMenuAction;
        }

        public string Title => "Remove Car";
        public string Description => "Remove an existing car from the system";
        public char KeyChar => 'R';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("          REMOVE CAR               ");
                Console.WriteLine("===================================");

                await _viewCarsMenuAction.ExecuteAsync();
                
                var cars = await _dataService.GetAllCarsAsync();
                if (cars == null || cars.Count == 0)
                {
                    return true;
                }

                Console.Write("\nEnter the ID of the car to remove: ");
                if (int.TryParse(Console.ReadLine(), out int carId))
                {
                    var carToRemove = cars.FirstOrDefault(c => c.CarId == carId);
                    if (carToRemove != null)
                    {
                        Console.WriteLine($"\nAre you sure you want to remove car ID {carId}?");
                        Console.WriteLine($"Manufacturer: {carToRemove.ManufactureName}");
                        Console.WriteLine($"Model: {carToRemove.ModelName}");
                        Console.Write("Type 'YES' to confirm removal: ");
                        
                        string confirmation = Console.ReadLine() ?? string.Empty;
                        if (confirmation.ToUpper() == "YES")
                        {
                            bool success = await _dataService.RemoveCarAsync(carId);
                            if (success)
                            {
                                Console.WriteLine($"\nCar with ID {carId} has been successfully removed.");
                            }
                            else
                            {
                                Console.WriteLine($"\nFailed to remove car with ID {carId}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nRemoval cancelled.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nCar with ID {carId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid car ID entered.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in RemoveCarMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
