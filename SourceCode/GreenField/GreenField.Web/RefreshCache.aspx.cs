using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GreenField.Web.Helpers;

namespace GreenField.Web
{
    public partial class RefreshCache : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            new DefaultCacheProvider().Get("sdf");
        }
    }
}