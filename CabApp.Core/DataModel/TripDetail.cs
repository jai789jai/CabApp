using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.DataModel
{
    public class TripDetail
    {
        public TripDetail() 
        { 
            TripStatus = TripStatus.IN_PROGRESS;
            BookingTime = DateTime.UtcNow;
        }
        
        public int Id { get; set; }
        public LocationDetail FromLocation { get; set; } = new LocationDetail();
        public LocationDetail ToLocation { get; set; } = new LocationDetail();
        public TripStatus TripStatus { get; set; }
        
        // New properties for cab assignment and booking
        public int? AssignedCabId { get; set; } // Null if not assigned yet
        public DateTime BookingTime { get; set; }
        public DateTime? StartTime { get; set; } // When trip actually started
        public DateTime? EndTime { get; set; } // When trip completed
    }
}
