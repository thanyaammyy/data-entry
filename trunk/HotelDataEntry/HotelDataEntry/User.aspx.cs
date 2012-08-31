using System;
using System.Text;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class User : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var mainCompany = Request.QueryString["companyid"];
            if(!Page.IsPostBack)
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
                Response.Write(CompanyToJSON(companyid));
                try
                {
                    Response.End();
                }
                catch (Exception exception)
                {

                }
            }
        }

        private string CompanyToJSON(int companyId)
        {
            var dropdownHtml = new StringBuilder();
            var listCompany = PropertyHelper.ListAlterCompany(companyId);
            for (var i = 0; i < listCompany.Count;i++ )
            {
                if (i != 0) dropdownHtml.Append("|");
                dropdownHtml.Append(listCompany[i].PropertyId+","+listCompany[i].PropertyCode);
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
            if (!(string.IsNullOrEmpty(status) || string.IsNullOrEmpty(mainCompany)|| string.IsNullOrEmpty(permissionId)))
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
                    PermissionId= Convert.ToInt32(permissionId)
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
            if(string.IsNullOrEmpty(userId))return;
            UserHelper.DeleteUserProfile(Convert.ToInt32(userId));
        }

    }
}