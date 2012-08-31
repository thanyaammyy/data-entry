﻿using System;
using System.Linq;
using System.Text;
using HotelDataEntryLib;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class User : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var mainCompany = Request.QueryString["companyid"];
            var user = Request.QueryString["userid"];
            if (!Page.IsPostBack)
            {
                if (JqgridUser.AjaxCallBackMode == AjaxCallBackMode.RequestData)
                {
                    JqgridUserBinding();
                }
            }

            if (!string.IsNullOrEmpty(mainCompany))
            {
                var companyid = Convert.ToInt32(mainCompany);
                Response.Clear();
                if (!string.IsNullOrEmpty(user))
                {
                    var userId = Convert.ToInt32(user);
                    Response.Write(CompanyToJSON(companyid, userId));
                }
                else
                {
                    Response.Write(CompanyToJSON(companyid, 0));
                }
                try
                {
                    Response.End();
                }
                catch (Exception exception)
                {
                }
            }
        }

        private string CheckSelected(int id, int userId)
        {
            string selected = "";
            using (var hdc = new HotelDataEntryDataContext())
            {
                var propertyId = hdc.Users.Single(item => item.UserId == userId).AlterPropertyId;
                selected = propertyId == id ? "selected" : "";
            }
            return selected;
        }

        private string CompanyToJSON(int companyId, int userId)
        {
            var dropdownHtml = new StringBuilder();
            var listCompany = PropertyHelper.ListAlterCompany(companyId);
            for (var i = 0; i < listCompany.Count; i++)
            {
                if (i != 0) dropdownHtml.Append("|");
                dropdownHtml.Append(listCompany[i].PropertyId + "," + listCompany[i].PropertyCode + "," + CheckSelected(listCompany[i].PropertyId, userId));
            }
            return dropdownHtml.ToString();
        }

        private void JqgridUserBinding()
        {
            var currencyList = UserHelper.ListUser();
            JqgridUser.DataSource = currencyList;
            JqgridUser.DataBind();
        }

        protected void JqgridUser_RowAdding(object sender, JQGridRowAddEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var mainCompany = e.RowData["PropertyCode"];
            var alterCompany = e.RowData["AlterCompany"];
            var permissionId = e.RowData["PermissionId"];
            if (!(string.IsNullOrEmpty(status) || string.IsNullOrEmpty(mainCompany) || string.IsNullOrEmpty(permissionId)))
            {
                var user = new HotelDataEntryLib.User()
                {
                    PropertyId = Convert.ToInt32(mainCompany),
                    FirstName = e.RowData["FirstName"],
                    LastName = e.RowData["LastName"],
                    Email = e.RowData["Email"],
                    Status = Convert.ToInt32(status),
                    UpdateDateTime = DateTime.Now,
                    AlterPropertyId = Convert.ToInt32(alterCompany),
                    PermissionId = Convert.ToInt32(permissionId)
                };
                UserHelper.AddUserProfile(user);
            }
        }

        protected void JqgridUser_RowEditing(object sender, JQGridRowEditEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var mainCompany = e.RowData["PropertyCode"];
            var alterCompany = e.RowData["AlterCompany"];
            var permissionId = e.RowData["PermissionId"];
            var userId = e.RowKey;
            if (!(string.IsNullOrEmpty(status) || string.IsNullOrEmpty(mainCompany) || string.IsNullOrEmpty(permissionId)))
            {
                var user = new HotelDataEntryLib.User()
                {
                    UserId = Convert.ToInt32(userId),
                    PropertyId = Convert.ToInt32(mainCompany),
                    FirstName = e.RowData["FirstName"],
                    LastName = e.RowData["LastName"],
                    Email = e.RowData["Email"],
                    Status = Convert.ToInt32(status),
                    UpdateDateTime = DateTime.Now,
                    AlterPropertyId = Convert.ToInt32(alterCompany),
                    PermissionId = Convert.ToInt32(permissionId)
                };
                UserHelper.UpdateUserProfile(user);
            }
        }

        protected void JqgridUser_RowDeleting(object sender, JQGridRowDeleteEventArgs e)
        {
            var userId = e.RowKey;
            if (string.IsNullOrEmpty(userId)) return;
            UserHelper.DeleteUserProfile(Convert.ToInt32(userId));
        }
    }
}