using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using HotelDataEntryLib;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HotelDataEntry
{
    public partial class Reports : System.Web.UI.Page
    {
        private string _propertyName;
        private string _year;
        private string _currency;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["PropertyIdReport"] == null || Session["YearReport"] == null)
            {
                Page.RegisterClientScriptBlock("closeIframeAdd",
                                               "<script type=\"text/javascript\" language=\"javascript\">parent.location.href = 'Login.aspx';</script>");
                return;
            }
            var str = Session["YearReport"].ToString();
            if (string.IsNullOrEmpty(str)) return;
            var year = Convert.ToInt32(str);
            var propertyId = Convert.ToInt32(Session["PropertyIdReport"]);
            var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(propertyId);
            _propertyName = property.PropertyName;
            _year = year.ToString();
            lbProperty.Text = _propertyName;
            lbYear.Text = _year;

            var curr = PropertyHelper.GetProperty(propertyId);
            var currency = CurrencyHelper.GetCurrency(curr.CurrencyId);
            _currency = currency.CurrencyCode;
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

        private IList<HotelDataEntryLib.Helper.Reports> CalculateReport(IList<HotelDataEntryLib.Helper.Reports> report,
                                                                        IList<HotelRevenue> hotelRevenue)
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
                        var actualOccupancyRoom = 0.00;
                        var dateNo = 0;
                        foreach (var t1 in listRevenueEntry)
                        {
                            if (t1.OccupancyRoom > 0) ++dateNo;
                            actualOccupancyRoom += t1.OccupancyRoom;
                            actualFB += t1.FBRevenue;
                            actualRoom += t1.RoomRevenue;
                            actualSpa += t1.SpaRevenue;
                            actualOthers += t1.Others;
                        }
                        report[k].OccupancyRoomActual = dateNo == 0 ? 0 : Math.Round((actualOccupancyRoom/dateNo), 2);
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
            AppendTotal(report);
            JqGridReport.DataBind();
            JqGridReport.AppearanceSettings.ShrinkToFit = true;
        }

        protected List<double> CalculateTotal(IList<HotelDataEntryLib.Helper.Reports> reports)
        {
            var list = new List<double>(5);
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

            list.Add(totalRoomActual);
            list.Add(totalRoomBudget);
            list.Add(totalFBActual);
            list.Add(totalFBBudget);
            list.Add(toatlSpaActual);
            list.Add(toatlSpaBudget);
            list.Add(totalOtherActual);
            list.Add(totalOtherBudget);
            return list;
        }

        protected void AppendTotal(IList<HotelDataEntryLib.Helper.Reports> reports)
        {
            var listTotal = CalculateTotal(reports);
            JqGridReport.Columns.FromDataField("RoomActual").FooterValue = listTotal[0].ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("RoomBudget").FooterValue = listTotal[1].ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("FBActual").FooterValue = listTotal[2].ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("FBBudget").FooterValue = listTotal[3].ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("SpaActual").FooterValue = listTotal[4].ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("SpaBudget").FooterValue = listTotal[5].ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("OtherActual").FooterValue = listTotal[6].ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("OtherBudget").FooterValue = listTotal[7].ToString("#,##0.00");
            JqGridReport.Columns.FromDataField("MonthYear").FooterValue = "Total";
        }

        protected void btnExcel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridReport.ExportSettings.ExportHeaders = true;
            JqGridReport.ExportSettings.ExportDataRange = ExportDataRange.All;

            var dt = JqGridReport.GetExportData();

            dt.Rows.Add("Total", "-", "-", JqGridReport.Columns.FromDataField("RoomActual").FooterValue,
                        JqGridReport.Columns.FromDataField("RoomBudget").FooterValue,
                        JqGridReport.Columns.FromDataField("FBActual").FooterValue,
                        JqGridReport.Columns.FromDataField("FBBudget").FooterValue,
                        JqGridReport.Columns.FromDataField("SpaActual").FooterValue,
                        JqGridReport.Columns.FromDataField("SpaBudget").FooterValue,
                        JqGridReport.Columns.FromDataField("OtherActual").FooterValue,
                        JqGridReport.Columns.FromDataField("OtherBudget").FooterValue);
            ExportToExcel(dt);
        }

        protected void btnPDF_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridReport.ExportSettings.ExportHeaders = true;
            JqGridReport.ExportSettings.ExportDataRange = ExportDataRange.All;

            var dt = JqGridReport.GetExportData();

            dt.Rows.Add("Total", "-", "-", JqGridReport.Columns.FromDataField("RoomActual").FooterValue,
                        JqGridReport.Columns.FromDataField("RoomBudget").FooterValue,
                        JqGridReport.Columns.FromDataField("FBActual").FooterValue,
                        JqGridReport.Columns.FromDataField("FBBudget").FooterValue,
                        JqGridReport.Columns.FromDataField("SpaActual").FooterValue,
                        JqGridReport.Columns.FromDataField("SpaBudget").FooterValue,
                        JqGridReport.Columns.FromDataField("OtherActual").FooterValue,
                        JqGridReport.Columns.FromDataField("OtherBudget").FooterValue);
            ExportToPDF(dt);
        }

        private void ExportToExcel(DataTable dt)
        {
            var attachment = "attachment; filename=" + _propertyName + " MTD " + _year + ".xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            var tab = "";
            Response.Write("[" + _currency + "] " + _propertyName + " MTD " + _year);
            Response.Write("\r\n");
            Response.Write("\r\n");
            foreach (DataColumn dc in dt.Columns)
            {
                if (dc.ColumnName.ToLower().Equals("actual") || dc.ColumnName.ToLower().Equals("budget"))
                {
                    Response.Write(tab + "Occupancy(%) " + dc.ColumnName);
                }
                else
                {
                    Response.Write(tab + dc.ColumnName);
                }
                tab = "\t";
            }
            Response.Write("\n");
            foreach (DataRow dr in dt.Rows)
            {
                tab = "";
                int i;
                for (i = 0; i < dt.Columns.Count; i++)
                {

                    Response.Write(tab + dr[i].ToString());
                    tab = "\t";
                }
                Response.Write("\n");
            }
            Response.Write("\n");
            Response.Write("Print Date: [" + DateTime.Now + "] ");
            Response.End();
        }

        private void ExportToPDF(DataTable dt)
        {
            var attachment = "attachment; filename=" + _propertyName + " MTD " + _year + ".pdf";
            var pdfDoc = new Document(PageSize.A4.Rotate());
            var pdfStream = new MemoryStream();
            var pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream);

            pdfDoc.Open(); //Open Document to write
            pdfDoc.AddSubject(_year);
            pdfDoc.AddTitle(_propertyName);

            pdfDoc.AddCreationDate();
            pdfDoc.NewPage();

            var fontH = FontFactory.GetFont("ARIAL", 9, Font.BOLD);
            var fontT = FontFactory.GetFont("ARIAL", 12, Font.BOLD);
            var font8 = FontFactory.GetFont("ARIAL", 8);
            var font8B = FontFactory.GetFont("ARIAL", 8, Font.BOLD);

            var preface = new Paragraph();
            var prefacedate = new Paragraph();

            // Lets write a big header
            preface.Add(new Paragraph("[" + _currency + "] " + _propertyName + " MTD " + _year, fontT));
            prefacedate.Add(new Paragraph("Print Date: [" + DateTime.Now + "] ", font8B));


            var pdfTable = new PdfPTable(dt.Columns.Count);
            pdfTable.HorizontalAlignment = 0;
            pdfTable.TotalWidth = 781f;
            pdfTable.LockedWidth = true;

            PdfPCell pdfPCell = null;



            //Add Header of the pdf table
            for (var column = 0; column < dt.Columns.Count; column++)
            {
                if (dt.Columns[column].Caption.ToLower().Equals("actual") ||
                    dt.Columns[column].Caption.ToLower().Equals("budget"))
                {
                    pdfPCell = new PdfPCell(new Phrase(new Chunk("Occupancy(%) " + dt.Columns[column].Caption, fontH)));
                }
                else
                {
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].Caption, fontH)));

                }
                pdfTable.AddCell(pdfPCell);
            }

            //How add the data from datatable to pdf table
            for (var rows = 0; rows < dt.Rows.Count; rows++)
            {
                for (var column = 0; column < dt.Columns.Count; column++)
                {
                    if (rows == dt.Rows.Count - 1)
                    {
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font8B)));
                    }
                    else
                    {
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font8)));

                    }
                    if (column != 0) pdfPCell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pdfTable.AddCell(pdfPCell);
                }
            }



            var widths = new float[] {55f, 75f, 75f, 72f, 72f, 72f, 72f, 72f, 72f, 72f, 72f};
            pdfTable.SetWidths(widths);
            pdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table            

            pdfDoc.SetMargins(5.0f, 5.0f, 40.0f, 0f);
            pdfDoc.Add(preface);
            pdfDoc.Add(pdfTable); // add pdf table to the document
            pdfDoc.Add(prefacedate);
            pdfDoc.Close();
            pdfWriter.Close();


            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", attachment);
            Response.BinaryWrite(pdfStream.ToArray());
            Response.End();
        }

        protected void JqGridReport_Init(object sender, EventArgs e)
        {
            if (Session["YearReport"] == null || Session["PropertyIdReport"] == null) return;
            if (string.IsNullOrWhiteSpace(Session["YearReport"].ToString()) ||
                string.IsNullOrEmpty(Session["PropertyIdReport"].ToString()))
                return;
            var year = Session["YearReport"].ToString();
            var propertyId = Session["PropertyIdReport"].ToString();
            BindingJqGridReport(Convert.ToInt32(year), Convert.ToInt32(propertyId));

            var username = Session["UserSession"].ToString();
            var user = UserHelper.GetUser(username);

            if (user.PropertyId == 15) //OHG ID
            {
                divExportAllData.Attributes["style"] = "";
            }
        }

        protected void btnExcelAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (Session["YearReport"] == null) return;
            var y = Convert.ToInt32(Session["YearReport"].ToString());
            var budget = HotelDataEntryLib.Page.BudgetHelper.GetAllPropertyByHotelBudget(y);
            var attachment = "attachment; filename=All Properties" + " MTD " + _year + ".xls";

            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";

            for (var i = 0; i < budget.Count; i++)
            {
                Response.Write("[" + budget[i].CurrencyCode + "] " + budget[i].PropertyName + " MTD " + _year);
                Response.Write("\r\n");
                Response.Write("\r\n");

                Response.Write("Month/Year\t");
                Response.Write("Occupancy(%) Actual\t");
                Response.Write("Occupancy(%) Budget\t");
                Response.Write("Room Actual\t");
                Response.Write("Room Budget\t");
                Response.Write("F & B Actual\t");
                Response.Write("F & B Budget\t");
                Response.Write("Spa Actual\t");
                Response.Write("Spa Budget\t");
                Response.Write("Others Actual\t");
                Response.Write("Others Budget\t");
                Response.Write("\n");
                var hotelBudget = new HotelBudget
                                      {
                                          Year = y,
                                          PropertyId = budget[i].PropertyId
                                      };
                var reportBudget = HotelDataEntryLib.Page.ReportsHelper.BudgetReport(hotelBudget);
                var hotelRevenue = HotelDataEntryLib.Page.HotelRevenueHelper.GetHotelRevenueList(hotelBudget);
                var report = CalculateReport(reportBudget, hotelRevenue);
                var total = CalculateTotal(report);
                for (var j = 0; j < report.Count; j++)
                {
                    Response.Write(report[j].MonthYear + "\t");
                    Response.Write(report[j].OccupancyRoomActual.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].OccupancyRoomBudget.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].RoomActual.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].RoomBudget.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].FBActual.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].FBBudget.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].SpaActual.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].SpaBudget.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].OtherActual.ToString("#,##0.00") + "\t");
                    Response.Write(report[j].OtherBudget.ToString("#,##0.00") + "\t");
                    Response.Write("\n");
                }
                Response.Write("Total" + "\t");
                Response.Write("-" + "\t");
                Response.Write("-" + "\t");
                Response.Write(total[0].ToString("#,##0.00") + "\t");
                Response.Write(total[1].ToString("#,##0.00") + "\t");
                Response.Write(total[2].ToString("#,##0.00") + "\t");
                Response.Write(total[3].ToString("#,##0.00") + "\t");
                Response.Write(total[4].ToString("#,##0.00") + "\t");
                Response.Write(total[5].ToString("#,##0.00") + "\t");
                Response.Write(total[6].ToString("#,##0.00") + "\t");
                Response.Write(total[7].ToString("#,##0.00") + "\t");
                Response.Write("\n");
                Response.Write("\n");
                Response.Write("\n");
            }

            Response.Write("\n");
            Response.Write("Print Date: [" + DateTime.Now + "] ");
            Response.Write("\r\n");
            Response.End();
        }

        protected void btnPDFAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            if (Session["YearReport"] == null) return;
            var y = Convert.ToInt32(Session["YearReport"].ToString());
            var budget = HotelDataEntryLib.Page.BudgetHelper.GetAllPropertyByHotelBudget(y);

            var attachment = "attachment; filename= All Properties" + " MTD " + _year + ".pdf";
            var pdfDoc = new Document(PageSize.A4.Rotate(), 30.0f, 5.0f, 40.0f, 0f);
            var pdfStream = new MemoryStream();
            var pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream);

            pdfDoc.Open(); //Open Document to write

            pdfDoc.NewPage();

            var fontH = FontFactory.GetFont("ARIAL", 9, Font.BOLD);
            var fontT = FontFactory.GetFont("ARIAL", 12, Font.BOLD);
            var font8 = FontFactory.GetFont("ARIAL", 8);
            var font8B = FontFactory.GetFont("ARIAL", 8, Font.BOLD);


            var prefacedate = new Paragraph {new Paragraph("Print Date: [" + DateTime.Now + "] ", font8B)};
            var widths = new float[] {55f, 75f, 75f, 72f, 72f, 72f, 72f, 72f, 72f, 72f, 72f};


            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            for (var i = 0; i < budget.Count; i++)
            {
                //Begin table
                var pdfTable = new PdfPTable(11);
                pdfTable.HorizontalAlignment = 0;
                pdfTable.TotalWidth = 781f;
                pdfTable.LockedWidth = true;
                pdfTable.SetWidths(widths);
                pdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table            

                var preface = new Paragraph();
                // Header
                preface.Add(new Paragraph("[" + budget[i].CurrencyCode + "] " + budget[i].PropertyName + " MTD " + y,
                                          fontT));
                pdfDoc.Add(preface);

                PdfPCell pdfPCell = null;

                //Add Header of the pdf table
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Month/Year", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Occupancy(%) Acctual", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Occupancy(%) Budget", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Room Actual", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Room Budget", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("F & B Actual", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("F & B Budget", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Spa Actual", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Spa Budget", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Others Actual", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Others Budget", fontH)));
                pdfTable.AddCell(pdfPCell);

                //How add the data from datatable to pdf table
                var hotelBudget = new HotelBudget
                                      {
                                          Year = y,
                                          PropertyId = budget[i].PropertyId
                                      };
                var reportBudget = HotelDataEntryLib.Page.ReportsHelper.BudgetReport(hotelBudget);
                var hotelRevenue = HotelDataEntryLib.Page.HotelRevenueHelper.GetHotelRevenueList(hotelBudget);
                var report = CalculateReport(reportBudget, hotelRevenue);
                var total = CalculateTotal(report);
                for (var j = 0; j < report.Count; j++)
                {
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].MonthYear, font8)))
                                   {HorizontalAlignment = Element.ALIGN_LEFT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell =
                        new PdfPCell(new Phrase(new Chunk(report[j].OccupancyRoomActual.ToString("#,##0.00"), font8)))
                            {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell =
                        new PdfPCell(new Phrase(new Chunk(report[j].OccupancyRoomBudget.ToString("#,##0.00"), font8)))
                            {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].RoomActual.ToString("#,##0.00"), font8)))
                                   {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].RoomBudget.ToString("#,##0.00"), font8)))
                                   {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].FBActual.ToString("#,##0.00"), font8)))
                                   {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].FBBudget.ToString("#,##0.00"), font8)))
                                   {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].SpaActual.ToString("#,##0.00"), font8)))
                                   {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].SpaBudget.ToString("#,##0.00"), font8)))
                                   {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].OtherActual.ToString("#,##0.00"), font8)))
                                   {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);
                    pdfPCell = new PdfPCell(new Phrase(new Chunk(report[j].OtherBudget.ToString("#,##0.00"), font8)))
                                   {HorizontalAlignment = Element.ALIGN_RIGHT};
                    pdfTable.AddCell(pdfPCell);

                }

                pdfPCell = new PdfPCell(new Phrase(new Chunk("Total", font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("-", font8B))) {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("-", font8B))) {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[0].ToString("#,##0.00"), font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[1].ToString("#,##0.00"), font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[2].ToString("#,##0.00"), font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[3].ToString("#,##0.00"), font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[4].ToString("#,##0.00"), font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[5].ToString("#,##0.00"), font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[6].ToString("#,##0.00"), font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[7].ToString("#,##0.00"), font8B)))
                               {HorizontalAlignment = Element.ALIGN_RIGHT};
                pdfTable.AddCell(pdfPCell);

                pdfDoc.Add(pdfTable); // add pdf table to the document

                var newLine = new Paragraph();
                newLine.Add(new Paragraph("", fontT));
                preface.Leading = 50.0f;

                pdfDoc.Add(newLine);
            }

            /////////////////////////////////////////////////////////////////////////////////////////////////////////

            pdfDoc.Add(prefacedate);
            pdfDoc.Close();
            pdfWriter.Close();


            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", attachment);
            Response.BinaryWrite(pdfStream.ToArray());
            Response.End();
        }
    }
}

