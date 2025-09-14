using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.DataModel
{
    public class CabDetails
    {
        public CabDetails() { }
        public int Id { get; set; }
        public CarInfo CarInfo { get; set; } = new CarInfo();   
        public WorkState CurrentWorkState { get; set; }
        public int TotalTripsCount { get { return ComppletedTrips.Count; } }
        public List<TripDetail> ComppletedTrips { get; set; } = new List<TripDetail>();
        public DriverInfo DriverInfo { get; set; } = new DriverInfo();
    }
}
