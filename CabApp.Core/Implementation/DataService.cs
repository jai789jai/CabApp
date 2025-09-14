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

        public async Task<bool> AddCabAsync(CabDetails cab)
        {
            try
            {
                var cabs = await GetAllCabsAsync();
                
                // Generate new ID if not provided
                if (cab.Id == 0)
                {
                    cab.Id = cabs.Count > 0 ? cabs.Max(c => c.Id) + 1 : 1;
                }
                
                cabs.Add(cab);
                await _dataStore.WriteData(cabs, "cabs");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in AddCabAsync", ex);
                return false;
            }
        }

        public async Task<bool> UpdateCabAsync(CabDetails cab)
        {
            try
            {
                var cabs = await GetAllCabsAsync();
                var existingCab = cabs.FirstOrDefault(c => c.Id == cab.Id);
                
                if (existingCab == null)
                    return false;
                
                var index = cabs.IndexOf(existingCab);
                cabs[index] = cab;
                
                await _dataStore.WriteData(cabs, "cabs");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in UpdateCabAsync", ex);
                return false;
            }
        }

        public async Task<bool> RemoveCabAsync(int cabId)
        {
            try
            {
                var cabs = await GetAllCabsAsync();
                var cabToRemove = cabs.FirstOrDefault(c => c.Id == cabId);
                
                if (cabToRemove == null)
                    return false;
                
                cabs.Remove(cabToRemove);
                await _dataStore.WriteData(cabs, "cabs");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in RemoveCabAsync", ex);
                return false;
            }
        }

        public async Task<CabDetails?> GetCabByIdAsync(int cabId)
        {
            try
            {
                var cabs = await GetAllCabsAsync();
                return cabs.FirstOrDefault(c => c.Id == cabId);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in GetCabByIdAsync", ex);
                return null;
            }
        }

        // Car operations
        public async Task<List<CarInfo>> GetAllCarsAsync()
        {
            List<CarInfo> cars = new List<CarInfo>();
            try
            {
                cars = await _dataStore.GetData<CarInfo>("cars");
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in GetAllCarsAsync", ex);
            }
            return cars;
        }

        public async Task<bool> AddCarAsync(CarInfo car)
        {
            try
            {
                var cars = await GetAllCarsAsync();
                
                if (car.CarId == 0)
                {
                    car.CarId = cars.Count > 0 ? cars.Max(c => c.CarId) + 1 : 1;
                }
                
                cars.Add(car);
                await _dataStore.WriteData(cars, "cars");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in AddCarAsync", ex);
                return false;
            }
        }

        public async Task<bool> UpdateCarAsync(CarInfo car)
        {
            try
            {
                var cars = await GetAllCarsAsync();
                var existingCar = cars.FirstOrDefault(c => c.CarId == car.CarId);
                
                if (existingCar == null)
                    return false;
                
                var index = cars.IndexOf(existingCar);
                cars[index] = car;
                
                await _dataStore.WriteData(cars, "cars");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in UpdateCarAsync", ex);
                return false;
            }
        }

        public async Task<bool> RemoveCarAsync(int carId)
        {
            try
            {
                var cars = await GetAllCarsAsync();
                var carToRemove = cars.FirstOrDefault(c => c.CarId == carId);
                
                if (carToRemove == null)
                    return false;
                
                cars.Remove(carToRemove);
                await _dataStore.WriteData(cars, "cars");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in RemoveCarAsync", ex);
                return false;
            }
        }

        public async Task<CarInfo?> GetCarByIdAsync(int carId)
        {
            try
            {
                var cars = await GetAllCarsAsync();
                return cars.FirstOrDefault(c => c.CarId == carId);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in GetCarByIdAsync", ex);
                return null;
            }
        }

        // Driver operations
        public async Task<List<DriverInfo>> GetAllDriversAsync()
        {
            List<DriverInfo> drivers = new List<DriverInfo>();
            try
            {
                drivers = await _dataStore.GetData<DriverInfo>("drivers");
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in GetAllDriversAsync", ex);
            }
            return drivers;
        }

        public async Task<bool> AddDriverAsync(DriverInfo driver)
        {
            try
            {
                var drivers = await GetAllDriversAsync();
                
                if (driver.EmpId == 0)
                {
                    driver.EmpId = drivers.Count > 0 ? drivers.Max(d => d.EmpId) + 1 : 1;
                }
                
                drivers.Add(driver);
                await _dataStore.WriteData(drivers, "drivers");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in AddDriverAsync", ex);
                return false;
            }
        }

        public async Task<bool> UpdateDriverAsync(DriverInfo driver)
        {
            try
            {
                var drivers = await GetAllDriversAsync();
                var existingDriver = drivers.FirstOrDefault(d => d.EmpId == driver.EmpId);
                
                if (existingDriver == null)
                    return false;
                
                var index = drivers.IndexOf(existingDriver);
                drivers[index] = driver;
                
                await _dataStore.WriteData(drivers, "drivers");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in UpdateDriverAsync", ex);
                return false;
            }
        }

        public async Task<bool> RemoveDriverAsync(int driverId)
        {
            try
            {
                var drivers = await GetAllDriversAsync();
                var driverToRemove = drivers.FirstOrDefault(d => d.EmpId == driverId);
                
                if (driverToRemove == null)
                    return false;
                
                drivers.Remove(driverToRemove);
                await _dataStore.WriteData(drivers, "drivers");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in RemoveDriverAsync", ex);
                return false;
            }
        }

        public async Task<DriverInfo?> GetDriverByIdAsync(int driverId)
        {
            try
            {
                var drivers = await GetAllDriversAsync();
                return drivers.FirstOrDefault(d => d.EmpId == driverId);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in GetDriverByIdAsync", ex);
                return null;
            }
        }

        // Trip operations
        public async Task<List<TripDetail>> GetAllTripsAsync()
        {
            List<TripDetail> trips = new List<TripDetail>();
            try
            {
                trips = await _dataStore.GetData<TripDetail>("trips");
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in GetAllTripsAsync", ex);
            }
            return trips;
        }

        public async Task<bool> AddTripAsync(TripDetail trip)
        {
            try
            {
                var trips = await GetAllTripsAsync();
                
                if (trip.Id == 0)
                {
                    trip.Id = trips.Count > 0 ? trips.Max(t => t.Id) + 1 : 1;
                }
                
                trips.Add(trip);
                await _dataStore.WriteData(trips, "trips");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in AddTripAsync", ex);
                return false;
            }
        }

        public async Task<bool> UpdateTripAsync(TripDetail trip)
        {
            try
            {
                var trips = await GetAllTripsAsync();
                var existingTrip = trips.FirstOrDefault(t => t.Id == trip.Id);
                
                if (existingTrip == null)
                    return false;
                
                var index = trips.IndexOf(existingTrip);
                trips[index] = trip;
                
                await _dataStore.WriteData(trips, "trips");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in UpdateTripAsync", ex);
                return false;
            }
        }

        public async Task<bool> RemoveTripAsync(int tripId)
        {
            try
            {
                var trips = await GetAllTripsAsync();
                var tripToRemove = trips.FirstOrDefault(t => t.Id == tripId);
                
                if (tripToRemove == null)
                    return false;
                
                trips.Remove(tripToRemove);
                await _dataStore.WriteData(trips, "trips");
                return true;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in RemoveTripAsync", ex);
                return false;
            }
        }

        public async Task<TripDetail?> GetTripByIdAsync(int tripId)
        {
            try
            {
                var trips = await GetAllTripsAsync();
                return trips.FirstOrDefault(t => t.Id == tripId);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in GetTripByIdAsync", ex);
                return null;
            }
        }
    }
}
