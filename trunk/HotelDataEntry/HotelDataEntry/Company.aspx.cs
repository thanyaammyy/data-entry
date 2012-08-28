using System;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class Company : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if(!Page.IsPostBack)
            {
                JqgridCompanyBinding();
            }
        }

        private void JqgridCompanyBinding()
        {
            var companyList = PropertyHelper.ListAllCompany();
            JqgridCompany.DataSource = companyList;
            JqgridCompany.DataBind();
        }
    }
}