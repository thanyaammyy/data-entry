using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI.WebControls;
using HotelDataEntryLib;
using HotelDataEntryLib.Helper;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class DataEntry : System.Web.UI.Page
    {
        public string MonthYear;
        public int UserId ;
        protected void Page_Load(object sender, EventArgs e)
        {
            //Report
            Session["monthly"] = null;
            Session["property"] = null;
            Session["dateFrom"] = null;
            Session["dateTo"] = null;
            Session["monthly"] = null;
            Session["property2"] = null;
            Session["monthlyDate"] = null;
            Session["IsMonthly"] = null;


            if (!IsPostBack)
            {
                if (Session["propertyId"] == null || Session["MonthYear"]==null) return;
                ShowData(Convert.ToInt32(Session["propertyId"]), Session["MonthYear"].ToString());
            }
        }

        protected void btnCreateForm_Click(object sender, EventArgs e)
        {
            var propertyId = ddlCompany.SelectedValue;
            MonthYear = hiddenMonthYear.Value;
            if (string.IsNullOrEmpty(propertyId) ||  string.IsNullOrEmpty(MonthYear))
                return;
            Session["propertyId"] = propertyId;
            Session["MonthYear"] = MonthYear;
            ShowData(Convert.ToInt32(Session["propertyId"]),  Session["MonthYear"].ToString());
        }

        private void ShowData(int propertyId,  string my)
        {
            if (string.IsNullOrEmpty(my) || propertyId <= 0 )
            {
                lbError.Visible = true;
                lbCalendar.Visible = true;
                lbCompany.Visible = true;
            }
            else
            {
                lbError.Visible = false;
                lbCalendar.Visible = false;
                lbCompany.Visible = false;
                //divJqgrid.Attributes["style"] = "";
                //var hotelEntry = new HotelEntry()
                //{
                //    PropertyId = propertyId,
                //    DataEntrySubTypeId = dataEntrySubTypeId,
                //    MonthYear = my
                //};

                //if (HotelEntryHelper.ExistMothYear(hotelEntry))
                //{
                //    var exsitEntry = HotelEntryHelper.GetHotelEntry(hotelEntry);
                //    BindDataEntryJqgrid(exsitEntry);
                //}
                //else
                //{
                //    var newEntry = HotelEntryHelper.AddHotelEntryListByMonthYear(hotelEntry);
                //    DataEntryHelper.AddDataEntryListByMonthYear(newEntry);
                //    BindDataEntryJqgrid(newEntry);
                //}
            }
        }
        //private void BindDataEntryJqgrid(HotelEntry hotelEntry)
        //{
        //    var dataEntryList = DataEntryHelper.ListDataEntryByMonthYear(hotelEntry);
        //    JqGridDataEntry.DataSource = dataEntryList;
        //    CalculateTotal(dataEntryList);
        //    JqGridDataEntry.DataBind();
        //}
        //protected void JqGridDataEntry_RowEditing(object sender, JQGridRowEditEventArgs e)
        //{
        //    var dataEntryId = e.RowKey;
        //    var hotelEntryId = e.RowData["HotelEntryId"]==""?0:Convert.ToInt32(e.RowData["HotelEntryId"]);
        //    var actualData = string.IsNullOrEmpty(e.RowData["ActualData"]) ? 0.00 : float.Parse(e.RowData["ActualData"]);
        //    var budget = string.IsNullOrEmpty(e.RowData["Budget"]) ? 0.00 : float.Parse(e.RowData["Budget"]);
        //    var dataEntry = new HotelDataEntryLib.DataEntry()
        //        {
        //            DataEntryId = Convert.ToInt32(dataEntryId),
        //            ActualData =actualData,
        //            Budget = budget,
        //        };
        //    DataEntryHelper.UpdateDataEntry(dataEntry);
        //    var hotelEntry = new HotelEntry()
        //                         {
        //                             HotelEntryId = hotelEntryId
        //                         };
        //    BindDataEntryJqgrid(hotelEntry);
        //}

        //protected void CalculateTotal()//List<HotelDataEntryLib.DataEntry> listDataEntry)
        //{
        //    var actualTotal = 0.00;       
        //    var budgetTotal = 0.00;
        //    //foreach (var dataEntry in listDataEntry)
        //    //{
        //    //    var actualValue = dataEntry.ActualData;
        //    //    actualTotal += actualValue;

        //    //    var budgetValue = dataEntry.Budget;
        //    //    budgetTotal += budgetValue;
        //    //}

        //    JqGridDataEntry.Columns.FromDataField("ActualData").FooterValue = actualTotal.ToString("#,##0.00");
        //    JqGridDataEntry.Columns.FromDataField("Budget").FooterValue = budgetTotal.ToString("#,##0.00");
        //    JqGridDataEntry.Columns.FromDataField("PositionDate").FooterValue = "Total";
        //}

        //protected void ddlMenu_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    var selectedValue = ddlMenu.SelectedValue;
        //    MonthYear = hiddenMonthYear.Value;
        //    Session["MonthYear"] = MonthYear;
        //    divJqgrid.Attributes["style"] = "display:none";
        //    if (Convert.ToInt32(selectedValue) < 4)
        //    {
        //        ddlSubMenu.Enabled = true;
        //        if (((DropDownList)sender).SelectedValue != "")
        //        {
        //            Session["DataEntryTypeId"] = selectedValue;
        //        }
        //        updateRevenuePanel.Update();
        //    }
        //    else
        //    {
        //        Session["dataEntrySubTypeId"] = 7;
        //        ddlSubMenu.SelectedIndex = 0;
        //        ddlSubMenu.Enabled = false;
        //    }
        //}
        //protected void ddlSubMenu_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    MonthYear = hiddenMonthYear.Value;
        //    Session["MonthYear"] = MonthYear;
        //    divJqgrid.Attributes["style"] = "display:none";
        //}

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            MonthYear = hiddenMonthYear.Value;
            Session["MonthYear"] = MonthYear;
            divJqgrid.Attributes["style"] = "display:none";
            var selectedValue = ddlCompany.SelectedValue;
            if (((DropDownList)sender).SelectedValue != "")
            {
                var property = Convert.ToInt32(selectedValue);
                if (property != 0)
                {
                    var curr = PropertyHelper.GetProperty(property);
                    var currency = CurrencyHelper.GetCurrency(curr.CurrencyId);
                    lbCurerncy.Text = currency.CurrencyCode;
                }
                else
                {
                    lbCurerncy.Text = "";
                }
            }
        }
    }
}