using System;
using System.Collections.Generic;
using System.Data;
using HotelDataEntryLib;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class DataEntry : System.Web.UI.Page
    {
        public string MonthYear;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["propertyId"] == null || Session["dataEntryTypeId"] == null || Session["MonthYear"]==null) return;
                ShowData(Convert.ToInt32(Session["propertyId"]), Convert.ToInt32(Session["dataEntryTypeId"]), Session["MonthYear"].ToString());
            }
        }

        protected void btnCreateForm_Click(object sender, EventArgs e)
        {
            var propertyId = ddlCompany.SelectedValue;
            var dataEntryTypeId = ddlMenu.SelectedValue;
            MonthYear = hiddenMonthYear.Value;
            if (string.IsNullOrEmpty(propertyId) || string.IsNullOrEmpty(dataEntryTypeId) || string.IsNullOrEmpty(MonthYear))
                return;
            Session["propertyId"] = propertyId;
            Session["dataEntryTypeId"] = dataEntryTypeId;
            Session["MonthYear"] = MonthYear;
            ShowData(Convert.ToInt32(Session["propertyId"]), Convert.ToInt32(Session["dataEntryTypeId"]), Session["MonthYear"].ToString());
        }

        private void ShowData(int propertyId, int dataEntryTypeId, string my)
        {
            if (string.IsNullOrEmpty(my) || propertyId <= 0 || dataEntryTypeId <= 0)
            {
                lbError.Visible = true;
                lbCalendar.Visible = true;
                lbCompany.Visible = true;
                lbMenu.Visible = true;
            }
            else
            {
                lbError.Visible = false;
                lbCalendar.Visible = false;
                lbCompany.Visible = false;
                lbMenu.Visible = false;
                divJqgrid.Attributes["style"] = "";
                var hotelEntry = new HotelEntry()
                {
                    PropertyId = propertyId,
                    DataEntryTypeId = dataEntryTypeId,
                    MonthYear = my
                };

                if (HotelEntryHelper.ExistMothYear(hotelEntry))
                {
                    var exsitEntry = HotelEntryHelper.GetHotelEntry(hotelEntry);
                    BindDataEntryJqgrid(exsitEntry);
                }
                else
                {
                    var newEntry = HotelEntryHelper.AddHotelEntryListByMonthYear(hotelEntry);
                    DataEntryHelper.AddDataEntryListByMonthYear(newEntry);
                    BindDataEntryJqgrid(newEntry);
                }
            }
        }
        private void BindDataEntryJqgrid(HotelEntry hotelEntry)
        {
            JqGridDataEntry.Visible = true;
            var dataEntryList = DataEntryHelper.ListDataEntryByMonthYear(hotelEntry);
            JqGridDataEntry.DataSource = dataEntryList;
            CalculateTotal(dataEntryList);
            JqGridDataEntry.DataBind();
        }
        protected void JqGridDataEntry_RowEditing(object sender, JQGridRowEditEventArgs e)
        {
            var dataEntryId = e.RowKey;
            var hotelEntryId = e.RowData["HotelEntryId"]==""?0:Convert.ToInt32(e.RowData["HotelEntryId"]);
            var actualData = e.RowData["ActualData"] == "" ? 0.00 : float.Parse(e.RowData["ActualData"]);
            var budget = e.RowData["Budget"] == "" ? 0.00 : float.Parse(e.RowData["Budget"]);
            var ytdActual = e.RowData["YTDActual"] == "" ? 0.00 : float.Parse(e.RowData["YTDActual"]);
            var ytdBudget = e.RowData["YTDBudget"] == "" ? 0.00 : float.Parse(e.RowData["YTDBudget"]);
            var dataEntry = new HotelDataEntryLib.DataEntry()
                {
                    DataEntryId = Convert.ToInt32(dataEntryId),
                    ActualData =actualData,
                    Budget = budget,
                    YTDActual = ytdActual,
                    YTDBudget = ytdBudget
                };
            DataEntryHelper.UpdateDataEntry(dataEntry);
            var hotelEntry = new HotelEntry()
                                 {
                                     HotelEntryId = hotelEntryId
                                 };
            BindDataEntryJqgrid(hotelEntry);
        }

        protected void CalculateTotal(List<HotelDataEntryLib.DataEntry> listDataEntry)
        {
            var actualTotal = 0.00;       
            var budgetTotal = 0.00;
            var ytdActualTotal = 0.00;
            var ytdBudgetTotal = 0.00;
            foreach (var dataEntry in listDataEntry)
            {
                var actualValue = dataEntry.ActualData;
                actualTotal += actualValue;

                var budgetValue = dataEntry.Budget;
                budgetTotal += budgetValue;

                var ytdActualValue = dataEntry.YTDActual ?? 0.00;
                ytdActualTotal += ytdActualValue;

                var ytdBudgetValue = dataEntry.YTDBudget ?? 0.00;
                ytdBudgetTotal += ytdBudgetValue;
            }

            JqGridDataEntry.Columns.FromDataField("ActualData").FooterValue = actualTotal.ToString("#,##0.00");
            JqGridDataEntry.Columns.FromDataField("Budget").FooterValue = budgetTotal.ToString("#,##0.00");
            JqGridDataEntry.Columns.FromDataField("YTDActual").FooterValue = ytdActualTotal.ToString("#,##0.00");
            JqGridDataEntry.Columns.FromDataField("YTDBudget").FooterValue = ytdBudgetTotal.ToString("#,##0.00");
            JqGridDataEntry.Columns.FromDataField("PositionDate").FooterValue = "Total";
        }
    }
}