using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using HotelDataEntryLib;
using Trirand.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HotelDataEntry
{
    public partial class Reports : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PropertyIdReport"] == null || Session["YearReport"] == null) return;
            var str = Session["YearReport"].ToString();
            if (string.IsNullOrEmpty(str)) return;
            var year = Convert.ToInt32(str);
            var propertyId = Convert.ToInt32(Session["PropertyIdReport"]);
            var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(propertyId);
            lbProperty.Text = property.PropertyName;
            lbYear.Text = year.ToString();
        }

        private void BindingJqGridReport(int year, int propertyId)
        {
            var hotelBudget = new HotelBudget
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
            JqGridReport.AppearanceSettings.ShrinkToFit = true;
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

        //protected void btnCSV_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        //{
        //    JqGridReport.ExportSettings.ExportDataRange = ExportDataRange.All;
        //    JqGridReport.ExportToCSV("report.csv");
        //}

        protected void btnExcel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridReport.ExportSettings.ExportDataRange = ExportDataRange.All;
            JqGridReport.ExportToExcel("report.xls");
        }

        protected void btnPDF_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridReport.ExportSettings.ExportDataRange = ExportDataRange.All;
            var dt = JqGridReport.GetExportData();
            ExportToPDF(dt);
        }

        private void ExportToPDF(DataTable dt)
        {
            var pdfDoc = new Document();
            var pdfStream = new MemoryStream();
            var pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream);

            pdfDoc.Open();//Open Document to write
            pdfDoc.NewPage();

            var font8 = FontFactory.GetFont("ARIAL", 7);

            var pdfTable = new PdfPTable(dt.Columns.Count);
            PdfPCell pdfPCell = null;

            //Add Header of the pdf table
            for (var column = 0; column < dt.Columns.Count; column++)
            {
                pdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].Caption, font8)));
                pdfTable.AddCell(pdfPCell);
            }

            //How add the data from datatable to pdf table
            for (var rows = 0; rows < dt.Rows.Count; rows++)
            {
                for (var column = 0; column < dt.Columns.Count; column++)
                {
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font8)));
                    pdfTable.AddCell(pdfPCell);
                }
            }

            pdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table            
            pdfDoc.Add(pdfTable); // add pdf table to the document
            pdfDoc.Close();
            pdfWriter.Close();


            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=report.pdf");
            Response.BinaryWrite(pdfStream.ToArray());
            Response.End();
        }

        protected void JqGridReport_Init(object sender, EventArgs e)
        {
            if (Session["YearReport"] == null || Session["PropertyIdReport"] == null) return;
            if (string.IsNullOrWhiteSpace(Session["YearReport"].ToString()) || string.IsNullOrEmpty(Session["PropertyIdReport"].ToString()))
                return;
            var year = Session["YearReport"].ToString();
            var propertyId = Session["PropertyIdReport"].ToString();
            BindingJqGridReport(Convert.ToInt32(year), Convert.ToInt32(propertyId));
        }
    }
}