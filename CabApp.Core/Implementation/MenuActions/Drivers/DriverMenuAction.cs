using CabApp.Core.Abstraction;

namespace CabApp.Core.Implementation.MenuActions.Drivers
{
    public class DriverMenuAction: IMenuAction
    {
        private readonly IMenuCoordinator _menuCoordinator;

        public DriverMenuAction(IMenuCoordinator menuCoordinator)
        {
            _menuCoordinator = menuCoordinator;
        }

        public string Title => "Driver Management";
        public string Description => "Manage drivers in the system";
        public char KeyChar => 'D';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            await _menuCoordinator.ShowMenuAsync("driver");
            return true;
        }
    }
}
