using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TopDown.Web
{
	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			routes.MapRoute("DefaultDrop", "{what}/{controller}/{action}", new { controller = "Home", action = "BroadGlobalActive", what = "Drop" });
			routes.MapRoute("DefaultKeep", "{what}/{controller}/{action}", new { controller = "Home", action = "BroadGlobalActive", what = "Keep" });
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
		}
	}
}