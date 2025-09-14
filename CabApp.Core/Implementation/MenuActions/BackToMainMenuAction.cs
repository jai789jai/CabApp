using CabApp.Core.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace CabApp.Core.Implementation.MenuActions
{
    public class BackToMainMenuAction : IMenuAction
    {
        private readonly IMenuCoordinator _menuCoordinator;

        public BackToMainMenuAction(IMenuCoordinator menuCoordinator)
        {
            _menuCoordinator = menuCoordinator;
        }

        public string Title => "Back to Main Menu";
        public string Description => "Return to the main menu";
        public char KeyChar => 'B';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            await _menuCoordinator.ShowMenuAsync("main");
            return true;
        }
    }
}
