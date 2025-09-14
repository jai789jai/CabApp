using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Abstraction
{
    public interface IMenuService
    {
        Task DisplayMenuAsync(string title, List<IMenuAction> actions);
        Task WaitForUserAsync(string message = "Press any key to continue...");

    }
}
