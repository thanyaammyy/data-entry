<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="Company.aspx.cs" Inherits="HotelDataEntry.Company" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="headerMenuLabel">
        Compamy Management</div>
    <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <cc1:JQGrid ID="JqgridCompany" AutoWidth="True" runat="server" Height="80%">
                <Columns>
                    <cc1:JQGridColumn DataField="PropertyId" PrimaryKey="True" Width="55" Visible="False" />
                    <cc1:JQGridColumn HeaderText="Company Code" DataField="PropertyCode" Editable="True"
                        TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="Brand" DataField="BrandName" Editable="True" TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="CompanyName" DataField="CompanyName" Editable="True"
                        TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="Conversion Rate" DataField="ConversionRate" Editable="True"
                        TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="Currency" DataField="Currency" Editable="True" TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="Status" DataField="Status" Editable="True" TextAlign="Center" />
                </Columns>
                <AddDialogSettings CloseAfterAdding="False" />
                <EditDialogSettings CloseAfterEditing="True" />
                <ToolBarSettings ShowEditButton="True" ShowDeleteButton="true" ShowAddButton="True"
                    ShowRefreshButton="True" ShowSearchButton="True" />
                <AppearanceSettings ShowRowNumbers="true" />
                <DeleteDialogSettings LeftOffset="497" TopOffset="241"></DeleteDialogSettings>
                <AddDialogSettings Width="300" Modal="True" TopOffset="250" LeftOffset="500" Height="300"
                    CloseAfterAdding="True" Caption="Add Season" ClearAfterAdding="True"></AddDialogSettings>
                <EditDialogSettings Width="300" Modal="True" TopOffset="250" LeftOffset="500" Height="300"
                    CloseAfterEditing="True" Caption="Edit Season"></EditDialogSettings>
            </cc1:JQGrid>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
