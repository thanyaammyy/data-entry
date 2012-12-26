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
            //Revenue
            Session["rPropertyId"] = null;
            Session["MonthYear"] = null;

            //Budget
            Session["bPropertyId"] = null;
            Session["year"] = null;

            if (Session["permission"] != null)
            {
                if (!string.IsNullOrEmpty(Session["permission"].ToString()))
                {
                    if (Convert.ToInt32(Session["permission"]) != 4)//administrator permission
                    {
                        Response.Redirect("Login.aspx");
                    }
                }
            }
            else
            {
                Response.Redirect("Login.aspx");
            }

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
            string accessProperties;
            if(userId==0)
            {
                accessProperties = "";
            }
            else
            {
                var user = UserHelper.GetUserInfo(userId);
                accessProperties = user.AccessProperties;
            }

            var listProperties = PropertyHelper.Properites();
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
            var permissionId = e.RowData["PermissionName"];
            var position = e.RowData["Position"];
            var username = e.RowData["Username"];
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
                    PermissionId = Convert.ToInt32(permissionId),
                    Username = username,
                    Position = position
                };
                if (UserHelper.IsUserExist(user.Username)) return;
                UserHelper.AddUserProfile(user);
            }
        }

        protected void JqgridUser_RowEditing(object sender, JQGridRowEditEventArgs e)
        {
            var status = e.RowData["StatusLabel"];
            var mainCompany = e.RowData["PropertyCode"];
            var accessProperties = e.RowData["AccessProperties"];
            var permissionId = e.RowData["PermissionName"];
            var position = e.RowData["Position"];
            var userId = e.RowKey;
            var username = e.RowData["Username"];
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
                    PermissionId = Convert.ToInt32(permissionId),
                    Username = username,
                    Position = position
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