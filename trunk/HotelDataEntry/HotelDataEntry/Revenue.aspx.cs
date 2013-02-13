﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.UI.WebControls;
using HotelDataEntryLib;
using HotelDataEntryLib.Helper;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace HotelDataEntry
{
    public partial class DataEntry : System.Web.UI.Page
    {
        public string MonthYear;
        public int UserId ;

        private string _propertyName;
        private string _year;
        private string _currency;

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
                    
                    var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(Convert.ToInt32(Session["rPropertyId"]));
                    _propertyName = property.PropertyName;
                    _year = Session["MonthYear"].ToString();

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
            
            var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(Convert.ToInt32(Session["rPropertyId"]));
            _propertyName = property.PropertyName;
            _year = Session["MonthYear"].ToString();

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

                var username = Session["UserSession"].ToString();
                var user = UserHelper.GetUser(username);

                if(user.PropertyId==15)//OHG ID
                {
                    divExportAllData.Attributes["style"] = "";
                }

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
                _year = MonthYear;
                _propertyName = ((DropDownList) sender).Text;
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

        protected void JqGridRevenueEntry_Init(object sender, EventArgs e)
        {
            if (Session["rPropertyId"] == null || Session["MonthYear"] == null) return;

            ShowData(Convert.ToInt32(Session["rPropertyId"]), Session["MonthYear"].ToString());

            var property = HotelDataEntryLib.Page.PropertyHelper.GetProperty(Convert.ToInt32(Session["rPropertyId"]));
            _propertyName = property.PropertyName;
            _year = Session["MonthYear"].ToString();
        }

        protected void btnExcel_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridRevenueEntry.ExportSettings.ExportHeaders = true;
            JqGridRevenueEntry.ExportSettings.ExportDataRange = ExportDataRange.All;

            var dt = JqGridRevenueEntry.GetExportData();

            dt.Rows.Add("Total", "-", JqGridRevenueEntry.Columns.FromDataField("RoomRevenue").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("FBRevenue").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("SpaRevenue").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("Others").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("Total").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("Budget").FooterValue);
            ExportToExcel(dt);
        }

        protected void btnPDF_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {
            JqGridRevenueEntry.ExportSettings.ExportHeaders = true;
            JqGridRevenueEntry.ExportSettings.ExportDataRange = ExportDataRange.All;

            var dt = JqGridRevenueEntry.GetExportData();

            dt.Rows.Add("Total", "-", JqGridRevenueEntry.Columns.FromDataField("RoomRevenue").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("FBRevenue").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("SpaRevenue").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("Others").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("Total").FooterValue,
                        JqGridRevenueEntry.Columns.FromDataField("Budget").FooterValue);
            ExportToPDF(dt);
        }

        private void ExportToExcel(DataTable dt)
        {
            var attachment = "attachment; filename=" + _propertyName + " Revenue " + _year + ".xls";
            Response.ClearContent();
            Response.AddHeader("content-disposition", attachment);
            Response.ContentType = "application/vnd.ms-excel";
            var tab = "";
            Response.Write("[" + _currency + "] " + _propertyName + " Revenue " + _year);
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
            Response.End();
        }

        private void ExportToPDF(DataTable dt)
        {
            var attachment = "attachment; filename=" + _propertyName + " Revenue " + _year + ".pdf";
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

            var preface = new Paragraph();
            var prefacedate = new Paragraph();

            // Lets write a big header
            preface.Add(new Paragraph("["+_currency+"] "+_propertyName + " Revenue " + _year, fontT));
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

            var widths = new float[] { 55, 75f, 75f, 72f, 72f, 72f, 72f, 72f };
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

        protected void btnExcelAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }

        protected void btnPDFAll_Click(object sender, System.Web.UI.ImageClickEventArgs e)
        {

        }
    }
}