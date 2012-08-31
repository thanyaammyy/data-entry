using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class User : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var maincompany = Request.QueryString["companyid"];
            if(!Page.IsPostBack)
            {
                if (JqgridUser.AjaxCallBackMode == AjaxCallBackMode.RequestData)
                {
                    JqgridUserBinding();
                }
            }

            if (!string.IsNullOrEmpty(maincompany))
            {
                var companyid = Convert.ToInt32(maincompany);
                Response.Clear();
                Response.Write(PropertyHelper.ListAlterCompany(companyid));
                try
                {
                    Response.End();
                }
                catch (Exception exception)
                {

                }
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