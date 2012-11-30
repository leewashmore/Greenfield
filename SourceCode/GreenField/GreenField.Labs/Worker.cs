using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenField.Labs
{
    public class Worker
    {
        private IDimensionEntitiesFactory entitiesFactory;

        public Worker(IDimensionEntitiesFactory entitiesFactory)
        {
            this.entitiesFactory = entitiesFactory;
        }

        public List<DateTime> GetMonthEnds()
        {
            var entities = this.entitiesFactory.CreateEntities();
            //var q = from t in entities.GF_PERF_TOPLEVELMONTH
            //        select t;

            var q = 
               /* (entities.GF_PERF_TOPLEVELMONTH.Select(g => new {g.TO_DATE})).ToList()
                .Select(x => DateTime.ParseExact(x.TO_DATE, "dd-MM-yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-us")))
                .Distinct()
                .OrderByDescending(x => x)
                .Take(12);*/

                (entities.GF_PERF_TOPLEVELMONTH.Select(g => new { g.TO_DATE })).ToList()
                .Select(x => DateTime.ParseExact(x.TO_DATE, "dd-MM-yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-us")))
                .Distinct()
                .OrderByDescending(x => x)
                .Take(12);


            return q.ToList();
            //return new DateTime[] {new DateTime(2012, 11, 7)}.ToList();
        }
    }
}
