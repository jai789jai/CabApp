using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Trips
{
    public class RemoveTripMenuAction : IMenuAction
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuService _menuService;
        private readonly IDataService _dataService;
        private readonly ViewTripsMenuAction _viewTripsMenuAction;

        public RemoveTripMenuAction(IAppLogger logger, IMenuService menuService, IDataService dataService, ViewTripsMenuAction viewTripsMenuAction)
        {
            _appLogger = logger;
            _menuService = menuService;
            _dataService = dataService;
            _viewTripsMenuAction = viewTripsMenuAction;
        }

        public string Title => "Remove Trip";
        public string Description => "Remove an existing trip from the system";
        public char KeyChar => 'R';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine("         REMOVE TRIP               ");
                Console.WriteLine("===================================");

                await _viewTripsMenuAction.ExecuteAsync();
                
                var trips = await _dataService.GetAllTripsAsync();
                if (trips == null || trips.Count == 0)
                {
                    return true;
                }

                Console.Write("\nEnter the ID of the trip to remove: ");
                if (int.TryParse(Console.ReadLine(), out int tripId))
                {
                    var tripToRemove = trips.FirstOrDefault(t => t.Id == tripId);
                    if (tripToRemove != null)
                    {
                        Console.WriteLine($"\nAre you sure you want to remove trip ID {tripId}?");
                        Console.WriteLine($"From: {tripToRemove.FromLocation.City}, {tripToRemove.FromLocation.Country}");
                        Console.WriteLine($"To: {tripToRemove.ToLocation.City}, {tripToRemove.ToLocation.Country}");
                        Console.WriteLine($"Status: {tripToRemove.TripStatus}");
                        Console.Write("Type 'YES' to confirm removal: ");
                        
                        string confirmation = Console.ReadLine() ?? string.Empty;
                        if (confirmation.ToUpper() == "YES")
                        {
                            bool success = await _dataService.RemoveTripAsync(tripId);
                            if (success)
                            {
                                Console.WriteLine($"\nTrip with ID {tripId} has been successfully removed.");
                            }
                            else
                            {
                                Console.WriteLine($"\nFailed to remove trip with ID {tripId}.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("\nRemoval cancelled.");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"\nTrip with ID {tripId} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("\nInvalid trip ID entered.");
                }

                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in RemoveTripMenuAction.ExecuteAsync", ex);
                return false;
            }
        }
    }
}
