using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Web;
using System.Web.UI.WebControls;
using HotelDataEntryLib;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HotelDataEntry
{
    public partial class Budget : System.Web.UI.Page
    {
        public string Year;
        public int UserId;

        private string _propertyName;
        private string _year;
        private string _currency;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Revenue
            Session["rPropertyId"] = null;
            Session["MonthYear"] = null;
            
            //First Load from menulink
            var fromMenu = Request.QueryString["key"];
            if (!string.IsNullOrEmpty(fromMenu))
            {
                Session["fromMenuBudget"] = fromMenu;
                Response.Redirect("Budget.aspx");
            }

            if (!IsPostBack)
            {
                if (Session["fromMenuBudget"]==null)
                {
                    if (Session["bPropertyId"] == null || Session["year"] == null) return;
                    
                    var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(Convert.ToInt32(Session["bPropertyId"]));
                    _propertyName = property.PropertyName;
                    _year = Session["year"].ToString();

                    ShowData(Convert.ToInt32(Session["bPropertyId"]), Session["year"].ToString());
                }
                else
                {
                    divJqgrid.Attributes["style"] = "display:none";
                    Session["bPropertyId"] = null;
                    Session["year"] = null;
                }
            }    
        }

        protected void btnCreateForm_Click(object sender, EventArgs e)
        {
            var propertyId = ddlCompany.SelectedValue;
            Year = hiddenMonthYear.Value;
            Session["bPropertyId"] = propertyId;
            Session["year"] = Year;
            Session["fromMenuBudget"] = null;
            
            var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(Convert.ToInt32(Session["bPropertyId"]));
            _propertyName = property.PropertyName;
            _year = Session["year"].ToString();

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
            var roomBudget = string.IsNullOrEmpty(e.RowData["RoomBudget"]) ? 0.00 : Convert.ToDouble(e.RowData["RoomBudget"]);
            var fbBudget = string.IsNullOrEmpty(e.RowData["FBBudget"]) ? 0.00 : Convert.ToDouble(e.RowData["FBBudget"]);
            var spa = string.IsNullOrEmpty(e.RowData["SpaBudget"]) ? 0.00 : Convert.ToDouble(e.RowData["SpaBudget"]);
            var others = string.IsNullOrEmpty(e.RowData["Others"]) ? 0.00 : Convert.ToDouble(e.RowData["Others"]);
            var occupancyRoom = 0.0;
            if (string.IsNullOrEmpty(e.RowData["OccupancyRoom"]))
            {
                occupancyRoom = 0;
            }
            else
            {
                var strOccupancy = "";
                strOccupancy = e.RowData["OccupancyRoom"].Contains("%") ? e.RowData["OccupancyRoom"].Remove(e.RowData["OccupancyRoom"].Length - 1, 1) : e.RowData["OccupancyRoom"];
                occupancyRoom = Convert.ToDouble(strOccupancy);
            }

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
            var roomTotal = 0.00;
            var fbTotal = 0.00;
            var spaTotal = 0.00;
            var othersTotal = 0.00;
            var total = 0.00;
            foreach (var revenueEntry in listRevenueEntry)
            {
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
                _year = Year;
                _propertyName = ((DropDownList)sender).Text;
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

            var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(Convert.ToInt32(Session["bPropertyId"]));
            _propertyName = property.PropertyName;
            _year = Session["year"].ToString();

            ShowData(Convert.ToInt32(Session["bPropertyId"]), Session["year"].ToString());
        }

        protected void btnExcel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridBudgetEntry.ExportSettings.ExportHeaders = true;
            JqGridBudgetEntry.ExportSettings.ExportDataRange = ExportDataRange.All;

            var dt = JqGridBudgetEntry.GetExportData();

            dt.Rows.Add("Total", "-", JqGridBudgetEntry.Columns.FromDataField("RoomBudget").FooterValue,
                        JqGridBudgetEntry.Columns.FromDataField("FBBudget").FooterValue,
                        JqGridBudgetEntry.Columns.FromDataField("SpaBudget").FooterValue,
                        JqGridBudgetEntry.Columns.FromDataField("Others").FooterValue,
                        JqGridBudgetEntry.Columns.FromDataField("Total").FooterValue);
            ExportToExcel(dt);
        }

        protected void btnPDF_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridBudgetEntry.ExportSettings.ExportHeaders = true;
            JqGridBudgetEntry.ExportSettings.ExportDataRange = ExportDataRange.All;

            var dt = JqGridBudgetEntry.GetExportData();

            dt.Rows.Add("Total", "-", JqGridBudgetEntry.Columns.FromDataField("RoomBudget").FooterValue,
                        JqGridBudgetEntry.Columns.FromDataField("FBBudget").FooterValue,
                        JqGridBudgetEntry.Columns.FromDataField("SpaBudget").FooterValue,
                        JqGridBudgetEntry.Columns.FromDataField("Others").FooterValue,
                        JqGridBudgetEntry.Columns.FromDataField("Total").FooterValue);
            ExportToPDF(dt);
        }

        private void ExportToExcel(DataTable dt)
        {
            var attachment = "attachment; filename=" + _propertyName + " Budget " + _year + ".xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            var tab = "";
            Response.Write("[" + _currency + "] " + _propertyName + " Budget " + _year);
            Response.Write("\r\n");
            foreach (DataColumn dc in dt.Columns)
            {
                Response.Write(tab + dc.ColumnName);
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
            Response.End();
        }

        private void ExportToPDF(DataTable dt)
        {
            var attachment = "attachment; filename=" + _propertyName + " Budget " + _year + ".pdf";
            var pdfDoc = new Document(PageSize.A4.Rotate());
            var pdfStream = new MemoryStream();
            var pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream);

            pdfDoc.Open();//Open Document to write
            pdfDoc.AddSubject(_year);
            pdfDoc.AddTitle(_propertyName);

            pdfDoc.AddCreationDate();
            pdfDoc.NewPage();

            var fontH = FontFactory.GetFont("ARIAL", 9, Font.BOLD);
            var fontT = FontFactory.GetFont("ARIAL", 12, Font.BOLD);
            var font8 = FontFactory.GetFont("ARIAL", 8);
            var font8B = FontFactory.GetFont("ARIAL", 8, Font.BOLD);

            Paragraph preface = new Paragraph();

            // Lets write a big header
            preface.Add(new Paragraph("[" + _currency + "] " + _propertyName + " Budget " + _year, fontT));

            var pdfTable = new PdfPTable(dt.Columns.Count);
            pdfTable.HorizontalAlignment = 0;
            pdfTable.TotalWidth = 781f;
            pdfTable.LockedWidth = true;

            PdfPCell pdfPCell = null;



            //Add Header of the pdf table
            for (var column = 0; column < dt.Columns.Count; column++)
            {
                pdfPCell = new PdfPCell(new Phrase(new Chunk(dt.Columns[column].Caption, fontH)));
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

            var widths = new float[] { 55, 75f, 75f, 72f, 72f, 72f, 72f };
            pdfTable.SetWidths(widths);
            pdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table            

            pdfDoc.SetMargins(5.0f, 5.0f, 40.0f, 0f);
            pdfDoc.Add(preface);
            pdfDoc.Add(pdfTable); // add pdf table to the document
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