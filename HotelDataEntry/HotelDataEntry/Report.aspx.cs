using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI.WebControls;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;


namespace HotelDataEntry
{
    public partial class Report : System.Web.UI.Page
    {
        public string DateFrom;
        public string DateTo;
        public string MonthlyDate;
        protected void Page_Load(object sender, EventArgs e)
        {
            //dataEntry
            Session["propertyId"] = null;
            Session["dataEntryTypeId"] = null;
            Session["MonthYear"] = null;
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateFrom = hiddenDateFrom.Value;
            DateTo = hiddenDateTo.Value;
            Session["dateFrom"] = DateFrom;
            Session["dateTo"] = DateTo;
            if (((DropDownList)sender).SelectedValue != "")
            {
                var propertyId = Convert.ToInt32(ddlCompany.SelectedValue);
                if (propertyId==0)
                {
                    displayCurrency.Attributes["style"] = "display:none";
                    return;
                }
                var curr = PropertyHelper.GetProperty(propertyId);
                //var currency = CurrencyHelper.GetCurrency(curr.CurrencyId);
                //lbCurerncy.Text = currency.CurrencyCode;
                displayCurrency.Attributes["style"] = "";
            }
        }

        protected void btnYearlyReport_Click(object sender, EventArgs e)
        {
            var property = ddlCompany.SelectedValue;
            DateFrom = hiddenDateFrom.Value;
            DateTo = hiddenDateTo.Value;



            if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(DateFrom) || string.IsNullOrEmpty(DateTo))
                return;
            Session["property"] = property;
            Session["dateFrom"] = DateFrom;
            Session["dateTo"] = DateTo;
            var propertyId = Convert.ToInt32(Session["property"]);
            var strDateFrom = Session["dateFrom"].ToString();
            var strDateTo = Session["dateTo"].ToString();
            ShowYearlyReport(strDateFrom, strDateTo, propertyId);
             
        }

        private static bool IsValidDateFromTo(string dateFrom, string dateTo)
        {
            var from = DateTime.Parse(dateFrom, new System.Globalization.CultureInfo("en-US"));
            var to = DateTime.Parse(dateTo, new System.Globalization.CultureInfo("en-US"));
            var result = DateTime.Compare(to, from);
            return result>=0;
        }

