using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CabApp.Core.DataModel
{
    public enum WorkState
    {
        [Description("Idle")]
        IDLE = 0,
        [Description("On Trip")]
        ON_TRIP = 1,
        [Description("Grounded")]
        GROUNDED = 3
    }

    public enum TripStatus
    {
        [Description("In Progress")]
        IN_PROGRESS = 0,
        [Description("Completed")]
        COMPLETED = 1,
        [Description("Cancelled")]
        CANCELLED = 2  
    }
}
