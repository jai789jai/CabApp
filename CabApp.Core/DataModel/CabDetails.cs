using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.DataModel
{
    public class CabDetails
    {
        public CabDetails() 
        { 
            CurrentWorkState = WorkState.IDLE;
            LastIdleTime = DateTime.UtcNow;
        }
        
        public int Id { get; set; }
        public int CarId { get; set; }
        public WorkState CurrentWorkState { get; set; }
        public int TotalTripsCount { get { return ComppletedTrips.Count; } set { /* Ignore setter, this is computed */ } }
        public List<int> ComppletedTrips { get; set; } = new List<int>(); 
        public int DriverId { get; set; }
        
        // New properties for location and idle time tracking
        public int CurrentLocationId { get; set; }
        public DateTime LastIdleTime { get; set; }
        public int? CurrentTripId { get; set; } // Null when idle, contains trip ID when on trip
        
        // Helper method to calculate idle duration
        public TimeSpan GetIdleDuration()
        {
            if (CurrentWorkState == WorkState.IDLE)
            {
                return DateTime.UtcNow - LastIdleTime;
            }
            return TimeSpan.Zero;
        }
    }
}
