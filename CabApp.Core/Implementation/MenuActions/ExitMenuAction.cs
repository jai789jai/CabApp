using CabApp.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation.MenuActions
{
    public class ExitMenuAction: IMenuAction
    {
        public ExitMenuAction()
        {
        }

        public string Title => "Exit";
        public string Description => "Exit the Cab Management System";
        public char KeyChar => 'X';
        public bool IsEnabled => true;

        public async Task<bool> ExecuteAsync()
        {
            Console.Clear();
            Console.WriteLine("Exiting the Cab Management System.");
            Environment.Exit(0);
            await Task.CompletedTask;
            return true;
        }
    }
}
