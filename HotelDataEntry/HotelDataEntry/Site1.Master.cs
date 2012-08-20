using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class Site1 : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!ReferenceEquals(Session["LoginSession"], "True"))
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                lbUsername.Text = Session["UserSession"].ToString();
            }
        }
    }
}