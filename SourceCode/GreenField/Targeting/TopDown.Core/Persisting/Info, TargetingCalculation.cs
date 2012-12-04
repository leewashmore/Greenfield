using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TopDown.Core.Persisting
{
    public class TargetingCalculationInfo
    {
        [DebuggerStepThrough]
        public TargetingCalculationInfo()
        {
        }

        [DebuggerStepThrough]
        public TargetingCalculationInfo(Int32 id, Int32 statusCode, DateTime queuedOn, DateTime? startedOn, DateTime? finishedOn, String log)
        {
            this.Id = id;
            this.StatusCode = statusCode;
            this.QueuedOn = queuedOn;
            this.StartedOn = startedOn;
            this.FinishedOn = finishedOn;
            this.Log = log;
        }

        public Int32 Id { get; set; }
        public Int32 StatusCode { get; set; }
        public DateTime QueuedOn { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? FinishedOn { get; set; }
        public String Log { get; set; }
    }
}
