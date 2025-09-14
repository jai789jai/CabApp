using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Abstraction
{
    public interface IMenuAction
    {
        string Title { get; }
        string Description { get; }
        char KeyChar { get; }
        bool IsEnabled { get; }
        Task<bool> ExecuteAsync();
    }
}
