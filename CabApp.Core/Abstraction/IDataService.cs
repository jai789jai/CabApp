using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Abstraction
{
    public interface IDataService
    {
        // Cab operations
        Task<List<CabDetails>> GetAllCabsAsync();
        Task<bool> AddCabAsync(CabDetails cab);
        Task<bool> UpdateCabAsync(CabDetails cab);
        Task<bool> RemoveCabAsync(int cabId);
        Task<CabDetails?> GetCabByIdAsync(int cabId);
        
        // Car operations
        Task<List<CarInfo>> GetAllCarsAsync();
        Task<bool> AddCarAsync(CarInfo car);
        Task<bool> UpdateCarAsync(CarInfo car);
        Task<bool> RemoveCarAsync(int carId);
        Task<CarInfo?> GetCarByIdAsync(int carId);
        
        // Driver operations
        Task<List<DriverInfo>> GetAllDriversAsync();
        Task<bool> AddDriverAsync(DriverInfo driver);
        Task<bool> UpdateDriverAsync(DriverInfo driver);
        Task<bool> RemoveDriverAsync(int driverId);
        Task<DriverInfo?> GetDriverByIdAsync(int driverId);
        
        // Trip operations
        Task<List<TripDetail>> GetAllTripsAsync();
        Task<bool> AddTripAsync(TripDetail trip);
        Task<bool> UpdateTripAsync(TripDetail trip);
        Task<bool> RemoveTripAsync(int tripId);
        Task<TripDetail?> GetTripByIdAsync(int tripId);
        
        // Location operations
        Task<List<LocationDetail>> GetAllLocationsAsync();
        Task<bool> AddLocationAsync(LocationDetail location);
        Task<bool> UpdateLocationAsync(LocationDetail location);
        Task<bool> RemoveLocationAsync(int locationId);
        Task<LocationDetail?> GetLocationByIdAsync(int locationId);
    }
}
