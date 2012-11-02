using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using HotelDataEntryLib;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class Budget : System.Web.UI.Page
    {
        public string Year;
        public int UserId;
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

            //Revenue
            Session["rPropertyId"] = null;
            Session["MonthYear"] = null;

            if (!IsPostBack)
            {
                if (Session["bPropertyId"] == null || Session["year"] == null) return;
                ShowData(Convert.ToInt32(Session["bPropertyId"]), Session["year"].ToString());
            }
        }
        protected void btnCreateForm_Click(object sender, EventArgs e)
        {
            var propertyId = ddlCompany.SelectedValue;
            Year = hiddenMonthYear.Value;
            Session["bPropertyId"] = propertyId;
            Session["year"] = Year;
            ShowData(Convert.ToInt32(Session["bPropertyId"]), Session["year"].ToString());
        }

        private void ShowData(int propertyId, string y)
        {
            if (string.IsNullOrEmpty(y) || propertyId <= 0)
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
                var hotelEntry = new HotelDataEntryLib.HotelBudget()
                {
                    PropertyId = propertyId,
                    Year = Convert.ToInt32(y)
                };

                if (HotelBudgetHelper.ExistYear(hotelEntry))
                {
                    var exsitEntry = HotelBudgetHelper.GetHotelEntry(hotelEntry);
                    BindBudgetEntryJqgrid(exsitEntry);
                }
                else
                {
                    var newEntry = HotelBudgetHelper.AddHotelEntryListByYear(hotelEntry);
                    BudgetHelper.AddBudgetEntryListByYear(newEntry);
                    BindBudgetEntryJqgrid(newEntry);
                }
            }
        }
        private void BindBudgetEntryJqgrid(HotelDataEntryLib.HotelBudget hotelEntry)
        {
            var userPermission = Session["permission"].ToString();
            var dataEntryList = BudgetHelper.ListBudgetEntryByYear(hotelEntry);
            JqGridBudgetEntry.DataSource = dataEntryList;
            CalculateTotal(dataEntryList);
            JqGridBudgetEntry.DataBind();
            if (!string.IsNullOrEmpty(userPermission))
            {
                if (Convert.ToInt32(userPermission) < 2)
                {
                    JqGridBudgetEntry.ToolBarSettings.ShowEditButton = false;

                }
            }
        }
        protected void JqGridDataEntry_RowEditing(object sender, JQGridRowEditEventArgs e)
        {
            var budgetEntryId = e.RowKey;
            var hotelEntryId = e.RowData["HotelEntryId"] == "" ? 0 : Convert.ToInt32(e.RowData["HotelEntryId"]);
            var occupiedRoom = string.IsNullOrEmpty(e.RowData["OccupiedRoom"]) ? 0: float.Parse(e.RowData["OccupiedRoom"]);
            var roomRevenue = string.IsNullOrEmpty(e.RowData["TotalRoomRevenues"]) ? 0.00 : float.Parse(e.RowData["TotalRoomRevenues"]);
            var food = string.IsNullOrEmpty(e.RowData["Food"]) ? 0.00 : float.Parse(e.RowData["Food"]);
            var beverage = string.IsNullOrEmpty(e.RowData["Beverage"]) ? 0.00 : float.Parse(e.RowData["Beverage"]);
            var service = string.IsNullOrEmpty(e.RowData["Service"]) ? 0.00 : float.Parse(e.RowData["Service"]);
            var spa = string.IsNullOrEmpty(e.RowData["SpaProduct"]) ? 0.00 : float.Parse(e.RowData["SpaProduct"]);
            var others = string.IsNullOrEmpty(e.RowData["Others"]) ? 0.00 : float.Parse(e.RowData["Others"]);
            var revenueEntry = new BudgetEntry()
            {
                BudgetId  = Convert.ToInt32(budgetEntryId),
                HotelBudgetId = hotelEntryId,
                OccupiedRoom = occupiedRoom,
                TotalRoomRevenues = roomRevenue,
                Food = food,
                Beverage = beverage,
                Service = service,
                SpaProduct = spa,
                Others = others,
                Total = roomRevenue + food + beverage + service + spa + others
            };
            BudgetHelper.UpdateBudgetEntry(revenueEntry);
        }

        protected void CalculateTotal(List<HotelDataEntryLib.BudgetEntry> listRevenueEntry)
        {
            var occupiedRoomTotal = 0;
            var roomRevenuesTotal = 0.00;
            var foodTotal = 0.00;
            var beverageTotal = 0.00;
            var serviceTotal = 0.00;
            var spaTotal = 0.00;
            var othersTotal = 0.00;
            var total = 0.00;
            foreach (var revenueEntry in listRevenueEntry)
            {
                var occupiedRoom = revenueEntry.OccupiedRoom;
                occupiedRoomTotal += Convert.ToInt32(occupiedRoom);

                var totalRoomRevenues = revenueEntry.TotalRoomRevenues;
                roomRevenuesTotal += totalRoomRevenues;

                var food = revenueEntry.Food;
                foodTotal += food;

                var beverage = revenueEntry.Beverage;
                beverageTotal += beverage;

                var service = revenueEntry.Service;
                serviceTotal += service;

                var spa = revenueEntry.SpaProduct;
                spaTotal += spa;

                var others = revenueEntry.Others;
                othersTotal += others;

                var tmd = revenueEntry.Total;
                total += tmd;
            }

            JqGridBudgetEntry.Columns.FromDataField("OccupiedRoom").FooterValue = occupiedRoomTotal.ToString();
            JqGridBudgetEntry.Columns.FromDataField("TotalRoomRevenues").FooterValue = roomRevenuesTotal.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("Food").FooterValue = foodTotal.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("Beverage").FooterValue = beverageTotal.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("Service").FooterValue = serviceTotal.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("SpaProduct").FooterValue = spaTotal.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("Others").FooterValue = othersTotal.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("Total").FooterValue = total.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("PositionMonth").FooterValue = "Total";
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            Year = hiddenMonthYear.Value;
            Session["year"] = Year;
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

        protected void CurrencyLabel_DataBound(object sender, EventArgs e)
        {
            var property = string.IsNullOrEmpty(ddlCompany.SelectedValue) ? 0 : Convert.ToInt32(ddlCompany.SelectedValue);
            if (property != 0)
            {
                var curr = PropertyHelper.GetProperty(property);
                var currency = CurrencyHelper.GetCurrency(curr.CurrencyId);
                lbCurerncy.Text = currency.CurrencyCode;
            }
        }
    }
}