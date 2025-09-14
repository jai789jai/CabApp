using CabApp.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class RemoveCabMenuAction: IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;
        private readonly ViewCabsMenuAction _viewCabsMenuAction;

        public RemoveCabMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService, ViewCabsMenuAction viewCabsMenuAction)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
            _viewCabsMenuAction = viewCabsMenuAction;
        }

        public string Title => "Delete Cab";
        public string Description => "Remove an Existing Cab from the system";
        public char KeyChar => 'D';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("           DELETE CAB              ");
                Console.WriteLine("===================================");

                // Use ViewCabsMenuAction to list all cabs
                await _viewCabsMenuAction.ExecuteAsync();
                
                var cabs = await _dataService.GetAllCabsAsync();
                if (cabs == null || cabs.Count == 0)
                {
                    return true;
                }

                Console.Write("\nEnter the ID of the cab to delete: ");
                if (int.TryParse(Console.ReadLine(), out int cabId))
                {
                    // Confirm deletion
                    var cabToDelete = cabs.FirstOrDefault(c => c.Id == cabId);
                    if (cabToDelete != null)
                    {
                        // Get related data for display
                        var cars = await _dataService.GetAllCarsAsync();
                        var drivers = await _dataService.GetAllDriversAsync();
                        var car = cars.FirstOrDefault(c => c.CarId == cabToDelete.CarId);
                        var driver = drivers.FirstOrDefault(d => d.EmpId == cabToDelete.DriverId);

                        Console.WriteLine($"\nAre you sure you want to delete cab ID {cabId}?");
                        Console.WriteLine($"Car: {car?.ModelName ?? "Unknown"} (ID: {cabToDelete.CarId})");
                        Console.WriteLine($"Driver: {driver?.FirstName ?? "Unknown"} {driver?.LastName ?? ""} (ID: {cabToDelete.DriverId})");
                        Console.WriteLine($"Work State: {cabToDelete.CurrentWorkState}");
                        Console.Write("Type 'YES' to confirm deletion: ");
                        
                        string confirmation = Console.ReadLine() ?? string.Empty;
                        if (confirmation.ToUpper() == "YES")
                        {
                            bool success = await _dataService.RemoveCabAsync(cabId);
                            if (success)
                            {
                                Console.WriteLine($"\nCab with ID {cabId} has been successfully deleted.");
                            }
                            else
                            {
                                Console.WriteLine($"\nFailed to delete cab with ID {cabId}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nDeletion cancelled.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nCab with ID {cabId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid cab ID entered.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occured in RemoveCabMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
