using CabApp.Core.Abstraction;
using CabApp.Core.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.Implementation
{
    public class Helper
    {
        private readonly IAppLogger _appLogger;
        private readonly IDataService _dataService;

        public Helper(IAppLogger appLogger, IDataService dataService)
        {
            _appLogger = appLogger;
            _dataService = dataService;
        }

        // Cab management operations
        public async Task<bool> ChangeCabLocationAsync(int cabId, int newLocationId)
        {
            try
            {
                var cab = await _dataService.GetCabByIdAsync(cabId);
                if (cab == null)
                    return false;

                // Verify the new location exists
                var location = await _dataService.GetLocationByIdAsync(newLocationId);
                if (location == null)
                    return false;

                cab.CurrentLocationId = newLocationId;
                return await _dataService.UpdateCabAsync(cab);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in ChangeCabLocationAsync", ex);
                return false;
            }
        }

        public async Task<bool> ChangeCabStateAsync(int cabId, WorkState newState)
        {
            try
            {
                var cab = await _dataService.GetCabByIdAsync(cabId);
                if (cab == null)
                    return false;

                var oldState = cab.CurrentWorkState;
                cab.CurrentWorkState = newState;

                if (newState == WorkState.IDLE)
                {
                    cab.IdleStartTime = DateTime.UtcNow;
                    cab.CurrentTripId = null;
                }
                else if (newState == WorkState.ON_TRIP && oldState == WorkState.IDLE)
                {
                    cab.CurrentTripId = null;
                }

                return await _dataService.UpdateCabAsync(cab);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in ChangeCabStateAsync", ex);
                return false;
            }
        }

        public async Task<List<CabDetails>> GetAvailableCabsAtLocationAsync(int locationId)
        {
            try
            {
                var allCabs = await _dataService.GetAllCabsAsync();
                return allCabs.Where(c => c.CurrentLocationId == locationId &&
                                        c.CurrentWorkState == WorkState.IDLE).ToList();
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in GetAvailableCabsAtLocationAsync", ex);
                return new List<CabDetails>();
            }
        }

        public async Task<CabDetails?> BookCabForTripAsync(int tripId, int fromLocationId)
        {
            try
            {
                var trip = await _dataService.GetTripByIdAsync(tripId);
                if (trip == null)
                    return null;

                // Get available cabs at the location
                var availableCabs = await GetAvailableCabsAtLocationAsync(fromLocationId);
                if (!availableCabs.Any())
                    return null;

                // Find the cab that has been idle the longest
                var selectedCab = availableCabs
                    .OrderByDescending(c => c.GetIdleDuration())
                    .First();

                // If multiple cabs have the same idle duration, randomly select one
                var longestIdleDuration = selectedCab.GetIdleDuration();
                var cabsWithSameIdleTime = availableCabs
                    .Where(c => c.GetIdleDuration() == longestIdleDuration)
                    .ToList();

                if (cabsWithSameIdleTime.Count > 1)
                {
                    var random = new Random();
                    selectedCab = cabsWithSameIdleTime[random.Next(cabsWithSameIdleTime.Count)];
                }

                // Assign the cab to the trip
                selectedCab.CurrentWorkState = WorkState.ON_TRIP;
                selectedCab.CurrentTripId = tripId;
                trip.AssignedCabId = selectedCab.Id;
                trip.StartTime = DateTime.UtcNow;

                // Update both cab and trip
                var cabUpdated = await _dataService.UpdateCabAsync(selectedCab);
                var tripUpdated = await _dataService.UpdateTripAsync(trip);

                return (cabUpdated && tripUpdated) ? selectedCab : null;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in BookCabForTripAsync", ex);
                return null;
            }
        }

        public async Task<bool> CompleteTripAsync(int tripId)
        {
            try
            {
                var trip = await _dataService.GetTripByIdAsync(tripId);
                if (trip == null || trip.AssignedCabId == null)
                    return false;

                var cab = await _dataService.GetCabByIdAsync(trip.AssignedCabId.Value);
                if (cab == null)
                    return false;

                // Update trip status
                trip.TripStatus = TripStatus.COMPLETED;
                trip.EndTime = DateTime.UtcNow;

                // Update cab status
                cab.CurrentWorkState = WorkState.IDLE;
                cab.IdleStartTime = DateTime.UtcNow;
                cab.CurrentTripId = null;
                cab.ComppletedTrips.Add(tripId);

                // Update both records
                var tripUpdated = await _dataService.UpdateTripAsync(trip);
                var cabUpdated = await _dataService.UpdateCabAsync(cab);

                return tripUpdated && cabUpdated;
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Exception occurred in CompleteTripAsync", ex);
                return false;
            }
        }
    }
}
