using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Abstraction
{
    public interface IDataStore
    {
        Task<List<T>> GetData<T>(string storeName);
        Task WriteData<T>(List<T> data, string storeName);
        void ClearData(string storeName);
    }
}
