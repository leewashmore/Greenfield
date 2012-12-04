using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TopDown.Web.Controllers
{
    [OutputCache(CacheProfile = "NoCache")]
    public class DropController : ControllerBase
    {
        public ActionResult Everything(String what)
        {
			var shouldDrop = base.ShouldDrop(what);
			var monitor = this.CreateMonitor();
            var facade = this.CreateFacade(shouldDrop, monitor);
            facade.RepositoryManager.DropTargetingTypeRepository();
            facade.RepositoryManager.DropTargetingTypeGroupRepository();
            facade.RepositoryManager.DropCountryRepository();
            facade.RepositoryManager.DropBasketRespoitory();
            return this.Content("All repositories have been dropped.");
        }

        public ActionResult TargetingTypes(String what)
        {
			var shouldDrop = base.ShouldDrop(what);
			var monitor = this.CreateMonitor();
            var facade = this.CreateFacade(shouldDrop, monitor);
            facade.RepositoryManager.DropTargetingTypeRepository();
            return this.Content("Targeting type repository has been dropped.");
        }
        public ActionResult TargetingTypeGroups(String what)
        {
			var shouldDrop = base.ShouldDrop(what);
			var monitor = this.CreateMonitor();
            var facade = this.CreateFacade(shouldDrop,monitor);
            facade.RepositoryManager.DropTargetingTypeGroupRepository();
            return this.Content("Targeting type group repository has been dropped.");
        }

    }
}
