using System;
using System.Web.UI.WebControls;
using HotelDataEntryLib;
using HotelDataEntryLib.Page;
namespace HotelDataEntry
{
    public partial class Report : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((DropDownList)sender).SelectedValue != "")
            {
                var propertyId = Convert.ToInt32(ddlCompany.SelectedValue);
                if (propertyId==0)
                {
                    displayCurrency.Attributes["style"] = "display:none";
                    return;
                }
                var curr = PropertyHelper.GetProperty(propertyId);
                var currency = CurrencyHelper.GetCurrency(curr.CurrencyId);
                lbCurerncy.Text = currency.CurrencyCode;
                displayCurrency.Attributes["style"] = "";
            }
        }

        protected void btnCreateForm_Click(object sender, EventArgs e)
        {
            var propertyId = ddlCompany.SelectedValue;
            var dateFrom = hiddenDateFrom.Value;
            var dateTo = hiddenDateTo.Value;
            if(string.IsNullOrEmpty(propertyId)||string.IsNullOrEmpty(dateFrom)||string.IsNullOrEmpty(dateTo))
            {
                lbCompany.Visible = true;
                lbDateFrom.Visible = true;
                lbDateTo.Visible = true;
                lbError.Visible = true;
            }
            else
            {
                lbCompany.Visible = false;
                lbDateFrom.Visible = false;
                lbDateTo.Visible = false;
                lbError.Visible = false;
            }
        }
    }
}