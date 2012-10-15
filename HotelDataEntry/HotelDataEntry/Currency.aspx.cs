using System;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class Currency : System.Web.UI.Page
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

            if (!string.IsNullOrEmpty(Session["permission"].ToString()))
            {
                if (Convert.ToInt32(Session["permission"]) != 3)
                {
                    Response.Redirect("Login.aspx");
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

            if (!IsPostBack)
            {
                JqgridCurrencyBinding();
            }

            
        }

        private void JqgridCurrencyBinding()
        {
            var currencyList = CurrencyHelper.ListCurreny();
            JqgridCurrency.DataSource = currencyList;
            JqgridCurrency.DataBind();
        }

        protected void JqgridCurrency_RowAdding(object sender, Trirand.Web.UI.WebControls.JQGridRowAddEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var isBase = e.RowData["IsBaseLabel"];
            var rate = e.RowData["ConversionRate"];
            if(!(string.IsNullOrEmpty(status)||string.IsNullOrEmpty(isBase)||string.IsNullOrEmpty(rate)))
            {
                var currency = new HotelDataEntryLib.Currency()
                               {
                                   CurrencyName = e.RowData["CurrencyName"],
                                   CurrencyCode = e.RowData["CurrencyCode"],
                                   Status = Convert.ToInt32(status),
                                   ConversionRate = Convert.ToDouble(rate),
                                   IsBase = Convert.ToInt32(isBase)
                               };
                CurrencyHelper.AddCurrency(currency);
            }
        }

        protected void JqgridCurrency_RowDeleting(object sender, Trirand.Web.UI.WebControls.JQGridRowDeleteEventArgs e)
        {
            var currencyId = e.RowKey;
            if(!string.IsNullOrEmpty(currencyId))
            {
                CurrencyHelper.DeleteCurrency(Convert.ToInt32(currencyId));
            }
        }

        protected void JqgridCurrency_RowEditing(object sender, Trirand.Web.UI.WebControls.JQGridRowEditEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var isBase = e.RowData["IsBaseLabel"];
            var rate = e.RowData["ConversionRate"];
            var id = e.RowKey;
            if (!(string.IsNullOrEmpty(status) || string.IsNullOrEmpty(isBase) || string.IsNullOrEmpty(rate)||string.IsNullOrEmpty(id)))
            {
                var currency = new HotelDataEntryLib.Currency()
                {
                    CurrencyId = Convert.ToInt32(id),
                    CurrencyName = e.RowData["CurrencyName"],
                    CurrencyCode = e.RowData["CurrencyCode"],
                    Status = Convert.ToInt32(status),
                    ConversionRate = Convert.ToDouble(rate),
                    IsBase = Convert.ToInt32(isBase)
                };
                CurrencyHelper.UpdateCurrency(currency);
            }
        }
    }
}