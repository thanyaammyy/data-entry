using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using HotelDataEntryLib;

namespace HotelDataEntry
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["rPropertyId"] == null || Session["MonthYear"] == null) return;
            var str = Session["MonthYear"].ToString().Split('/');
            if (string.IsNullOrEmpty(str[1])) return;
            var year = Convert.ToInt32(str[1]);
            var propertyId = Convert.ToInt32(Session["rPropertyId"]);
            var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(propertyId);
            lbProperty.Text = property.PropertyName;
            lbYear.Text = year.ToString();

            var hotelBudget = new HotelDataEntryLib.HotelBudget
                                  {
                                        Year = year,
                                        PropertyId = propertyId
                                  };
            var reportBudget = HotelDataEntryLib.Page.ReportsHelper.BudgetReport(hotelBudget);
            var hotelRevenue = HotelDataEntryLib.Page.HotelRevenueHelper.GetHotelRevenueList(hotelBudget);
            var report = CalculateReport(reportBudget, hotelRevenue);
            BindReportGrid(report);
        }

        private IList<HotelDataEntryLib.Helper.Reports> CalculateReport(IList<HotelDataEntryLib.Helper.Reports> report, IList<HotelRevenue> hotelRevenue  )
        {
            for (var k = 0; k < report.Count(); k++)
            {
                foreach (var t in hotelRevenue)
                {
                    if (report[k].MonthYear.Equals(t.Month + "/" + t.Year))
                    {
                        var listRevenueEntry =
                            HotelDataEntryLib.Page.RevenueHelper.GetRevenueEntry(t.HotelRevenueId);
                        var actualFB = 0.00;
                        var actualRoom = 0.00;
                        var actualSpa = 0.00;
                        var actualOthers = 0.00;
                        foreach (var t1 in listRevenueEntry)
                        {
                            actualFB += t1.FBRevenue;
                            actualRoom += t1.RoomRevenue;
                            actualSpa += t1.SpaRevenue;
                            actualOthers += t1.Others;
                        }
                        report[k].FBActual = actualFB;
                        report[k].RoomActual = actualRoom;
                        report[k].SpaActual = actualSpa;
                        report[k].OtherActual = actualOthers;
                    }
                }
            }
            return report;
        }

        private void BindReportGrid(IList<HotelDataEntryLib.Helper.Reports> report)
        {
            JqGridReport.DataSource = report;
            CalculateTotal(report);
            JqGridReport.DataBind();
        }

        protected void CalculateTotal(IList<HotelDataEntryLib.Helper.Reports> reports)
        {
            var totalRoomActual = 0.00;
            var totalRoomBudget = 0.00;
            var totalFBActual = 0.00;
            var totalFBBudget = 0.00;
            var toatlSpaActual = 0.00;
            var toatlSpaBudget = 0.00;
            var totalOtherActual = 0.00;
            var totalOtherBudget = 0.00;

            foreach (var report in reports)
            {
                var roomActual = report.RoomActual;
                totalRoomActual += roomActual;

                var roomBudget = report.RoomBudget;
                totalRoomBudget += roomBudget;

                var fbActual = report.FBActual;
                totalFBActual += fbActual;

                var fbBudget = report.FBBudget;
                totalFBBudget += fbBudget;

                var spaActual = report.SpaActual;
                toatlSpaActual += spaActual;

                var spaBudget = report.SpaBudget;
                toatlSpaBudget += spaBudget;

                var otherActual = report.OtherActual;
                totalOtherActual += otherActual;

                var otherBudget = report.OtherBudget;
                totalOtherBudget += otherBudget;
            }

            JqGridReport.Columns.FromDataField("RoomActual").FooterValue = totalRoomActual.ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("RoomBudget").FooterValue = totalRoomBudget.ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("FBActual").FooterValue = totalFBActual.ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("FBBudget").FooterValue = totalFBBudget.ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("SpaActual").FooterValue = toatlSpaActual.ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("SpaBudget").FooterValue = toatlSpaBudget.ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("OtherActual").FooterValue = totalOtherActual.ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("OtherBudget").FooterValue = totalOtherBudget.ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("MonthYear").FooterValue = "Total";
        }
    }
}