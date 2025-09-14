using CabApp.Core.Abstraction;

namespace CabApp.Core.Implementation.MenuActions.Locations
{
    public class LocationMenuAction: IMenuAction
    {
        private readonly IMenuCoordinator _menuCoordinator;

        public LocationMenuAction(IMenuCoordinator menuCoordinator)
        {
            _menuCoordinator = menuCoordinator;
        }

        public string Title => "Location Management";
        public string Description => "Manage locations in the system";
        public char KeyChar => 'L';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            await _menuCoordinator.ShowMenuAsync("location");
            return true;
        }
    }
}
