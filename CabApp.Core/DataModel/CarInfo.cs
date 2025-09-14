using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.DataModel
{
    public class CarInfo
    {
        public CarInfo() { }
        public int CarId { get; set; }
        public string ManufactureName { get; set; } = string.Empty;
        public string ModelName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int ManfactureYear { get; set; }
        public int KmDriven { get; set; }

    }
}
