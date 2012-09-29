using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BenchmarkNodeFinancials
{
    class GroupedBenchmarkNodeData
    {
       public String BenchmarkID{ get; set; }
       public String NodeName1 { get; set; }
       public String NodeID1 { get; set; }
       public String NodeName2 { get; set; }
       public String NodeID2 { get; set; }
       public int DataID { get; set; }
       public String PeriodType { get; set; }
       public int PeriodYear { get; set; }
       public String Currency { get; set; }
       public Decimal Amount { get; set; }
       public DateTime UpdateDate { get; set; }
    }
}
