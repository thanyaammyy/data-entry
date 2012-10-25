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

            //Budget
            Session["bPropertyId"] = null;
            Session["year"] = null;

            if (!IsPostBack)
            {
                if (Session["rPropertyId"] == null || Session["MonthYear"]==null) return;
                ShowData(Convert.ToInt32(Session["rPropertyId"]), Session["MonthYear"].ToString());
            }
        }

        protected void btnCreateForm_Click(object sender, EventArgs e)
        {
            var propertyId = ddlCompany.SelectedValue;
            MonthYear = hiddenMonthYear.Value;
            Session["rPropertyId"] = propertyId;
            Session["MonthYear"] = MonthYear;
            ShowData(Convert.ToInt32(Session["rPropertyId"]), Session["MonthYear"].ToString());
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
                divJqgrid.Attributes["style"] = "";
                var str = my.Split('/');
                if(!string.IsNullOrEmpty(str[0])&&!string.IsNullOrEmpty(str[1]))
                {
                    var hotelEntry = new HotelDataEntryLib.HotelRevenue()
                    {
                        PropertyId = propertyId,
                        Month = Convert.ToInt32(str[0]),
                        Year = Convert.ToInt32(str[1])
                    };

                    if (HotelRevenueHelper.ExistMothYear(hotelEntry))
                    {
                        var exsitEntry = HotelRevenueHelper.GetHotelEntry(hotelEntry);
                        BindDataEntryJqgrid(exsitEntry);
                    }
                    else
                    {
                        var budgetEntry = new HotelBudget()
                        {
                             PropertyId = hotelEntry.PropertyId,
                             Year = hotelEntry.Year
                        };
                        if(!HotelBudgetHelper.ExistYear(budgetEntry))
                        {
                            var newBudgetEntry = HotelBudgetHelper.AddHotelEntryListByYear(budgetEntry);
                            BudgetHelper.AddBudgetEntryListByYear(newBudgetEntry);
                        }
                        var newEntry = HotelRevenueHelper.AddHotelEntryListByMonthYear(hotelEntry);
                        RevenueHelper.AddRevenueEntryListByMonthYear(newEntry);
                        BindDataEntryJqgrid(newEntry);
                    }
                }
            }
        }
        private void BindDataEntryJqgrid(HotelDataEntryLib.HotelRevenue hotelEntry)
        {
            var userPermission = Session["permission"].ToString();
            var dataEntryList = RevenueHelper.ListRevenueEntryByMonthYear(hotelEntry);
            JqGridRevenueEntry.DataSource = dataEntryList;
            CalculateTotal(dataEntryList);
            JqGridRevenueEntry.DataBind();
            if (!string.IsNullOrEmpty(userPermission))
            {
                if(Convert.ToInt32(userPermission)<2||Convert.ToInt32(userPermission)==3)
                {
                    JqGridRevenueEntry.ToolBarSettings.ShowEditButton = false;
                    
                }
            }
        }
        protected void JqGridDataEntry_RowEditing(object sender, JQGridRowEditEventArgs e)
        {
            var revenueEntryId = e.RowKey;
            var hotelEntryId = e.RowData["HotelEntryId"] == "" ? 0 : Convert.ToInt32(e.RowData["HotelEntryId"]);
            var occupiedRoom = string.IsNullOrEmpty(e.RowData["OccupiedRoom"]) ? 0.00 : float.Parse(e.RowData["OccupiedRoom"]);
            var roomRevenue = string.IsNullOrEmpty(e.RowData["TotalRoomRevenues"]) ? 0.00 : float.Parse(e.RowData["TotalRoomRevenues"]);
            var food = string.IsNullOrEmpty(e.RowData["Food"]) ? 0.00 : float.Parse(e.RowData["Food"]);
            var beverage = string.IsNullOrEmpty(e.RowData["Beverage"]) ? 0.00 : float.Parse(e.RowData["Beverage"]);
            var service = string.IsNullOrEmpty(e.RowData["Service"]) ? 0.00 : float.Parse(e.RowData["Service"]);
            var spa = string.IsNullOrEmpty(e.RowData["Spa"]) ? 0.00 : float.Parse(e.RowData["Spa"]);
            var others = string.IsNullOrEmpty(e.RowData["Others"]) ? 0.00 : float.Parse(e.RowData["Others"]);
            var revenueEntry = new RevenueEntry()
                {
                    RevenueId = Convert.ToInt32(revenueEntryId),
                    HotelRevenueId = hotelEntryId,
                    OccupiedRoom = occupiedRoom,
                    TotalRoomRevenues = roomRevenue,
                    Food = food,
                    Beverage = beverage,
                    Service = service,
                    Spa = spa,
                    Others = others,
                    Total = occupiedRoom+roomRevenue+food+beverage+service+spa+others
                };
            RevenueHelper.UpdateRevenueEntry(revenueEntry);
        }

        protected void CalculateTotal(List<HotelDataEntryLib.Helper.Revenue> listRevenueEntry)
        {
            var occupiedRoomTotal = 0.00;
            var roomRevenuesTotal = 0.00;
            var foodTotal = 0.00;
            var beverageTotal = 0.00;
            var serviceTotal = 0.00;
            var spaTotal = 0.00;
            var othersTotal = 0.00;
            var total = 0.00;
            var budgetTotal = 0.00;
            foreach (var revenueEntry in listRevenueEntry)
            {
                var occupiedRoom = revenueEntry.OccupiedRoom;
                occupiedRoomTotal += occupiedRoom;

                var totalRoomRevenues = revenueEntry.TotalRoomRevenues;
                roomRevenuesTotal += totalRoomRevenues;

                var food = revenueEntry.Food;
                foodTotal += food;

                var beverage = revenueEntry.Beverage;
                beverageTotal += beverage;

                var service = revenueEntry.Service;
                serviceTotal += service;

                var spa = revenueEntry.Spa;
                spaTotal += spa;

                var others = revenueEntry.Others;
                othersTotal += others;

                var tmd = revenueEntry.Total;
                total += tmd;

                var budget = revenueEntry.Budget;
                budgetTotal += budget;
            }

            JqGridRevenueEntry.Columns.FromDataField("OccupiedRoom").FooterValue = occupiedRoomTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("TotalRoomRevenues").FooterValue = roomRevenuesTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("Food").FooterValue = foodTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("Beverage").FooterValue = beverageTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("Service").FooterValue = serviceTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("Spa").FooterValue = spaTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("Others").FooterValue = othersTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("Total").FooterValue = total.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("Budget").FooterValue = budgetTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("PositionDate").FooterValue = "Total";
        }

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