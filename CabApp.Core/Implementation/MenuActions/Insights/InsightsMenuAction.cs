using CabApp.Core.Abstraction;
using Microsoft.Extensions.DependencyInjection;

namespace CabApp.Core.Implementation.MenuActions.Insights
{
    public class InsightsMenuAction: IMenuAction
    {
        private readonly IMenuCoordinator _menuCoordinator;

        public InsightsMenuAction(IMenuCoordinator menuCoordinator)
        {
            _menuCoordinator = menuCoordinator;
        }

        public string Title => "Insights";
        public string Description => "View analytics and insights about cab operations";
        public char KeyChar => 'I';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            await _menuCoordinator.ShowMenuAsync("insights");
            return true;
        }
    }
}
