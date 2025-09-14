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
        public int CarId { get; set; }
        public WorkState CurrentWorkState { get; set; }
        public int TotalTripsCount { get { return ComppletedTrips.Count; } set { /* Ignore setter, this is computed */ } }
        public List<int> ComppletedTrips { get; set; } = new List<int>(); 
        public int DriverId { get; set; }
    }
}
