using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation
{
    public class DataService: IDataService
    {
        private readonly IAppLogger _appLogger;
        private readonly IDataStore _dataStore;

        public DataService(IAppLogger appLogger, IDataStore dataStore)
        {
            _appLogger = appLogger;
            _dataStore = dataStore;
        }

        public async Task<List<CabDetails>> GetAllCabsAsync()
        {
            List<CabDetails> cabs = new List<CabDetails>();
            try
            {
                cabs = await _dataStore.GetData<CabDetails>("cabs");
            }
            catch(Exception ex)
            {
                _appLogger.LogError("Exception occured in GetAllCabsAsync", ex);
            }
            return cabs;
        }
    }
}
