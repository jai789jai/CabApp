using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class ViewCabsMenuAction: IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public ViewCabsMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "View Cabs";
        public string Description => "List all the cabs in the system";
        public char KeyChar => 'V';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {   
                Console.Clear();
                var cabs = await _dataService.GetAllCabsAsync();

                Console.WriteLine("===================================");
                Console.WriteLine("            CABS LIST              ");
                Console.WriteLine("===================================");
                if (cabs != null && cabs.Count > 0)
                {
                    // Get related data
                    var cars = await _dataService.GetAllCarsAsync();
                    var drivers = await _dataService.GetAllDriversAsync();
                    var locations = await _dataService.GetAllLocationsAsync();
                    
                    foreach (var cab in cabs)
                    {
                        var car = cars.FirstOrDefault(c => c.CarId == cab.CarId);
                        var driver = drivers.FirstOrDefault(d => d.EmpId == cab.DriverId);
                        var location = locations.FirstOrDefault(l => l.Id == cab.CurrentLocationId);
                        
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"ID: {cab.Id}").Append(", ");
                        sb.Append($"Car: {car?.ModelName ?? "Unknown"}").Append(", ");
                        sb.Append($"Driver: {driver?.FirstName ?? "Unknown"} {driver?.LastName ?? ""}").Append(", ");
                        sb.Append($"Location: {location?.City ?? "Unknown"}").Append(", ");
                        sb.Append($"State: {cab.CurrentWorkState.ToString()}").Append(", ");
                        sb.Append($"Trips: {cab.ComppletedTrips?.Count ?? 0}");
                        
                        // Show idle time if cab is idle
                        if (cab.CurrentWorkState == WorkState.IDLE)
                        {
                            var idleDuration = cab.GetIdleDuration();
                            sb.Append($", Idle: {idleDuration.TotalMinutes:F1} min");
                        }
                        
                        // Show current trip if on trip
                        if (cab.CurrentWorkState == WorkState.ON_TRIP && cab.CurrentTripId.HasValue)
                        {
                            sb.Append($", Trip: {cab.CurrentTripId}");
                        }

                        Console.WriteLine(sb.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No cabs available in the system.");
                }
            }
            catch(Exception ex)
            {
                _appLogger.LogError("Exception occured in ViewCabsMenuAction.ExecuteAsync", ex);
                return false;
            }
            return true;
        }
    }
}
