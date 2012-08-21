using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class UserInfo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Session["propertyid"] = "1";
        }

        protected void ddlCompany_SelectedIndexChanged1(object sender, EventArgs e)
        {
            if (((DropDownList)sender).SelectedValue != "")
            {
                Session["propertyid"] = ddlCompany.SelectedValue;
            }
        }
    }
}