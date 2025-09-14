using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Drivers
{
    public class AddDriverMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public AddDriverMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "Add Driver";
        public string Description => "Add a new driver to the system";
        public char KeyChar => 'A';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("         ADD NEW DRIVER            ");
                Console.WriteLine("===================================");

                var driver = new DriverInfo();

                Console.Write("First Name: ");
                driver.FirstName = Console.ReadLine() ?? string.Empty;

                Console.Write("Last Name: ");
                driver.LastName = Console.ReadLine() ?? string.Empty;

                Console.Write("Contact Number: ");
                driver.ContactNumber = Console.ReadLine() ?? string.Empty;

                Console.Write("Address: ");
                driver.Address = Console.ReadLine() ?? string.Empty;

                Console.Write("Date of Birth (yyyy-mm-dd): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime dob))
                {
                    driver.DateOfBirth = dob;
                }

                Console.Write("Date of Joining (yyyy-mm-dd): ");
                if (DateTime.TryParse(Console.ReadLine(), out DateTime doj))
                {
                    driver.DateOfJoining = doj;
                }

                Console.Write("License Number: ");
                if (int.TryParse(Console.ReadLine(), out int licenseNumber))
                {
                    driver.LicenseNumber = licenseNumber;
                }

                Console.Write("KM Driven: ");
                if (int.TryParse(Console.ReadLine(), out int kmDriven))
                {
                    driver.KmDriven = kmDriven;
                }

                bool success = await _dataService.AddDriverAsync(driver);
                
                if (success)
                {
                    Console.WriteLine($"Driver added successfully with ID: {driver.EmpId}");
                }
                else
                {
                    Console.WriteLine("Failed to add driver. Please try again.");
                }

                return success;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in AddDriverMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
