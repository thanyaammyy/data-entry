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
        protected void Page_Load(object sender, EventArgs e)
        {
            //dataEntry
            Session["propertyId"] = null;
            Session["dataEntryTypeId"] = null;
            Session["MonthYear"] = null;

            if (!IsPostBack)
            {
                if (chkReportMonthly.Checked) Session["monthly"] = "monthly";
                if (Session["property"] == null || Session["dateFrom"] == null || Session["dateTo"] == null) return;
                ShowYearlyReport(Session["dateFrom"].ToString(), Session["dateTo"].ToString(), Convert.ToInt32(Session["property"]));
            }
        }

        protected void ddlCompany_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((DropDownList)sender).SelectedValue != "")
            {
                var propertyId = Convert.ToInt32(ddlCompany.SelectedValue);
                if (propertyId==0)
                {
                    displayCurrency.Attributes["style"] = "display:none";
                    return;
                }
                var curr = PropertyHelper.GetProperty(propertyId);
                var currency = CurrencyHelper.GetCurrency(curr.CurrencyId);
                lbCurerncy.Text = currency.CurrencyCode;
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
            ShowYearlyReport(strDateFrom,strDateTo,propertyId);
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

                DateTime dateTimeFromLastYear;
                DateTime dateTimeToLastYear;
                try
                {
                    dateTimeFromLastYear = new DateTime((fromYear - 1), fromMonth, fromDate);
                }
                catch (Exception ex)
                {
                    dateTimeFromLastYear = new DateTime((fromYear - 1), fromMonth, (fromDate - 1));
                }
                try
                {
                    dateTimeToLastYear = new DateTime((toYear - 1), toMonth, toDate);
                }
                catch (Exception ex)
                {
                    dateTimeToLastYear = new DateTime((toYear - 1), toMonth, (toDate - 1));
                }

                var listBudgetTY = ReportHelper.CalculateYearlyReport(new DateTime(fromYear, fromMonth, fromDate), new DateTime(toYear, toMonth, toDate), propertyId);
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

                    BindRowToDataTable(listRow);
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

                    BindRowToDataTable(listRow);
                }
            }

        }

        private void BindRowToDataTable(List<HotelDataEntryLib.Helper.Report> listReport)
        {
            JqGridYearlyReport.DataSource = listReport;
            CalculateYearlyTotal(listReport);
            JqGridYearlyReport.ToolBarSettings.ToolBarPosition = ToolBarPosition.Hidden;
            JqGridYearlyReport.DataBind();
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

        }

        protected void btnCSV2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        protected void btnExcel2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        protected void btnPDF2_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        protected void chkReportMonthly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkReportMonthly.Checked) Session["monthly"] = "monthly";
            else Session["monthly"] = null;

            ddlCompany.SelectedIndex = 0;
            ddlCompany2.SelectedIndex = 0;
        }

        protected void ddlCompany2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((DropDownList)sender).SelectedValue != "")
            {
                var propertyId = Convert.ToInt32(ddlCompany2.SelectedValue);
                if (propertyId == 0)
                {
                    displayCurrency2.Attributes["style"] = "display:none";
                    return;
                }
                var curr = PropertyHelper.GetProperty(propertyId);
                var currency = CurrencyHelper.GetCurrency(curr.CurrencyId);
                lbCurrency2.Text = currency.CurrencyCode;
                displayCurrency.Attributes["style"] = "";
            }
        }
    }
}