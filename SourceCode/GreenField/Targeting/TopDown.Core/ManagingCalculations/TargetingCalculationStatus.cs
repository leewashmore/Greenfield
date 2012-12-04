using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TopDown.Core.ManagingCalculations
{
    public enum TargetingCalculationStatus
    {
        Queued = 0,
        Finished = 1,
        // Failed, // <---- there is no such thing as a failed calculation because we don't know how to handle them, more specifically we don't know how to put a calculation in a failed state reliably, becuase in situation when the failure happens due to loosing the connection to the database we have no means to convey this back to the database because of the same lost connection problem
        // Started, // <---- there is no such thing either because of the same reson stated above
    }
}
