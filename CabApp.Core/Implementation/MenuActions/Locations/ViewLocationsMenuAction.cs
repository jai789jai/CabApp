using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Locations
{
    public class ViewLocationsMenuAction: IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public ViewLocationsMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "View Locations";
        public string Description => "List all the locations in the system";
        public char KeyChar => 'V';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {   
                Console.Clear();
                var locations = await _dataService.GetAllLocationsAsync();

                Console.WriteLine("===================================");
                Console.WriteLine("          LOCATIONS LIST           ");
                Console.WriteLine("===================================");
                if (locations != null && locations.Count > 0)
                {
                    foreach (var location in locations)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"ID: {location.Id}").Append(", ");
                        sb.Append($"City: {location.City}").Append(", ");
                        sb.Append($"Country: {location.Country}").Append(", ");
                        sb.Append($"Latitude: {location.Latitude}").Append(", ");
                        sb.Append($"Longitude: {location.Longitude}");

                        Console.WriteLine(sb.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No locations available in the system.");
                }
            }
            catch(Exception ex)
            {
                _appLogger.LogError("Exception occurred in ViewLocationsMenuAction.ExecuteAsync", ex);
                return false;
            }
            return true;
        }
    }
}
