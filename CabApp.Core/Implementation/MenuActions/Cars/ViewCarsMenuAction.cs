using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cars
{
    public class ViewCarsMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public ViewCarsMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "View Cars";
        public string Description => "List all the cars in the system";
        public char KeyChar => 'V';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                var cars = await _dataService.GetAllCarsAsync();

                Console.WriteLine("===================================");
                Console.WriteLine("            CARS LIST              ");
                Console.WriteLine("===================================");
                if (cars != null && cars.Count > 0)
                {
                    foreach (var car in cars)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"ID: {car.CarId}").Append(", ");
                        sb.Append($"Manufacturer: {car.ManufactureName}").Append(", ");
                        sb.Append($"Model: {car.ModelName}").Append(", ");
                        sb.Append($"Year: {car.ManfactureYear}").Append(", ");
                        sb.Append($"KM Driven: {car.KmDriven}");

                        Console.WriteLine(sb.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No cars available in the system.");
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in ViewCarsMenuAction.ExecuteAsync", ex);
                return false;
            }
            return true;
        }
    }
}
