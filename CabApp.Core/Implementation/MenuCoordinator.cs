using CabApp.Core.Abstraction;
using CabApp.Core.Implementation.MenuActions;
using CabApp.Core.Implementation.MenuActions.Cabs;
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

                    Console.Write("Enter your choice: ");
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
                _ => CreateMainMenuAction(),
            };
        }

        private List<IMenuAction> CreateMainMenuAction()
        {
            return new List<IMenuAction>
            {
                _serviceProvider.GetRequiredService<CabMenuAction>(),
                _serviceProvider.GetRequiredService<ExitMenuAction>(),
            };
        }

        private List<IMenuAction> CreateCabMenuAction()
        {
            return new List<IMenuAction>
            {
                _serviceProvider.GetRequiredService<ViewCabsMenuAction>(),
            };
        }

    }
}
