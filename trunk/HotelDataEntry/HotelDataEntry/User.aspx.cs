using System;
using System.Linq;
using System.Text;
using HotelDataEntryLib.Page;
using Trirand.Web.UI.WebControls;

namespace HotelDataEntry
{
    public partial class User : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //dataEntry
            Session["propertyId"] = null;
            Session["dataEntryTypeId"] = null;
            Session["MonthYear"] = null;

            //Report
            Session["monthly"] = null;
            Session["property"] = null;
            Session["dateFrom"] = null;
            Session["dateTo"] = null;
            Session["monthly"] = null;
            Session["property2"] = null;
            Session["monthlyDate"] = null;
            Session["IsMonthly"] = null;

            var user = Request.QueryString["userid"];
            if (!Page.IsPostBack)
            {
                   JqgridUserBinding();

            }

            if (!string.IsNullOrEmpty(user))
            {
                var userId = Convert.ToInt32(user);
                Response.Clear();
                Response.Write(PropertiesToJson(userId));
                try
                {
                    Response.End();
                }
                catch (Exception exception)
                {
                }
            }
        }

        private static string PropertiesToJson(int userId)
        {
            var dropdownHtml = new StringBuilder();
            var user = UserHelper.GetUserInfo(userId);
            var accessProperties = user.AccessProperties;

            var listProperties = PropertyHelper.ListProperites();
            for (var i = 0; i < listProperties.Count; i++)
            {
                if (i != 0) dropdownHtml.Append("|");
                dropdownHtml.Append(listProperties[i].PropertyId + "," + listProperties[i].PropertyCode+","+CheckAccessProperty(accessProperties,listProperties[i].PropertyCode));
            }
            return dropdownHtml.ToString();
        }

        private static string CheckAccessProperty(string accessProperty, string propertyCode)
        {
            var result = "";
            var str = accessProperty.Split(',');
            foreach (var s in str.Where(s => propertyCode.Equals(s)))
            {
                result = "checked";
            }
            return result;
        }

        private void JqgridUserBinding()
        {
            var userList = UserHelper.ListUser();
            JqgridUser.DataSource = userList;
            JqgridUser.DataBind();
        }

        protected void JqgridUser_RowAdding(object sender, JQGridRowAddEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var mainCompany = e.RowData["PropertyCode"];
            var accessProperties = e.RowData["AccessProperties"];
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
                    AccessProperties = accessProperties,
                    PermissionId = Convert.ToInt32(permissionId)
                };
                UserHelper.AddUserProfile(user);
            }
        }

        protected void JqgridUser_RowEditing(object sender, JQGridRowEditEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var mainCompany = e.RowData["PropertyCode"];
            var accessProperties = e.RowData["AccessProperties"];
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
                    AccessProperties = accessProperties,
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