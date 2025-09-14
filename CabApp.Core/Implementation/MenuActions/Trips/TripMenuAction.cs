using CabApp.Core.Abstraction;

namespace CabApp.Core.Implementation.MenuActions.Trips
{
    public class TripMenuAction: IMenuAction
    {
        private readonly IMenuCoordinator _menuCoordinator;

        public TripMenuAction(IMenuCoordinator menuCoordinator)
        {
            _menuCoordinator = menuCoordinator;
        }

        public string Title => "Trip Management";
        public string Description => "Manage trips in the system";
        public char KeyChar => 'T';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            await _menuCoordinator.ShowMenuAsync("trip");
            return true;
        }
    }
}
