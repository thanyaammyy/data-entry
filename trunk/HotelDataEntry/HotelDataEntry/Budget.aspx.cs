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
            //Revenue
            Session["rPropertyId"] = null;
            Session["MonthYear"] = null;

            if (!IsPostBack)
            {
                divJqgrid.Attributes["style"] = "display:none";
                Session["bPropertyId"] = null;
                Session["year"] = null;
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
                    BudgetHelper.AddBudgetEntryListByYear(newEntry, Session["UserSession"].ToString());
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
            var occupancyRoom = string.IsNullOrEmpty(e.RowData["OccupancyRoom"]) ? 0 : float.Parse(e.RowData["OccupancyRoom"]);
            var roomBudget = string.IsNullOrEmpty(e.RowData["RoomBudget"]) ? 0.00 : float.Parse(e.RowData["RoomBudget"]);
            var fbBudget = string.IsNullOrEmpty(e.RowData["FBBudget"]) ? 0.00 : float.Parse(e.RowData["FBBudget"]);
            var spa = string.IsNullOrEmpty(e.RowData["SpaBudget"]) ? 0.00 : float.Parse(e.RowData["SpaBudget"]);
            var others = string.IsNullOrEmpty(e.RowData["Others"]) ? 0.00 : float.Parse(e.RowData["Others"]);
            var revenueEntry = new BudgetEntry()
            {
                BudgetId  = Convert.ToInt32(budgetEntryId),
                HotelBudgetId = hotelEntryId,
                OccupancyRoom = occupancyRoom,
                RoomBudget = roomBudget,
                FBBudget = fbBudget,
                SpaBudget = spa,
                Others = others,
                UpdateUser = Session["UserSession"].ToString(),
                Total = roomBudget + fbBudget + spa + others
            };
            BudgetHelper.UpdateBudgetEntry(revenueEntry);
        }

        protected void CalculateTotal(List<HotelDataEntryLib.BudgetEntry> listRevenueEntry)
        {
            var occupancyRoomTotal = 0;
            var roomTotal = 0.00;
            var fbTotal = 0.00;
            var spaTotal = 0.00;
            var othersTotal = 0.00;
            var total = 0.00;
            foreach (var revenueEntry in listRevenueEntry)
            {
                var occupiedRoom = revenueEntry.OccupancyRoom;
                occupancyRoomTotal += Convert.ToInt32(occupiedRoom);

                var roomRevenues = revenueEntry.RoomBudget;
                roomTotal += roomRevenues;

                var fb = revenueEntry.FBBudget;
                fbTotal += fb;

                var spa = revenueEntry.SpaBudget;
                spaTotal += spa;

                var others = revenueEntry.Others;
                othersTotal += others;

                var tmd = revenueEntry.Total;
                total += tmd;
            }

            JqGridBudgetEntry.Columns.FromDataField("OccupancyRoom").FooterValue = occupancyRoomTotal.ToString();
            JqGridBudgetEntry.Columns.FromDataField("RoomBudget").FooterValue = roomTotal.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("FBBudget").FooterValue = fbTotal.ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("SpaBudget").FooterValue = spaTotal.ToString("#,##0.00");
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
                CurrencyBinding(selectedValue);
            }
        }

        private void CurrencyBinding(string selectedValue)
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

        protected void JqGridBudgetEntry_Init(object sender, EventArgs e)
        {
            if (Session["bPropertyId"] == null || Session["year"] == null) return;
            ShowData(Convert.ToInt32(Session["bPropertyId"]), Session["year"].ToString());
        }
    }
}