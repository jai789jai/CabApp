using CabApp.Core.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace CabApp.Core.Implementation.MenuActions.Cabs
{
    public class CabMenuAction: IMenuAction
    {
        private readonly IMenuCoordinator _menuCoordinator;

        public CabMenuAction(IMenuCoordinator menuCoordinator)
        {
            _menuCoordinator = menuCoordinator;
        }

        public string Title => "Cab Management";
        public string Description => "Manage cabs in the system";
        public char KeyChar => 'C';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            await _menuCoordinator.ShowMenuAsync("cab");
            return true;
        }
    }
}
