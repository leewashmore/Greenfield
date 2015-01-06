using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using GreenField.Web.Services;

namespace GreenField.Web
{
    public partial class OpenPdf : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            long presentationId = Int64.Parse( Request.QueryString["PresentationId"]);
            MeetingOperations mo = new MeetingOperations();
             Byte[] pdfData= mo.GenerateICPacketReport(presentationId);
             Response.ContentType = "application/pdf";
             Response.AddHeader("content-disposition", "inline; filename=" + "preview.pdf");
             Response.BinaryWrite(pdfData.ToArray());
             Response.Flush();
             Response.End();



        }
    }
}