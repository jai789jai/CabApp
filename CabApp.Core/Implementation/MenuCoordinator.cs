using CabApp.Core.Abstraction;
using CabApp.Core.Implementation.MenuActions;
using CabApp.Core.Implementation.MenuActions.Cabs;
using CabApp.Core.Implementation.MenuActions.Cars;
using CabApp.Core.Implementation.MenuActions.Drivers;
using CabApp.Core.Implementation.MenuActions.Locations;
using CabApp.Core.Implementation.MenuActions.Trips;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation
{
    public class MenuCoordinator: IMenuCoordinator
    {
        private readonly IMenuService _menuService;
        private readonly IAppLogger _appLogger;
        private readonly IServiceProvider _serviceProvider;
        private bool _isRunning = true;

        public MenuCoordinator(IMenuService menuService, IAppLogger appLogger, IServiceProvider serviceProvider)
        {
            _menuService = menuService;
            _appLogger = appLogger;
            _serviceProvider = serviceProvider;
        }

        public async Task ShowMenuAsync(string menuType = "main")
        {
            try
            {
                while (_isRunning)
                {
                    Console.Clear();
                    var actions = GetActionsForMenu(menuType);
                    await _menuService.DisplayMenuAsync(menuType, actions);

                    var input = Console.ReadLine();

                    await HandleUserInputAsync(input, actions, menuType);
                    
                    menuType = ResetToMainMenu(menuType);
                }
            }
            catch(Exception ex)
            {
                _appLogger.LogError("Exception occured in ShowMenuAsync", ex);
            }
        }

        private static string ResetToMainMenu(string menuType)
        {
            if (menuType.ToLower() != "main")
            {
                menuType = "main";
            }

            return menuType;
        }

        public async Task HandleUserInputAsync(string? input, List<IMenuAction> actions, string menuType)
        {
            if(string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine($"Invalid Choice");
                return;
            }

            var selectionAction = actions.FirstOrDefault(a => a.KeyChar.ToString().Equals(input, StringComparison.OrdinalIgnoreCase) && a.IsEnabled);
            try
            {
                if(selectionAction == null)
                {
                    Console.WriteLine($"Invalid Choice: {input}");
                    return;
                }
                _ = await selectionAction.ExecuteAsync();
                await _menuService.WaitForUserAsync();
            }
            catch(Exception ex)
            {
                _appLogger.LogError("Exception occured in HandleUserInput", ex);
            }
        }

        public void StopApplication()
        {
            _isRunning = false;
        }
        private List<IMenuAction> GetActionsForMenu(string menuType)
        {
            return menuType.ToLower() switch
            {
                "main" => CreateMainMenuAction(),
                "cab" => CreateCabMenuAction(),
                "car" => CreateCarMenuAction(),
                "driver" => CreateDriverMenuAction(),
                "trip" => CreateTripMenuAction(),
                "location" => CreateLocationMenuAction(),
                _ => CreateMainMenuAction(),
            };
        }

        private List<IMenuAction> CreateMainMenuAction()
        {
            return new List<IMenuAction>
            {
                _serviceProvider.GetRequiredService<CabMenuAction>(),
                _serviceProvider.GetRequiredService<CarMenuAction>(),
                _serviceProvider.GetRequiredService<DriverMenuAction>(),
                _serviceProvider.GetRequiredService<TripMenuAction>(),
                _serviceProvider.GetRequiredService<LocationMenuAction>(),
                _serviceProvider.GetRequiredService<ExitMenuAction>(),
            };
        }

        private List<IMenuAction> CreateCabMenuAction()
        {
            return new List<IMenuAction>
            {
                _serviceProvider.GetRequiredService<ViewCabsMenuAction>(),
                _serviceProvider.GetRequiredService<AddCabMenuAction>(),
                _serviceProvider.GetRequiredService<UpdateCabMenuAction>(),
                _serviceProvider.GetRequiredService<RemoveCabMenuAction>(),
                _serviceProvider.GetRequiredService<ChangeCabLocationMenuAction>(),
                _serviceProvider.GetRequiredService<ChangeCabStateMenuAction>(),
                _serviceProvider.GetRequiredService<BookCabMenuAction>(),
                _serviceProvider.GetRequiredService<CompleteTripMenuAction>(),
            };
        }

        private List<IMenuAction> CreateCarMenuAction()
        {
            return new List<IMenuAction>
            {
                _serviceProvider.GetRequiredService<ViewCarsMenuAction>(),
                _serviceProvider.GetRequiredService<AddCarMenuAction>(),
                _serviceProvider.GetRequiredService<UpdateCarMenuAction>(),
                _serviceProvider.GetRequiredService<RemoveCarMenuAction>(),
            };
        }

        private List<IMenuAction> CreateDriverMenuAction()
        {
            return new List<IMenuAction>
            {
                _serviceProvider.GetRequiredService<ViewDriversMenuAction>(),
                _serviceProvider.GetRequiredService<AddDriverMenuAction>(),
                _serviceProvider.GetRequiredService<UpdateDriverMenuAction>(),
                _serviceProvider.GetRequiredService<RemoveDriverMenuAction>(),
            };
        }

        private List<IMenuAction> CreateTripMenuAction()
        {
            return new List<IMenuAction>
            {
                _serviceProvider.GetRequiredService<ViewTripsMenuAction>(),
                _serviceProvider.GetRequiredService<AddTripMenuAction>(),
                _serviceProvider.GetRequiredService<UpdateTripMenuAction>(),
                _serviceProvider.GetRequiredService<RemoveTripMenuAction>(),
            };
        }

        private List<IMenuAction> CreateLocationMenuAction()
        {
            return new List<IMenuAction>
            {
                _serviceProvider.GetRequiredService<ViewLocationsMenuAction>(),
                _serviceProvider.GetRequiredService<AddLocationMenuAction>(),
                _serviceProvider.GetRequiredService<UpdateLocationMenuAction>(),
                _serviceProvider.GetRequiredService<RemoveLocationMenuAction>(),
            };
        }

    }
}
