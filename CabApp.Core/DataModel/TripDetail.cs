using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.DataModel
{
    public class TripDetail
    {
        public TripDetail() { }
        public int Id { get; set; }
        public LocationDetail FromLocation { get; set; } = new LocationDetail();
        public LocationDetail ToLocation { get; set; } = new LocationDetail();
        public TripStatus TripStatus { get; set; }
    }
}
