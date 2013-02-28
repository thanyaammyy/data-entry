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
            var propertyId = Convert.ToInt32(ddlCompany.SelectedValue);
            if(propertyId<=0)
            {
                lbError.Visible = true;
                lbCalendar.Visible = true;
                lbCompany.Visible = true;
            }
            else
            {
                Year = hiddenMonthYear.Value;
                Session["bPropertyId"] = propertyId;
                Session["year"] = Year;
                Session["fromMenuBudget"] = null;

                var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(Convert.ToInt32(Session["bPropertyId"]));
                _propertyName = property.PropertyName;
                _year = Session["year"].ToString();

                ShowData(Convert.ToInt32(Session["bPropertyId"]), Session["year"].ToString());
            }
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

                var username = Session["UserSession"].ToString();
                var user = UserHelper.GetUser(username);

                if(user.PropertyId==15)//OHG ID
                {
                    divExportAllData.Attributes["style"] = "";
                }

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
            AppendTotal(dataEntryList);
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

        protected List<double> CalculateTotal(List<HotelDataEntryLib.BudgetEntry> listBudgetEntry)
        {
            var list = new List<double>(5);
            var roomTotal = 0.00;
            var fbTotal = 0.00;
            var spaTotal = 0.00;
            var othersTotal = 0.00;
            var total = 0.00;
            foreach (var revenueEntry in listBudgetEntry)
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
            list.Add(roomTotal);
            list.Add(fbTotal);
            list.Add(spaTotal);
            list.Add(othersTotal);
            list.Add(total);
            return list;
        }

        protected void AppendTotal(List<HotelDataEntryLib.BudgetEntry> listBudgetEntry)
        {
            var list = CalculateTotal(listBudgetEntry);
            JqGridBudgetEntry.Columns.FromDataField("RoomBudget").FooterValue = list[0].ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("FBBudget").FooterValue = list[1].ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("SpaBudget").FooterValue = list[2].ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("Others").FooterValue = list[3].ToString("#,##0.00");
            JqGridBudgetEntry.Columns.FromDataField("Total").FooterValue = list[4].ToString("#,##0.00");
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
                _currency = currency.CurrencyCode;
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
                _currency = currency.CurrencyCode;
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
            Response.Write("\n");
            Response.Write("Print Date: [" + DateTime.Now + "] ");
            Response.Write("\r\n");
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
            var font8B = FontFactory.GetFont("ARIAL", 8, Font.  BOLD);

            var preface = new Paragraph();
            var prefacedate = new Paragraph();

            // Lets write a big header
            preface.Add(new Paragraph("[" + _currency + "] " + _propertyName + " Budget " + _year, fontT));
            prefacedate.Add(new Paragraph("Print Date: [" + DateTime.Now + "] ", font8B));


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

        protected void btnPDFAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            var hb = new HotelBudget();
            if (Session["year"] == null) return;
            var year = Convert.ToInt32(Session["year"].ToString());
            var budget = HotelDataEntryLib.Page.BudgetHelper.GetAllPropertyByHotelBudget(year);
            
            var attachment = "attachment; filename= All Properties" + " Budget " + _year + ".pdf";
            var pdfDoc = new Document(PageSize.A4.Rotate(), 30.0f, 5.0f, 40.0f, 0f);
            var pdfStream = new MemoryStream();
            var pdfWriter = PdfWriter.GetInstance(pdfDoc, pdfStream);

            pdfDoc.Open();//Open Document to write

            pdfDoc.NewPage();

            var fontH = FontFactory.GetFont("ARIAL", 9, Font.BOLD);
            var fontT = FontFactory.GetFont("ARIAL", 12, Font.BOLD);
            var font8 = FontFactory.GetFont("ARIAL", 8);
            var font8B = FontFactory.GetFont("ARIAL", 8, Font.BOLD);

            
            var prefacedate = new Paragraph {new Paragraph("Print Date: [" + DateTime.Now + "] ", font8B)};
            var widths = new float[] { 55, 75f, 75f, 72f, 72f, 72f, 72f };
            

            /////////////////////////////////////////////////////////////////////////////////////////////////////////
            for (var i = 0; i < budget.Count; i++)
            {
                //Begin table
                var pdfTable = new PdfPTable(7);
                pdfTable.HorizontalAlignment = 0;
                pdfTable.TotalWidth = 781f;
                pdfTable.LockedWidth = true;
                pdfTable.SetWidths(widths);
                pdfTable.SpacingBefore = 15f; // Give some space after the text or it may overlap the table            

                hb.HotelBudgetId = budget[i].HotelBudgetId;
                var listBudget = BudgetHelper.ListBudgetEntryByYear(hb);
                var total = CalculateTotal(listBudget);
                var preface = new Paragraph();
                // Header
                preface.Add(new Paragraph("[" + budget[i].CurrencyCode + "] " + budget[i].PropertyName + " Budget " + year, fontT));
                pdfDoc.Add(preface);

                PdfPCell pdfPCell = null;

                //Add Header of the pdf table
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Month/Year", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Occupancy(%)", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Room Budget", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("F & B Budget", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Spa Budget", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Others", fontH)));
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("Total", fontH)));
                pdfTable.AddCell(pdfPCell);

                //How add the data from datatable to pdf table
                for (var rows = 0; rows < listBudget.Count; rows++)
                {
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(listBudget[rows].PositionMonth, font8)))
                                        {HorizontalAlignment = Element.ALIGN_LEFT};
                        pdfTable.AddCell(pdfPCell);
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(listBudget[rows].OccupancyRoom.ToString("#,##0.00"), font8)))
                                        {HorizontalAlignment = Element.ALIGN_RIGHT};
                        pdfTable.AddCell(pdfPCell);
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(listBudget[rows].RoomBudget.ToString("#,##0.00"), font8)))
                                        {HorizontalAlignment = Element.ALIGN_RIGHT};
                        pdfTable.AddCell(pdfPCell);
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(listBudget[rows].FBBudget.ToString("#,##0.00"), font8)))
                                        {HorizontalAlignment = Element.ALIGN_RIGHT};
                        pdfTable.AddCell(pdfPCell);
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(listBudget[rows].SpaBudget.ToString("#,##0.00"), font8)))
                                        {HorizontalAlignment = Element.ALIGN_RIGHT};
                        pdfTable.AddCell(pdfPCell);
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(listBudget[rows].Others.ToString("#,##0.00"), font8)))
                                        {HorizontalAlignment = Element.ALIGN_RIGHT};
                        pdfTable.AddCell(pdfPCell);
                        pdfPCell = new PdfPCell(new Phrase(new Chunk(listBudget[rows].Total.ToString("#,##0.00"), font8)))
                                        {HorizontalAlignment = Element.ALIGN_RIGHT};
                        pdfTable.AddCell(pdfPCell); 

                }

                pdfPCell = new PdfPCell(new Phrase(new Chunk("Total", font8B))) { HorizontalAlignment = Element.ALIGN_RIGHT };
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk("-", font8B))) { HorizontalAlignment = Element.ALIGN_RIGHT };
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[0].ToString("#,##0.00"), font8B))) { HorizontalAlignment = Element.ALIGN_RIGHT };
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[1].ToString("#,##0.00"), font8B))) { HorizontalAlignment = Element.ALIGN_RIGHT };
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[2].ToString("#,##0.00"), font8B))) { HorizontalAlignment = Element.ALIGN_RIGHT };
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[3].ToString("#,##0.00"), font8B))) { HorizontalAlignment = Element.ALIGN_RIGHT };
                pdfTable.AddCell(pdfPCell);
                pdfPCell = new PdfPCell(new Phrase(new Chunk(total[4].ToString("#,##0.00"), font8B))) { HorizontalAlignment = Element.ALIGN_RIGHT };
                pdfTable.AddCell(pdfPCell);

                pdfDoc.Add(pdfTable); // add pdf table to the document

                var newLine = new Paragraph();
                    newLine.Add(new Paragraph("",fontT));
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

        protected void btnExcelAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            var hb = new HotelBudget();
            if (Session["year"] == null) return;
            var year = Convert.ToInt32(Session["year"].ToString());
            var budget = HotelDataEntryLib.Page.BudgetHelper.GetAllPropertyByHotelBudget(year);

            var attachment = "attachment; filename=All Properties" + " Budget " + _year + ".xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            for (var i = 0; i < budget.Count; i++)
            {
                Response.Write("[" + budget[i].CurrencyCode + "] " + budget[i].PropertyName + " Budget " + _year);
                Response.Write("\r\n");
                Response.Write("\r\n");

                Response.Write("Month/Year\t");
                Response.Write("Occupancy(%)\t");
                Response.Write("Room Budget\t");
                Response.Write("F & B Budget\t");
                Response.Write("Spa Budget\t");
                Response.Write("Others\t");
                Response.Write("Total\t");
                Response.Write("\n");

                hb.HotelBudgetId = budget[i].HotelBudgetId;
                var listBudget = BudgetHelper.ListBudgetEntryByYear(hb);
                var total = CalculateTotal(listBudget);
                for (var j = 0; j < listBudget.Count;j++ )
                {
                    Response.Write(listBudget[j].PositionMonth+"\t");
                    Response.Write(listBudget[j].OccupancyRoom.ToString("#,##0.00") + "\t");
                    Response.Write(listBudget[j].RoomBudget.ToString("#,##0.00") + "\t");
                    Response.Write(listBudget[j].FBBudget.ToString("#,##0.00") + "\t");
                    Response.Write(listBudget[j].SpaBudget.ToString("#,##0.00") + "\t");
                    Response.Write(listBudget[j].Others.ToString("#,##0.00") + "\t");
                    Response.Write(listBudget[j].Total.ToString("#,##0.00") + "\t");
                    Response.Write("\n");
                }
                Response.Write("Total" + "\t");
                Response.Write("-" + "\t");
                Response.Write(total[0].ToString("#,##0.00") + "\t");
                Response.Write(total[1].ToString("#,##0.00") + "\t");
                Response.Write(total[2].ToString("#,##0.00") + "\t");
                Response.Write(total[3].ToString("#,##0.00") + "\t");
                Response.Write(total[4].ToString("#,##0.00") + "\t");
                Response.Write("\n");
                Response.Write("\n");
                Response.Write("\n");
            }

            Response.Write("\n");
            Response.Write("Print Date: [" + DateTime.Now + "] ");
            Response.Write("\r\n");
            Response.End();
        }
    }
}