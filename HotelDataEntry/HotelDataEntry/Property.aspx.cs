using System;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class Company : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Revenue
            Session["rPropertyId"] = null;
            Session["MonthYear"] = null;

            //Budget
            Session["bPropertyId"] = null;
            Session["year"] = null;

            if (Session["permission"] != null)
            {
                if (!string.IsNullOrEmpty(Session["permission"].ToString()))
                {
                    if (Convert.ToInt32(Session["permission"]) != 3)
                    {
                        Response.Redirect("Login.aspx");
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }


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
            var currency = e.RowData["CurrencyCode"];
            if(!(string.IsNullOrEmpty(status)||string.IsNullOrEmpty(currency)))
            {

                var property = new HotelDataEntryLib.Property()
                                   {
                                       PropertyCode = e.RowData["PropertyCode"],
                                       PropertyName = e.RowData["PropertyName"],
                                       Status = Convert.ToInt32(status),
                                       UpdateDateTime = DateTime.Now,
                                       CurrencyId = Convert.ToInt32(currency)                                   
                                   };
                PropertyHelper.AddProperty(property);
            }
        }

        protected void JqgridCompany_RowEditing(object sender, Trirand.Web.UI.WebControls.JQGridRowEditEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var currency = e.RowData["CurrencyCode"];
            var id = e.RowKey;
            if (!(string.IsNullOrEmpty(status) ||  string.IsNullOrEmpty(currency)||string.IsNullOrEmpty(id)))
            {
                var property = new HotelDataEntryLib.Property()
                                   {
                                       PropertyId = Convert.ToInt32(id),
                                       PropertyName = e.RowData["PropertyName"],
                                       PropertyCode = e.RowData["PropertyCode"],
                                       Status = Convert.ToInt32(status),
                                       UpdateDateTime = DateTime.Now,
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