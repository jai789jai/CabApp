using CabApp.Core.Abstraction;

namespace CabApp.Core.Implementation
{
    public class MenuService : IMenuService
    {
        private readonly IAppLogger _appLogger;
        public MenuService(IAppLogger appLogger)
        {
            _appLogger = appLogger;
        }
        public async Task DisplayMenuAsync(string title, List<IMenuAction> actions)
        {
            try
            {
                Console.Clear();
                Console.WriteLine("===================================");
                Console.WriteLine($"         {title.ToUpper()}        ");
                Console.WriteLine("===================================");
                var enabledActions = actions.Where(a => a.IsEnabled).ToList();
                foreach (var action in enabledActions)
                    Console.WriteLine($"{action.KeyChar}. {action.Title} - {action.Description}");
                Console.WriteLine("===================================");
                Console.Write("Select an option: ");
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occured in DisplayMenuAsync", ex);
            }
            await Task.CompletedTask;
        }

        public async Task WaitForUserAsync(string message = "Press any key to continue...")
        {
            Console.WriteLine();
            Console.WriteLine(message);
            Console.ReadKey();
            await Task.CompletedTask;
        }

    }
}
