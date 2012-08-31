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

    }
}