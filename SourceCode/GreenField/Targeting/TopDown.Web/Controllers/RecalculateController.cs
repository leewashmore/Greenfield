using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TopDown.Core;
using Aims.Expressions;

namespace TopDown.Web.Controllers
{
    [OutputCache(CacheProfile = "NoCache")]
    public class RecalculateController : ControllerBase
    {
        [HttpPost]
        public ActionResult Breakdown(String what, String bptAsJson)
        {
			return this.WatchAndWrap(delegate
			{
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(base.ShouldDrop(what));
				var recalculatedBreakdownJson = facade.RecalculateBreakdown(bptAsJson, ticket);
				return recalculatedBreakdownJson;
			});
        }

        [HttpPost]
		public ActionResult PstModel(String what, String pstAsJson)
        {
			return this.WatchAndWrap(delegate
			{
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(base.ShouldDrop(what));
                var recalculatedPstCompositionAsJson = facade.RecalculatePstModel(pstAsJson, ticket);
				return recalculatedPstCompositionAsJson;
			});
        }

		[HttpPost]
		public ActionResult BpstModel(String what, String bpstModelAsJson)
		{
			return this.WatchAndWrap(delegate
			{
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(base.ShouldDrop(what));
                var recalculatedBpstModelAsJson = facade.RecalculateBpstModel(bpstModelAsJson, BenchmarkDate, ticket);
				return recalculatedBpstModelAsJson;
			});
		}
    }
}
