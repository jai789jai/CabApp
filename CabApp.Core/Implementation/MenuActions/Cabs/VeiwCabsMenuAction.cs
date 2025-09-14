using CabApp.Core.Abstraction;
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
                    foreach (var cab in cabs)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"ID: {cab.Id}").Append(", ");
                        sb.Append($"Model: {cab.CarInfo.ModelName}").Append(", ");
                        sb.Append($"CompletedTrips: {cab.ComppletedTrips}").Append(", ");
                        sb.Append($"WorkState: {cab.CurrentWorkState.ToString()}");

                        Console.WriteLine(sb.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No cabs available in the system.");
                }
                await _menuService.WaitForUserAsync();
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
