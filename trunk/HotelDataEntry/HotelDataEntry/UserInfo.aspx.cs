using System;
using System.Configuration;
using System.Web.UI.WebControls;
using HotelDataEntryLib;
using HotelDataEntryLib.Helper;
using HotelDataEntryLib.Page;

namespace HotelDataEntry
{
    public partial class UserInfo : System.Web.UI.Page
    {
        public int UserId;
        protected void Page_Load(object sender, EventArgs e)
        {
            
            var strSharedSecret = ConfigurationManager.AppSettings["SharedSecret"];
            var decryptUserId = Encryption.DecryptStringAES(Request.QueryString["UserId"], strSharedSecret);
            var decryptEmail = Encryption.DecryptStringAES(Request.QueryString["Email"], strSharedSecret);

            UserId = Convert.ToInt32(decryptUserId);
            if (UserId != 0)
            {
                var userInfo = UserHelper.GetUserInfo(UserId);

                if(!Page.IsPostBack)
                {
                    ddlCompany.SelectedValue = userInfo.PropertyId.ToString();
                }
                
                tbFirstName.Text = userInfo.FirstName;
                tbLastName.Text = userInfo.LastName;
                lbEmail.Text = userInfo.Email;
                
                if (userInfo.AlterPropertyId != 0) ddlAlterCompany.SelectedValue = userInfo.AlterPropertyId.ToString();
            }
            else
            {
                lbEmail.Text = decryptEmail;
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
            var email = lbEmail.Text;
            if(!(string.IsNullOrEmpty(fName)||string.IsNullOrEmpty(lName)||string.IsNullOrEmpty(email)||company.Equals("0")))
            {
                var user = new User
                {
                    FirstName = fName,
                    LastName = lName,
                    Email = email,
                    PropertyId = Convert.ToInt32(company),
                    AlterPropertyId = Convert.ToInt32(ddlAlterCompany.SelectedValue),
                    Status = 1,
                    UpdateDateTime = DateTime.Now
                };
                if (UserId == 0)
                {
                    UserHelper.InsertUserProfile(user);
                }
                else
                {
                    user.UserId = UserId;
                    UserHelper.UpdateUserProfile(user);
                }
                Page.RegisterClientScriptBlock("closeIframe", "<script type=\"text/javascript\" language=\"javascript\">parent.$.fancybox.close();</script>");
            }else
            {
                lbRequired.Visible = true;
            }

        }
    }
}