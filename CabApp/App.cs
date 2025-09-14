using CabApp.Core.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp
{
    public class App
    {
        private readonly IAppLogger _appLogger;
        private readonly IMenuCoordinator _menuCoordinator;
        public App(IAppLogger appLogger, IMenuCoordinator menuCoordinator)
        {
            _appLogger = appLogger;
            _menuCoordinator = menuCoordinator;
        }

        public async Task Run(string[] args)
        {
            try
            {
                await _menuCoordinator.ShowMenuAsync("main");
            }
            catch(Exception ex)
            {
                _appLogger.LogError("An error occurred", ex);
            }
        }
    }
}
