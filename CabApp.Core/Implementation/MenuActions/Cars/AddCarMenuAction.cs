using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cars
{
    public class AddCarMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public AddCarMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "Add Car";
        public string Description => "Add a new car to the system";
        public char KeyChar => 'A';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("           ADD NEW CAR             ");
                Console.WriteLine("===================================");

                var car = new CarInfo();

                Console.Write("Manufacturer Name: ");
                car.ManufactureName = Console.ReadLine() ?? string.Empty;

                Console.Write("Model Name: ");
                car.ModelName = Console.ReadLine() ?? string.Empty;

                Console.Write("Description: ");
                car.Description = Console.ReadLine() ?? string.Empty;

                Console.Write("Manufacture Year: ");
                if (int.TryParse(Console.ReadLine(), out int year))
                {
                    car.ManfactureYear = year;
                }

                Console.Write("KM Driven: ");
                if (int.TryParse(Console.ReadLine(), out int kmDriven))
                {
                    car.KmDriven = kmDriven;
                }

                bool success = await _dataService.AddCarAsync(car);
                
                if (success)
                {
                    Console.WriteLine($"\nCar added successfully with ID: {car.CarId}");
                }
                else
                {
                    Console.WriteLine("\nFailed to add car. Please try again.");
                }

                return success;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in AddCarMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
