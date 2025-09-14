using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class ChangeCabStateMenuAction : IMenuAction
    {
        private readonly IDataService _dataService;
        private readonly IAppLogger _appLogger;

        public ChangeCabStateMenuAction(IDataService dataService, IAppLogger appLogger)
        {
            _dataService = dataService;
            _appLogger = appLogger;
        }

        public string Title => "Change Cab State";
        public string Description => "Change the work state of a cab";
        public char KeyChar => 'S';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            try
            {
                Console.WriteLine("=== Change Cab State ===");

                // Display all cabs
                var cabs = await _dataService.GetAllCabsAsync();
                if (!cabs.Any())
                {
                    Console.WriteLine("No cabs found. Please add cabs first.");
                    return false;
                }

                Console.WriteLine("Available Cabs:");
                foreach (var cab in cabs)
                {
                    var location = await _dataService.GetLocationByIdAsync(cab.CurrentLocationId);
                    var locationName = location?.City ?? "Unknown Location";
                    Console.WriteLine($"ID: {cab.Id}, Car ID: {cab.CarId}, Current Location: {locationName}, State: {cab.CurrentWorkState}");
                }

                // Get cab ID from user
                Console.Write("Enter Cab ID to change state: ");
                if (!int.TryParse(Console.ReadLine(), out int cabId))
                {
                    Console.WriteLine("Invalid cab ID.");
                    return false;
                }

                var selectedCab = await _dataService.GetCabByIdAsync(cabId);
                if (selectedCab == null)
                {
                    Console.WriteLine("Cab not found.");
                    return false;
                }

                // Display available states
                Console.WriteLine("Available States:");
                var states = Enum.GetValues<WorkState>();
                for (int i = 0; i < states.Length; i++)
                {
                    Console.WriteLine($"{i + 1}. {states[i]}");
                }

                // Get new state from user
                Console.Write($"Enter new state (1-{states.Length}): ");
                if (!int.TryParse(Console.ReadLine(), out int stateChoice) || 
                    stateChoice < 1 || stateChoice > states.Length)
                {
                    Console.WriteLine("Invalid state choice.");
                    return false;
                }

                var newState = states[stateChoice - 1];

                // Change the cab state
                var success = await _dataService.ChangeCabStateAsync(cabId, newState);
                if (success)
                {
                    Console.WriteLine($"Cab {cabId} state successfully changed from {selectedCab.CurrentWorkState} to {newState}.");
                }
                else
                {
                    Console.WriteLine("Failed to change cab state.");
                }
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Error in ChangeCabStateMenuAction", ex);
                Console.WriteLine("An error occurred while changing cab state.");
                return false;
            }
            return true;
        }
    }
}
