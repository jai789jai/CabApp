using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Drivers
{
    public class ViewDriversMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public ViewDriversMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "View Drivers";
        public string Description => "List all the drivers in the system";
        public char KeyChar => 'V';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                var drivers = await _dataService.GetAllDriversAsync();

                Console.WriteLine("===================================");
                Console.WriteLine("          DRIVERS LIST             ");
                Console.WriteLine("===================================");
                if (drivers != null && drivers.Count > 0)
                {
                    foreach (var driver in drivers)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"ID: {driver.EmpId}").Append(", ");
                        sb.Append($"Name: {driver.FirstName} {driver.LastName}").Append(", ");
                        sb.Append($"License: {driver.LicenseNumber}").Append(", ");
                        sb.Append($"Contact: {driver.ContactNumber}").Append(", ");
                        sb.Append($"KM Driven: {driver.KmDriven}");

                        Console.WriteLine(sb.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No drivers available in the system.");
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in ViewDriversMenuAction.ExecuteAsync", ex);
                return false;
            }
            return true;
        }
    }
}
