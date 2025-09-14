using CabApp.Core.Abstraction;

namespace CabApp.Core.Implementation.MenuActions.Cars
{
    public class CarMenuAction: IMenuAction
    {
        private readonly IMenuCoordinator _menuCoordinator;

        public CarMenuAction(IMenuCoordinator menuCoordinator)
        {
            _menuCoordinator = menuCoordinator;
        }

        public string Title => "Car Management";
        public string Description => "Manage cars in the system";
        public char KeyChar => 'R';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            await _menuCoordinator.ShowMenuAsync("car");
            return true;
        }
    }
}
