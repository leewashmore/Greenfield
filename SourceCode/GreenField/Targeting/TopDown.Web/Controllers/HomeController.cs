using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace TopDown.Web.Controllers
{
    [OutputCache(CacheProfile = "NoCache")]
    public class HomeController : ControllerBase
    {
        public ActionResult BroadGlobalActive(String what)
        {
			var shouldDrop = base.ShouldDrop(what);
			this.ViewBag.ShouldDrop = shouldDrop;
            return this.View(shouldDrop);
        }

		public ActionResult BottomUp(String what)
		{
			var shouldDrop = base.ShouldDrop(what);
			this.ViewBag.ShouldDrop = shouldDrop;
			return this.View();
		}

        public ActionResult BasketTargets(String what)
        {
			var shouldDrop = base.ShouldDrop(what);
			this.ViewBag.ShouldDrop = shouldDrop;
			return this.View(shouldDrop);
        }

		public ActionResult Calculations(String what)
		{
			var shouldDrop = base.ShouldDrop(what);
			var monitor = this.CreateMonitor();
			var facade = this.CreateFacade(shouldDrop, monitor);
			var calculations = facade.GetCalculationChangesets(20);
			var model = new Models.RecalculatePageModel(calculations);
			return this.View(model);
		}

		public ActionResult Calculate(String what, Int32 calculationId)
		{
			var shouldDrop = base.ShouldDrop(what);
			var monitor = this.CreateMonitor();
			var facade = this.CreateFacade(shouldDrop, monitor);
			var targets = facade.Calculate(calculationId, true);
			var model = new Models.CalculationPageModel(targets);
			return this.View(model);
		}

        [HttpPost]
        public ActionResult Login(string username, string password, string redirectUrl)
        {
            if (Membership.ValidateUser(username, password))
            {
                FormsAuthentication.SetAuthCookie(username, false);
                if (Url.IsLocalUrl(redirectUrl) && redirectUrl.Length > 1 && redirectUrl.StartsWith("/")
                    && !redirectUrl.StartsWith("//") && !redirectUrl.StartsWith("/\\"))
                {
                    return Redirect(redirectUrl);
                }
                else
                {
                    return RedirectToAction("Bpt", "Home", new { shouldDrop = false });
                }
            }
            else
            {
                ModelState.AddModelError("userpass", "Incorrect username or password");
                return this.View();
            }
        }

        public ActionResult Login()
        {
            return this.View();
        }
    }
}
