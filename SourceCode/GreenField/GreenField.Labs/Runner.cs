using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace GreenField.Labs
{
    public class Runner
    {
        public void RunPlease()
        {
            var worker = new Worker(new TestDimnesionEntitiesFactory());
            var dates = worker.GetMonthEnds();
            foreach (var date in dates)
            {
                Trace.WriteLine(date);
            }
        }
    }
}