        private void ShowYearlyReport(string dateFrom, string dateTo, int propertyId)
        {
            if (string.IsNullOrEmpty(dateFrom) || string.IsNullOrEmpty(dateTo)||propertyId<=0)
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

                divJqGridYearlyReport.Attributes["style"] = "display:";
                divExportData.Attributes["style"] = "display:";

                var sesseionDateFrom = dateFrom;
                var strFrom = sesseionDateFrom.Split('/');
                var fromDate = Convert.ToInt32(strFrom[0]);
                var fromMonth = Convert.ToInt32(strFrom[1]);
                var fromYear = Convert.ToInt32(strFrom[2]);

                var sesseionDateTo = dateTo;
                var strTo = sesseionDateTo.Split('/');
                var toDate = Convert.ToInt32(strTo[0]);
                var toMonth = Convert.ToInt32(strTo[1]);
                var toYear = Convert.ToInt32(strTo[2]);

                var dateTimeFrom = new DateTime(fromYear, fromMonth, fromDate);
                var dateTimeTo = new DateTime(toYear, toMonth, toDate);

                var dateTimeFromLastYear = dateTimeFrom.AddYears(-1);
                var dateTimeToLastYear = dateTimeTo.AddYears(-1);

                var listBudgetTY = ReportHelper.CalculateYearlyReport(dateTimeFrom, dateTimeTo, propertyId);
                var listBudgetLY = ReportHelper.CalculateYearlyReport(dateTimeFromLastYear, dateTimeToLastYear, propertyId);
                var budgetLY = listBudgetLY as List<HotelDataEntryLib.Helper.Report> ?? listBudgetLY.ToList();
                if (listBudgetLY != null && budgetLY.Count() != 0)
                {
                    var listRow = (from t in listBudgetTY
                                   from l in budgetLY
                                   where t.Type == l.Type && t.SubType == l.SubType
                                   select new HotelDataEntryLib.Helper.Report()
                                   {
                                       Type = t.Type,
                                       SubType = t.SubType,
                                       BudgetTY = t.BudgetTY,
                                       BudgetLY = l.BudgetTY
                                   }).ToList();

                    BindRowToDataTableYearlyReport(listRow);
                }
                else
                {
                    var listRow = (from t in listBudgetTY
                                   select new HotelDataEntryLib.Helper.Report()
                                   {
                                       Type = t.Type,
                                       SubType = t.SubType,
                                       BudgetTY = t.BudgetTY,
                                       BudgetLY = 0.00
                                   }).ToList();

                    BindRowToDataTableYearlyReport(listRow);
                }
            }

        }

        private void BindRowToDataTableYearlyReport(List<HotelDataEntryLib.Helper.Report> listReport)
        {
            JqGridYearlyReport.DataSource = listReport;
            CalculateYearlyTotal(listReport);
            JqGridYearlyReport.ToolBarSettings.ToolBarPosition = ToolBarPosition.Hidden;
            JqGridYearlyReport.DataBind();
        }

        private void BindRowToDataTableMonthlyReport(List<HotelDataEntryLib.Helper.Report> listReport)
        {
            JqGridMonthlyReport.DataSource = listReport;
            CalculateMonthlyTotal(listReport);
            JqGridMonthlyReport.ToolBarSettings.ToolBarPosition = ToolBarPosition.Hidden;
            JqGridMonthlyReport.DataBind();
        }

        protected void CalculateMonthlyTotal(List<HotelDataEntryLib.Helper.Report> listReport)
        {
            var actualTotal = 0.00;
            var actualLYTotal = 0.00;
            var actualYtdTotal = 0.00;
            var actualYtdLYTotal = 0.00;
            foreach (var o in listReport)
            {
                var actual = o.Actual;
                actualTotal += actual;

                var actualLY = o.ActualLY;
                actualLYTotal += actualLY;

                var actualYtd = o.ActualYtd;
                actualYtdTotal += actualYtd;

                var actualYtdLY = o.ActualYtdLY;
                actualYtdLYTotal += actualYtdLY;
            }

            JqGridMonthlyReport.Columns.FromDataField("Actual").FooterValue = actualTotal.ToString();
            JqGridMonthlyReport.Columns.FromDataField("ActualLY").FooterValue = actualLYTotal.ToString();
            JqGridMonthlyReport.Columns.FromDataField("ActualYtd").FooterValue = actualYtdTotal.ToString();
            JqGridMonthlyReport.Columns.FromDataField("ActualYtdLY").FooterValue = actualYtdLYTotal.ToString();
            JqGridMonthlyReport.Columns.FromDataField("SubType").FooterValue = "Total";
        }

        protected void CalculateYearlyTotal(List<HotelDataEntryLib.Helper.Report> listReport)
        {
            var btyTotal = 0.00;
            var blyTotal = 0.00;
            foreach (var o in listReport)
            {
                var bty = o.BudgetTY;
                btyTotal += bty;

                var bly = o.BudgetLY;
                blyTotal += bly;
            }

            JqGridYearlyReport.Columns.FromDataField("BudgetTY").FooterValue = btyTotal.ToString();
            JqGridYearlyReport.Columns.FromDataField("BudgetLY").FooterValue = blyTotal.ToString();
            JqGridYearlyReport.Columns.FromDataField("SubType").FooterValue = "Total";
        }

        protected void btnCSV_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridYearlyReport.ExportSettings.ExportDataRange = ExportDataRange.All;
            JqGridYearlyReport.ExportToCSV("report.csv");
        }

        protected void btnExcel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridYearlyReport.ExportSettings.ExportDataRange = ExportDataRange.All;
            JqGridYearlyReport.ExportToExcel("report.xls");
        }

        protected void btnPDF_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridYearlyReport.ExportSettings.ExportDataRange = ExportDataRange.All;
            var dt = JqGridYearlyReport.GetExportData();
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

            var PdfTable = new PdfPTable(dt.Columns.Count);
            PdfPCell PdfPCell = null;

            //Add Header of the pdf table
            for (var column = 0; column < dt.Columns.Count; column++)
            {
                PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].Caption, font8)));
                PdfTable.AddCell(PdfPCell);
            }

            //How add the data from datatable to pdf table
            for (var rows = 0; rows < dt.Rows.Count; rows++)
            {
                for (var column = 0; column < dt.Columns.Count; column++)
                {
                    PdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Rows[rows][column].ToString(), font8)));
                    PdfTable.AddCell(PdfPCell);
                }
            }

            PdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table            
            pdfDoc.Add(PdfTable); // add pdf table to the document
            pdfDoc.Close();
            pdfWriter.Close();


            Response.ClearContent();
            Response.ClearHeaders();
            Response.ContentType = "application/pdf";
            Response.AppendHeader("Content-Disposition", "attachment; filename=report.pdf");
            Response.BinaryWrite(pdfStream.ToArray());
            Response.End();
        }

        protected void btnMonthlyReport_Click(object sender, EventArgs e)
        {
            var property = ddlCompany2.SelectedValue;
            MonthlyDate = hiddenMonthYear.Value;


            if (string.IsNullOrEmpty(property) || string.IsNullOrEmpty(MonthlyDate))
                return;
            Session["property2"] = property;
            Session["monthlyDate"] = MonthlyDate;
            var propertyId = Convert.ToInt32(Session["property2"]);
            var strMonthlyDate = Session["monthlyDate"].ToString();
            ShowMonthlyReport(strMonthlyDate, propertyId);
        }

        protected void btnCSV2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridMonthlyReport.ExportSettings.ExportDataRange = ExportDataRange.All;
            JqGridMonthlyReport.ExportToCSV("report.csv");
        }

        protected void btnExcel2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridMonthlyReport.ExportSettings.ExportDataRange = ExportDataRange.All;
            JqGridMonthlyReport.ExportToExcel("report.xls");
        }

        protected void btnPDF2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridMonthlyReport.ExportSettings.ExportDataRange = ExportDataRange.All;
            var dt = JqGridMonthlyReport.GetExportData();
            ExportToPDF(dt);
        }

        protected void chkReportMonthly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkReportMonthly.Checked) { 
                Session["monthly"] = "monthly";
                Session["IsMonthly"] = "true";
            }
            else
            {
                Session["monthly"] = null;
                Session["IsMonthly"] = "false";
            }

            ddlCompany.SelectedIndex = 0;
            ddlCompany2.SelectedIndex = 0;
        }

        protected void ddlCompany2_SelectedIndexChanged(object sender, EventArgs e)
        {
            MonthlyDate = hiddenMonthYear.Value;
            Session["monthlyDate"] = MonthlyDate;
            if (((DropDownList)sender).SelectedValue != "")
            {
                var propertyId = Convert.ToInt32(ddlCompany2.SelectedValue);
                if (propertyId == 0)
                {
                    displayCurrency2.Attributes["style"] = "display:none";
                    return;
                }
                var curr = PropertyHelper.GetProperty(propertyId);
                //var currency = CurrencyHelper.GetCurrency(curr.CurrencyId);
                //lbCurrency2.Text = currency.CurrencyCode;
                displayCurrency2.Attributes["style"] = "";
            }
        }


        private void ShowMonthlyReport(string monthlyDate, int propertyId)
        {
            if (string.IsNullOrEmpty(monthlyDate)  || propertyId <= 0)
            {
                lbCompany2.Visible = true;
                lbMonthlyDate.Visible = true;
                lbError2.Visible = true;
            }
            else
            {
                lbCompany2.Visible = false;
                lbMonthlyDate.Visible = false;
                lbError2.Visible = false;

                divJqGridMonthlyReport.Attributes["style"] = "display:";
                divExportData2.Attributes["style"] = "display:";

                var sesseionDateFrom = monthlyDate;
                var strFrom = sesseionDateFrom.Split('/');
                var fromMonth = Convert.ToInt32(strFrom[0]);
                var fromYear = Convert.ToInt32(strFrom[1]);
                var endDate = DataEntryHelper.GetLastDayOfMonth(fromMonth, fromYear);

                //MTD
                var dateTimeFromThisMonth = new DateTime(fromYear, fromMonth, 1);
                var dateTimeToThisMonth = new DateTime(fromYear, fromMonth, endDate);

                //MTD-LY
                var dateTimeFromThisMonthLY = dateTimeFromThisMonth.AddYears(-1);
                var dateTimeToThisMonthLY = dateTimeToThisMonth.AddYears(-1);

                //YTD
                var dateTimeFromYtd = new DateTime(fromYear, 1,1);
                var dateTimeToYtd = new DateTime(fromYear, fromMonth, endDate);

                //YTD-LY
                var dateTimeFromYtdLY = dateTimeFromYtd.AddYears(-1);
                var dateTimeToTtdLY = dateTimeToYtd.AddYears(-1);

                var listActualThisMonth = ReportHelper.CalculateMonthlyReport(dateTimeFromThisMonth, dateTimeToThisMonth, propertyId);
                var listActualThisMonthLY = ReportHelper.CalculateMonthlyReport(dateTimeFromThisMonthLY, dateTimeToThisMonthLY, propertyId);

                var listActual = ListActual(listActualThisMonth, listActualThisMonthLY);


                var listYtd = ReportHelper.CalculateMonthlyReport(dateTimeFromYtd, dateTimeToYtd, propertyId);
                var listYtdLY = ReportHelper.CalculateMonthlyReport(dateTimeFromYtdLY, dateTimeToTtdLY, propertyId);

                var listActualYtd = ListActualYtd(listYtd, listYtdLY);

                var listRow = ListMonthly(listActual, listActualYtd);
                BindRowToDataTableMonthlyReport(listRow);
            }

        }

        private static IEnumerable<HotelDataEntryLib.Helper.Report> ListActual(IEnumerable<HotelDataEntryLib.Helper.Report> listActualThisMonth, IEnumerable<HotelDataEntryLib.Helper.Report> listActualThisMonthLY)
        {
            List<HotelDataEntryLib.Helper.Report> listRow;
            var actualThisMonth = listActualThisMonthLY as List<HotelDataEntryLib.Helper.Report> ?? listActualThisMonthLY.ToList();
            if (listActualThisMonthLY != null && actualThisMonth.Count() != 0)
            {
                listRow = (from t in listActualThisMonth
                               from l in actualThisMonth
                               where t.Type == l.Type && t.SubType == l.SubType
                               select new HotelDataEntryLib.Helper.Report()
                               {
                                   Type = t.Type,
                                   SubType = t.SubType,
                                   Actual = t.Actual,
                                   ActualLY = l.Actual
                               }).ToList();
            }
            else
            {
                listRow = (from t in listActualThisMonth
                               select new HotelDataEntryLib.Helper.Report()
                               {
                                   Type = t.Type,
                                   SubType = t.SubType,
                                   Actual = t.Actual,
                                   ActualLY = 0.00
                               }).ToList();
            }
            return listRow;
        }

        private static IEnumerable<HotelDataEntryLib.Helper.Report> ListActualYtd(IEnumerable<HotelDataEntryLib.Helper.Report> listActualYtd, IEnumerable<HotelDataEntryLib.Helper.Report> listActualYtdLY)
        {
            List<HotelDataEntryLib.Helper.Report> listRow;
            var actualThisMonth = listActualYtdLY as List<HotelDataEntryLib.Helper.Report> ?? listActualYtdLY.ToList();
            if (listActualYtdLY != null && actualThisMonth.Count() != 0)
            {
                listRow = (from t in listActualYtd
                           from l in actualThisMonth
                           where t.Type == l.Type && t.SubType == l.SubType
                           select new HotelDataEntryLib.Helper.Report()
                           {
                               Type = t.Type,
                               SubType = t.SubType,
                               ActualYtd = t.Actual,
                               ActualYtdLY = l.Actual
                           }).ToList();
            }
            else
            {
                listRow = (from t in listActualYtd
                           select new HotelDataEntryLib.Helper.Report()
                           {
                               Type = t.Type,
                               SubType = t.SubType,
                               ActualYtd = t.Actual,
                               ActualYtdLY = 0.00
                           }).ToList();
            }
            return listRow;
        }

        private static List<HotelDataEntryLib.Helper.Report> ListMonthly(IEnumerable<HotelDataEntryLib.Helper.Report> listActual, IEnumerable<HotelDataEntryLib.Helper.Report> listActualYtd)
        {
            return  ( from t in listActual
                            from l in listActualYtd
                            where t.Type == l.Type && t.SubType == l.SubType
                            select new HotelDataEntryLib.Helper.Report()
                            {
                                Type = t.Type,
                                SubType = t.SubType,
                                Actual = t.Actual,
                                ActualLY = t.ActualLY,
                                ActualYtd = l.ActualYtd,
                                ActualYtdLY = l.ActualYtdLY
                            }).ToList();

        }

        protected void JqGridYearlyReport_Init(object sender, EventArgs e)
        {
            Session["monthly"] = null;
            if (Session["property"] == null || Session["dateFrom"] == null) return;
            ShowYearlyReport(Session["dateFrom"].ToString(), Session["dateTo"].ToString(), Convert.ToInt32(Session["property"]));
        }

        protected void JqGridMonthlyReport_Init(object sender, EventArgs e)
        {
            Session["monthly"] = "monthly";
            if (Session["property2"] == null || Session["monthlyDate"] == null) return;
            ShowMonthlyReport(Session["monthlyDate"].ToString(), Convert.ToInt32(Session["property2"]));
        }

    }
}