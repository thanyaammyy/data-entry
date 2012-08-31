using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class User : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                JqgridUserBinding();
            }
        }

        private void JqgridUserBinding()
        {
            var currencyList = UserHelper.ListUser();
            JqgridUser.DataSource = currencyList;
            JqgridUser.DataBind();
        }

    }
}