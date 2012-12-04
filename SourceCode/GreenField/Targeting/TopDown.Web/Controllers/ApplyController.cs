using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using TopDown.Core;
using Aims.Expressions;

namespace TopDown.Web.Controllers
{
    [OutputCache(CacheProfile = "NoCache")]
    public class ApplyController : ControllerBase
    {
		[HttpPost]
		public ActionResult PstModel(String what, String pstAsJson)
        {
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(shouldDrop);
				var issuesAsJson = facade.TryApplyPstModel(pstAsJson, Username, ticket);
				return issuesAsJson;
			});
        }

		[HttpPost]
		public ActionResult Bpt(String what, String bptAsJson)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(shouldDrop);
				var issuesAsJson = facade.TryApplyBptModel(bptAsJson, Username, ticket);
				return issuesAsJson;
			});
		}

        [HttpPost]
        public ActionResult Bpst(String what, String bpstAsJson)
        {
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(shouldDrop);
				var issuesAsJson = facade.TryApplyBpstModel(bpstAsJson, BenchmarkDate, Username, ticket);
				return issuesAsJson;
			});
        }
    }
}
