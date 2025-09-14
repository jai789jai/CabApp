using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Drivers
{
    public class UpdateDriverMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;
        private readonly ViewDriversMenuAction _viewDriversMenuAction;

        public UpdateDriverMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService, ViewDriversMenuAction viewDriversMenuAction)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
            _viewDriversMenuAction = viewDriversMenuAction;
        }

        public string Title => "Update Driver";
        public string Description => "Update an existing driver in the system";
        public char KeyChar => 'U';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("        UPDATE DRIVER              ");
                Console.WriteLine("===================================");

                await _viewDriversMenuAction.ExecuteAsync();
                
                var drivers = await _dataService.GetAllDriversAsync();
                if (drivers == null || drivers.Count == 0)
                {
                    return true;
                }

                Console.Write("Enter the ID of the driver to update: ");
                if (int.TryParse(Console.ReadLine(), out int driverId))
                {
                    var existingDriver = await _dataService.GetDriverByIdAsync(driverId);
                    if (existingDriver != null)
                    {
                        Console.WriteLine($"Updating Driver ID: {existingDriver.EmpId}");
                        Console.WriteLine("===================================");

                        Console.WriteLine("Current Information:");
                        Console.WriteLine($"Name: {existingDriver.FirstName} {existingDriver.LastName}");
                        Console.WriteLine($"License: {existingDriver.LicenseNumber}");

                        Console.WriteLine("--- Update Driver Information (Press Enter to keep current value) ---");
                        Console.Write($"First Name [{existingDriver.FirstName}]: ");
                        string input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingDriver.FirstName = input;
                        }

                        Console.Write($"Last Name [{existingDriver.LastName}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingDriver.LastName = input;
                        }

                        Console.Write($"Contact Number [{existingDriver.ContactNumber}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingDriver.ContactNumber = input;
                        }

                        Console.Write($"Address [{existingDriver.Address}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input))
                        {
                            existingDriver.Address = input;
                        }

                        Console.Write($"License Number [{existingDriver.LicenseNumber}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int licenseNumber))
                        {
                            existingDriver.LicenseNumber = licenseNumber;
                        }

                        Console.Write($"KM Driven [{existingDriver.KmDriven}]: ");
                        input = Console.ReadLine() ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(input) && int.TryParse(input, out int kmDriven))
                        {
                            existingDriver.KmDriven = kmDriven;
                        }

                        bool success = await _dataService.UpdateDriverAsync(existingDriver);
                        if (success)
                        {
                            Console.WriteLine($"Driver with ID {driverId} has been successfully updated.");
                        }
                        else
                        {
                            Console.WriteLine($"Failed to update driver with ID {driverId}.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Driver with ID {driverId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid driver ID entered.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in UpdateDriverMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
