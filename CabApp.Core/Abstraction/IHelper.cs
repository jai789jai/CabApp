using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Abstraction
{
    public interface IHelper
    {
        Task<bool> ChangeCabLocationAsync(int cabId, int newLocationId);
        Task<bool> ChangeCabStateAsync(int cabId, WorkState newState);
        Task<List<CabDetails>> GetAvailableCabsAtLocationAsync(int locationId);
        Task<CabDetails?> BookCabForTripAsync(int tripId, int fromLocationId);
        Task<bool> CompleteTripAsync(int tripId);
    }
}
