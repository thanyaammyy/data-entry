using System;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class Company : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var companyList = CompanyHelper.ListCompany();
            JqgridCompany.DataSource = companyList;
            JqgridCompany.DataBind();
        }
    }
}