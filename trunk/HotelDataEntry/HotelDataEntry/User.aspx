<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="User.aspx.cs" Inherits="HotelDataEntry.User" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function populateAlterCompany() {
            updateCompanyCallBack($("#PropertyCode").val());
            $("#PropertyCode").bind("change", function (e) { updateCompanyCallBack($("#PropertyCode").val()); });
        }

        function updateCompanyCallBack(mainCompany) {
            $("#AlterCompany").html("<option value=''>Loading Company...</option>").attr("disabled", "disabled");
            var grid = $("#<%= JqgridUser.ClientID %>");
            var userid = grid.getGridParam("selrow");
            $.ajax({
                url: "User.aspx?companyid=" + mainCompany + "&userid=" + userid,
                type: "GET",
                success: function (altercompany) {
                    var company = altercompany.split("|");
                    var alterCompanyHtml = "";
                    for (var i = 0; i < company.length; i++) {
                        var str = company[i].split(",");
                        if (str[2]=="selected") alterCompanyHtml += '<option selected="' + str[2] + '" value="' + str[0] + '">' + str[1] + '</option>';
                        else alterCompanyHtml += '<option value="' + str[0] + '">' + str[1] + '</option>';
                    }
                    $("#AlterCompany").removeAttr("disabled").html(alterCompanyHtml);
                }
            });
        }

        function populateAddAlterCompany() {
            addCompanyCallBack($("#PropertyCode").val());
            $("#PropertyCode").bind("change", function (e) { addCompanyCallBack($("#PropertyCode").val()); });
        }

        function addCompanyCallBack(mainCompany) {
            $("#AlterCompany").html("<option value=''>Loading Company...</option>").attr("disabled", "disabled");
            $.ajax({
                url: "User.aspx?companyid=" + mainCompany + "&userid=-1",
                type: "GET",
                success: function (altercompany) {
                    var company = altercompany.split("|");
                    var alterCompanyHtml = "";
                    for (var i = 0; i < company.length; i++) {
                        var str = company[i].split(",");
                        alterCompanyHtml += '<option value="' + str[0] + '">' + str[1] + '</option>';
                    }
                    $("#AlterCompany").removeAttr("disabled").html(alterCompanyHtml);
                }
            });
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="headerMenuLabel">
        Company Management</div>
    <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:DropDownList ID="ddlCompany" ToolTip="Company is required." DataSourceID="CompanyDataSource"
                DataValueField="PropertyId" DataTextField="PropertyCode" Width="90" runat="server">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="CompanyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
                SelectMethod="ListCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper" runat="server">
            </asp:ObjectDataSource>
            <cc1:JQGrid ID="JqgridUser" AutoWidth="True" runat="server" Height="80%" OnRowAdding="JqgridUser_RowAdding"
                OnRowDeleting="JqgridUser_RowDeleting" OnRowEditing="JqgridUser_RowEditing">
                <Columns>
                    <cc1:JQGridColumn DataField="UserId" Searchable="False" PrimaryKey="True" Width="55" Visible="False" />
                    <cc1:JQGridColumn HeaderText="Company" DataField="PropertyCode" EditorControlID="ddlCompany"
                        EditType="DropDown" Editable="True" TextAlign="Center">
                        <EditClientSideValidators>
                            <cc1:RequiredValidator />
                        </EditClientSideValidators>
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Firstname" DataField="FirstName" Editable="True" TextAlign="Center">
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Lastname" DataField="LastName" Editable="True" TextAlign="Center">
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Email" DataField="Email" Editable="True" TextAlign="Center">
                        <EditClientSideValidators>
                            <cc1:EmailValidator />
                        </EditClientSideValidators>
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Alternative Company" DataField="AlterCompany" Editable="true"
                        EditType="DropDown" EditValues="Select a company" TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="Permission" DataField="PermissionId" Editable="True"
                        EditType="DropDown" EditValues="1:Admin" TextAlign="Center">
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Status" DataField="StatusLabel" EditType="DropDown"
                        EditValues="0:InActive;1:Active" Editable="True" TextAlign="Center" />
                </Columns>
                <AddDialogSettings CloseAfterAdding="False" />
                <EditDialogSettings CloseAfterEditing="True" />
                <ToolBarSettings ShowEditButton="True" ShowDeleteButton="true" ShowAddButton="True"
                    ShowRefreshButton="True" ShowSearchButton="True" />
                <AppearanceSettings ShowRowNumbers="true" />
                <PagerSettings PageSize="20" />
                <DeleteDialogSettings LeftOffset="497" TopOffset="241"></DeleteDialogSettings>
                <AddDialogSettings Width="300" Modal="True" TopOffset="250" LeftOffset="500" Height="300"
                    CloseAfterAdding="True" Caption="Add Season" ClearAfterAdding="True"></AddDialogSettings>
                <EditDialogSettings Width="300" Modal="True" TopOffset="250" LeftOffset="500" Height="300"
                    CloseAfterEditing="True" Caption="Edit Season"></EditDialogSettings>
                <ClientSideEvents AfterEditDialogShown="populateAlterCompany" AfterAddDialogShown="populateAddAlterCompany" />
            </cc1:JQGrid>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>