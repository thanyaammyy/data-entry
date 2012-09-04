﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
            JqGridDataEntry.DataBind();
        }
        protected void JqGridDataEntry_RowEditing(object sender, JQGridRowEditEventArgs jqGridRowEditEventArgs)
        {

        }
    }
}