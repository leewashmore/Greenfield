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
	public class GetController : ControllerBase
	{
		[HttpPost]
		public ActionResult Targetings(String what)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
				var facade = this.CreateJsonFacade(shouldDrop);
				var json = facade.GetTargetingTypePortfolioPickerModel();
				return json;
			});
		}

		[HttpPost]
		public ActionResult Breakdown(String what, Int32 targetingId, String portfolioId)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(shouldDrop);
				var json = facade.GetBreakdown(targetingId, portfolioId, ticket);
				return json;
			});
		}

		[HttpPost]
		public ActionResult PstModel(String what, String portfolioId)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(shouldDrop);
				var json = facade.GetPstComposition(portfolioId, ticket);
				return json;
			});
		}

		public ActionResult BpstModel(String what, Int32 targetingTypeGroupId, Int32 basketId)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
                var ticket = new CalculationTicket();
				var facade = this.CreateJsonFacade(shouldDrop);
				var json = facade.GetBpstModel(
					targetingTypeGroupId,
					basketId,
                    ticket
				);
				return json;
			});
		}

		[HttpPost]
		public ActionResult Securities(String what, String securityNamePattern, Int32 atMost)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
				var facade = this.CreateJsonFacade(shouldDrop);
				var json = facade.GetSecurities(securityNamePattern, atMost);
				return json;
			});
		}

		[HttpPost]
		public ActionResult SecuritiesInBasket(String what, String securityNamePattern, Int32 atMost, Int32 basketId)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
				var facade = this.CreateJsonFacade(shouldDrop);
				var json = facade.GetSecurities(securityNamePattern, atMost, basketId);
				return json;
			});
		}

		public ActionResult BasketPicker(String what)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
				var facade = this.CreateJsonFacade(shouldDrop);
				var json = facade.GetBasketPicker(Username);
				return json;
			});
		}

		public ActionResult BottomUpPortfolios(String what)
		{
			return this.WatchAndWrap(delegate
			{
				var shouldDrop = base.ShouldDrop(what);
				var facade = this.CreateJsonFacade(shouldDrop);
				var json = facade.GetBottomUpPortfolios(Username);
				return json;
			});
		}
	}
}
