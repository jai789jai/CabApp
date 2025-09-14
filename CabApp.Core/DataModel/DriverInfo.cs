using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.DataModel
{
    public class DriverInfo: EmployeeInfo
    {
        public DriverInfo() { }
        public int LicenseNumber { get; set; }
        public int KmDriven { get; set; }
    }
}
