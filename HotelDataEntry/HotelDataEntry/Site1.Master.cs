using System;

namespace HotelDataEntry
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["LoginSession"] == null) Response.Redirect("~/Login.aspx");
        }
    }
}