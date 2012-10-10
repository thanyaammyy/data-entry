using System;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class Company : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //dataEntry
            Session["propertyId"] = null;
            Session["dataEntryTypeId"] = null;
            Session["MonthYear"] = null;

            //Report
            Session["monthly"] = null;
            Session["property"] = null;
            Session["dateFrom"] = null;
            Session["dateTo"] = null;
            Session["monthly"] = null;
            Session["property2"] = null;
            Session["monthlyDate"] = null;
            Session["IsMonthly"] = null;
            if(!Page.IsPostBack)
            {
                JqgridCompanyBinding();
            }
        }

        private void JqgridCompanyBinding()
        {
            var companyList = PropertyHelper.ListAllProperties();
            JqgridCompany.DataSource = companyList;
            JqgridCompany.DataBind();
        }

        protected void JqgridCompany_RowAdding(object sender, Trirand.Web.UI.WebControls.JQGridRowAddEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var brand = e.RowData["BrandName"];
            var currency = e.RowData["CurrencyCode"];
            if(!(string.IsNullOrEmpty(status)||string.IsNullOrEmpty(brand)||string.IsNullOrEmpty(currency)))
            {

                var property = new HotelDataEntryLib.Property()
                                   {
                                       PropertyCode = e.RowData["PropertyCode"],
                                       PropertyName = e.RowData["PropertyName"],
                                       Status = Convert.ToInt32(status),
                                       UpdateDateTime = DateTime.Now,
                                       //BrandId = Convert.ToInt32(brand),
                                       CurrencyId = Convert.ToInt32(currency)                                   
                                   };
                PropertyHelper.AddProperty(property);
            }
        }

        protected void JqgridCompany_RowEditing(object sender, Trirand.Web.UI.WebControls.JQGridRowEditEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var brand = e.RowData["BrandName"];
            var currency = e.RowData["CurrencyCode"];
            var id = e.RowKey;
            if (!(string.IsNullOrEmpty(status) || string.IsNullOrEmpty(brand) || string.IsNullOrEmpty(currency)||string.IsNullOrEmpty(id)))
            {
                var property = new HotelDataEntryLib.Property()
                                   {
                                       PropertyId = Convert.ToInt32(id),
                                       PropertyName = e.RowData["PropertyName"],
                                       PropertyCode = e.RowData["PropertyCode"],
                                       Status = Convert.ToInt32(status),
                                       UpdateDateTime = DateTime.Now,
                                       //BrandId = Convert.ToInt32(brand),
                                       CurrencyId = Convert.ToInt32(currency)
                                   };
                PropertyHelper.UpdateProperty(property);
            }
        }

        protected void JqgridCompany_RowDeleting(object sender, Trirand.Web.UI.WebControls.JQGridRowDeleteEventArgs e)
        {
            var id = e.RowKey;
            if (string.IsNullOrEmpty(id)) return;
            PropertyHelper.DeleteProperty(Convert.ToInt32(id));
        }
    }
}