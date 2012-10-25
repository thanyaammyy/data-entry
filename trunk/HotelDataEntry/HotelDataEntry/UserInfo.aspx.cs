using System;
using System.Configuration;
using System.Net.Mail;
using HotelDataEntryLib.Helper;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class UserInfo : System.Web.UI.Page
    {
        public int UserId;
        public int UserPermissionId;
        public int UserPropertyId;
        public string UserName;
        public string AccessProperty;
        public string Position;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            var strSharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            var decryptKey = Encryption.DecryptStringAES(Request.QueryString["key"], strSharedSecret);

            if(decryptKey.Contains("&"))
            {
                var str = decryptKey.Split('&');
                UserId = Convert.ToInt32(str[0]);
            }
            else
            {
                UserId = Convert.ToInt32(decryptKey);
            }
            
            if (UserId != 0)
            {
                var userInfo = UserHelper.GetUserInfo(UserId);
                UserPermissionId = userInfo.PermissionId;/*Do something*/
                UserPropertyId = userInfo.PropertyId;
                UserName = userInfo.Username;
                AccessProperty = userInfo.AccessProperties;
                Position = userInfo.Position;

                ddlProperty.SelectedValue = userInfo.PropertyId.ToString();
                ddlProperty.Enabled = false;
                if(!Page.IsPostBack)
                {
                    tbFirstName.Text = userInfo.FirstName;
                    tbLastName.Text = userInfo.LastName;
                    tbEmail.Text = userInfo.Email;
                }
                tbPosition.Visible = false;
                lbPosition.Visible = true;
                lbPositionRequired.Visible = false;
                lbPosition.Text = userInfo.Position;
                lbAccessProperty.Text = userInfo.AccessProperties;
                lbUserPermission.Text = PermissionHelper.GetPermission(userInfo.PermissionId).PermissionName;
            }
            
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            var fName = tbFirstName.Text;
            var lName = tbLastName.Text;
            var email = tbEmail.Text;
            var position = tbPosition.Text;
            var propertyId = string.IsNullOrEmpty(ddlProperty.SelectedValue)?0:Convert.ToInt32(ddlProperty.SelectedValue);
            if(!(string.IsNullOrEmpty(fName)||string.IsNullOrEmpty(lName)||string.IsNullOrEmpty(email)||propertyId==0||string.IsNullOrEmpty(position)))
            {
                if(IsValidEmail(email))
                {
                    var user = new HotelDataEntryLib.User()
                    {
                        FirstName = fName,
                        LastName = lName,
                        Email = email,
                        Position = position
                    };
                    if (UserId == 0)
                    {
                        user.Status = 0;
                        user.PermissionId = 1;
                        user.PropertyId = Convert.ToInt32(ddlProperty.SelectedValue);
                        user.Username = Session["UserSession"].ToString();
                        user.AccessProperties = "N/A";
                        UserHelper.AddUserProfile(user);
                    }
                    else
                    {
                        user.UserId = UserId;
                        user.Status = 1;
                        user.PropertyId = UserPropertyId;
                        user.PermissionId = UserPermissionId;
                        user.Username = UserName;
                        user.AccessProperties = AccessProperty;
                        UserHelper.UpdateUserProfile(user);
                    }
                    Page.RegisterClientScriptBlock("closeIframe", "<script type=\"text/javascript\" language=\"javascript\">parent.$.fancybox.close(parent.location.reload(true));</script>");
                }
                else
                {
                    lbEmailError.Visible = true;
                    lbRequired.Visible = false;
                }

            }else
            {
                lbEmailError.Visible = false;
                lbRequired.Visible = true;
            }

        }

        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                var m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}