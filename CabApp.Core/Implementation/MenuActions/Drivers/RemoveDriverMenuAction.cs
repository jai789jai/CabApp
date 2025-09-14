using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Drivers
{
    public class RemoveDriverMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;
        private readonly ViewDriversMenuAction _viewDriversMenuAction;

        public RemoveDriverMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService, ViewDriversMenuAction viewDriversMenuAction)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
            _viewDriversMenuAction = viewDriversMenuAction;
        }

        public string Title => "Remove Driver";
        public string Description => "Remove an existing driver from the system";
        public char KeyChar => 'R';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("        REMOVE DRIVER              ");
                Console.WriteLine("===================================");

                await _viewDriversMenuAction.ExecuteAsync();
                
                var drivers = await _dataService.GetAllDriversAsync();
                if (drivers == null || drivers.Count == 0)
                {
                    return true;
                }

                Console.Write("\nEnter the ID of the driver to remove: ");
                if (int.TryParse(Console.ReadLine(), out int driverId))
                {
                    var driverToRemove = drivers.FirstOrDefault(d => d.EmpId == driverId);
                    if (driverToRemove != null)
                    {
                        Console.WriteLine($"\nAre you sure you want to remove driver ID {driverId}?");
                        Console.WriteLine($"Name: {driverToRemove.FirstName} {driverToRemove.LastName}");
                        Console.WriteLine($"License: {driverToRemove.LicenseNumber}");
                        Console.Write("Type 'YES' to confirm removal: ");
                        
                        string confirmation = Console.ReadLine() ?? string.Empty;
                        if (confirmation.ToUpper() == "YES")
                        {
                            bool success = await _dataService.RemoveDriverAsync(driverId);
                            if (success)
                            {
                                Console.WriteLine($"\nDriver with ID {driverId} has been successfully removed.");
                            }
                            else
                            {
                                Console.WriteLine($"\nFailed to remove driver with ID {driverId}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nRemoval cancelled.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nDriver with ID {driverId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid driver ID entered.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in RemoveDriverMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
