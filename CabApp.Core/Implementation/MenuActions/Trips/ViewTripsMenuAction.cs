using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Trips
{
    public class ViewTripsMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;

        public ViewTripsMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
        }

        public string Title => "View Trips";
        public string Description => "List all the trips in the system";
        public char KeyChar => 'V';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                var trips = await _dataService.GetAllTripsAsync();

                Console.WriteLine("===================================");
                Console.WriteLine("           TRIPS LIST              ");
                Console.WriteLine("===================================");
                if (trips != null && trips.Count > 0)
                {
                    foreach (var trip in trips)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append($"ID: {trip.Id}").Append(", ");
                        sb.Append($"From: {trip.FromLocation.City}, {trip.FromLocation.Country}").Append(", ");
                        sb.Append($"To: {trip.ToLocation.City}, {trip.ToLocation.Country}").Append(", ");
                        sb.Append($"Status: {trip.TripStatus}");

                        Console.WriteLine(sb.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("No trips available in the system.");
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in ViewTripsMenuAction.ExecuteAsync", ex);
                return false;
            }
            return true;
        }
    }
}
