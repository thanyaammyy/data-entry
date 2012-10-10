using System;
using System.Configuration;
using System.Net.Mail;
using System.Web.UI.WebControls;
using HotelDataEntryLib.Helper;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class UserInfo : System.Web.UI.Page
    {
        public int UserId;
        public int UserPermissionId;
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
                UserPermissionId = userInfo.PermissionId;
                if(!Page.IsPostBack)
                {
                    ddlCompany.SelectedValue = userInfo.PropertyId.ToString();
                    //if (userInfo.AlterPropertyId != 0) ddlAlterCompany.SelectedValue = userInfo.AlterPropertyId.ToString();
                }
                
                tbFirstName.Text = userInfo.FirstName;
                tbLastName.Text = userInfo.LastName;
                tbEmail.Text = userInfo.Email;
            }
            
        }

        protected void ddlCompany_SelectedIndexChanged1(object sender, EventArgs e)
        {
            if (((DropDownList)sender).SelectedValue != "")
            {
                Session["propertyid"] = ddlCompany.SelectedValue;
            }
            updateAlterPanel.Update();
        }

        protected void btnUpdateProfile_Click(object sender, EventArgs e)
        {
            var fName = tbFirstName.Text;
            var lName = tbLastName.Text;
            var company = ddlCompany.SelectedValue;
            var email = tbEmail.Text;
            if(!(string.IsNullOrEmpty(fName)||string.IsNullOrEmpty(lName)||string.IsNullOrEmpty(email)||company.Equals("0")))
            {
                if(IsValidEmail(email))
                {
                    var user = new HotelDataEntryLib.User()
                    {
                        FirstName = fName,
                        LastName = lName,
                        Email = email,
                        PropertyId = Convert.ToInt32(company),
                        //AlterPropertyId = Convert.ToInt32(ddlAlterCompany.SelectedValue),
                        Status = 1,
                        UpdateDateTime = DateTime.Now
                    };
                    if (UserId == 0)
                    {
                        user.PermissionId = 3;
                        user.Username = Session["UserSession"].ToString();
                        UserHelper.AddUserProfile(user);
                    }
                    else
                    {
                        user.UserId = UserId;
                        user.PermissionId = UserPermissionId;
                        UserHelper.UpdateUserProfile(user);
                    }
                    Page.RegisterClientScriptBlock("closeIframe", "<script type=\"text/javascript\" language=\"javascript\">parent.$.fancybox.close();</script>");
                }
                
            }else
            {
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