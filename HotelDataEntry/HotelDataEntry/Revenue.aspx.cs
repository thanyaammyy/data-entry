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

            //Budget
            Session["bPropertyId"] = null;
            Session["year"] = null;

            //First Load from menulink
            var fromMenu = Request.QueryString["key"];
            if (!string.IsNullOrEmpty(fromMenu))
            {
                Session["fromMenuRevenue"] = fromMenu;
                Response.Redirect("Revenue.aspx");
            }

            if (!IsPostBack)
            {
                if (Session["fromMenuRevenue"] == null)
                {
                    if (Session["rPropertyId"] == null || Session["MonthYear"] == null) return;
                    ShowData(Convert.ToInt32(Session["rPropertyId"]), Session["MonthYear"].ToString());
                }
                else
                {
                    Session["rPropertyId"] = null;
                    Session["MonthYear"] = null;
                    divJqgrid.Attributes["style"] = "display:none";
                    divReport.Attributes["style"] = "display:none";
                }
            }
        }

        protected void btnCreateForm_Click(object sender, EventArgs e)
        {
            Session["fromMenuRevenue"] = null;
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
                divReport.Attributes["style"] = "display:none";
            }
            else
            {
                lbError.Visible = false;
                lbCalendar.Visible = false;
                lbCompany.Visible = false;
                divReport.Attributes["style"] = "";
                divJqgrid.Attributes["style"] = "";
                var str = my.Split('/');
                if(!string.IsNullOrEmpty(str[0])&&!string.IsNullOrEmpty(str[1]))
                {
                    Session["PropertyIdReport"] = propertyId;//for reports.aspx property
                    Session["YearReport"] = Convert.ToInt32(str[1]);//for reports.aspx year
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
                            BudgetHelper.AddBudgetEntryListByYear(newBudgetEntry, Session["UserSession"].ToString());
                        }
                        var newEntry = HotelRevenueHelper.AddHotelEntryListByMonthYear(hotelEntry);
                        RevenueHelper.AddRevenueEntryListByMonthYear(newEntry, Session["UserSession"].ToString());
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
                if(Convert.ToInt32(userPermission)>=2)
                {
                    JqGridRevenueEntry.ToolBarSettings.ShowEditButton = true;
                }
            }
        }
        protected void JqGridDataEntry_RowEditing(object sender, JQGridRowEditEventArgs e)
        {
            var revenueEntryId = e.RowKey;
            var hotelEntryId = e.RowData["HotelEntryId"] == "" ? 0 : Convert.ToInt32(e.RowData["HotelEntryId"]);
            var roomRevenue = string.IsNullOrEmpty(e.RowData["RoomRevenue"]) ? 0.00 : Convert.ToDouble(e.RowData["RoomRevenue"]);
            var fbRevenue = string.IsNullOrEmpty(e.RowData["FBRevenue"]) ? 0.00 : Convert.ToDouble(e.RowData["FBRevenue"]);
            var spa = string.IsNullOrEmpty(e.RowData["SpaRevenue"]) ? 0.00 : Convert.ToDouble(e.RowData["SpaRevenue"]);
            var others = string.IsNullOrEmpty(e.RowData["Others"]) ? 0.00 : Convert.ToDouble(e.RowData["Others"]);
            var occupancyRoom=0.0;
            if (string.IsNullOrEmpty(e.RowData["OccupancyRoom"]))
            {
                occupancyRoom = 0;
            }
            else
            {
                var strOccupancy = "";
                strOccupancy = e.RowData["OccupancyRoom"].Contains("%") ? e.RowData["OccupancyRoom"].Remove(e.RowData["OccupancyRoom"].Length-1,1) : e.RowData["OccupancyRoom"];
                occupancyRoom = Convert.ToDouble(strOccupancy);
            }
            

            var revenueEntry = new RevenueEntry()
                {
                    RevenueId = Convert.ToInt32(revenueEntryId),
                    HotelRevenueId = hotelEntryId,
                    OccupancyRoom = occupancyRoom,
                    RoomRevenue = roomRevenue,
                    FBRevenue = fbRevenue,
                    SpaRevenue = spa,
                    Others = others,
                    UpdateUser = Session["UserSession"].ToString(),
                    Total = roomRevenue+fbRevenue+spa+others
                };
            RevenueHelper.UpdateRevenueEntry(revenueEntry);
        }

        protected void CalculateTotal(List<HotelDataEntryLib.Helper.Revenue> listRevenueEntry)
        {
            var roomRevenuesTotal = 0.00;
            var fbTotal = 0.00;
            var spaTotal = 0.00;
            var othersTotal = 0.00;
            var total = 0.00;
            var budgetTotal = 0.00;
            foreach (var revenueEntry in listRevenueEntry)
            {
                var totalRoomRevenues = revenueEntry.RoomRevenue;
                roomRevenuesTotal += totalRoomRevenues;

                var fb = revenueEntry.FBRevenue;
                fbTotal += fb;

                var spa = revenueEntry.SpaRevenue;
                spaTotal += spa;

                var others = revenueEntry.Others;
                othersTotal += others;

                var tmd = revenueEntry.Total;
                total += tmd;

                var budget = revenueEntry.Budget;
                budgetTotal += budget;
            }

            JqGridRevenueEntry.Columns.FromDataField("RoomRevenue").FooterValue = roomRevenuesTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("FBRevenue").FooterValue = fbTotal.ToString("#,##0.00");
            JqGridRevenueEntry.Columns.FromDataField("SpaRevenue").FooterValue = spaTotal.ToString("#,##0.00");
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
            divReport.Attributes["style"] = "display:none";
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

        protected void JqGridRevenueEntry_Init(object sender, EventArgs e)
        {
            if (Session["rPropertyId"] == null || Session["MonthYear"] == null) return;
            ShowData(Convert.ToInt32(Session["rPropertyId"]), Session["MonthYear"].ToString());
        }
    }
}