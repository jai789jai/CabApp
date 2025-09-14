using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Abstraction
{
    public interface IMenuCoordinator
    {
        Task ShowMenuAsync(string menuType = "main");
        Task HandleUserInputAsync(string input, List<IMenuAction> actions, string currentMenu);
        void StopApplication();
    }
}
