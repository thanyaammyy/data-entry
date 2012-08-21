using System;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class Main : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var email = Request.QueryString["email"];
            if(UserHelper.GetUser(email)==0)
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "MyKey", "firstLogin();", true);
            }
        }
    }
}