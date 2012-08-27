<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="Currency.aspx.cs" Inherits="HotelDataEntry.Currency" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="headerMenuLabel">
        Currency Management</div>
        <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <cc1:JQGrid ID="JqgridCurrency" AutoWidth="True" runat="server"  Height="80%" OnRowAdding="JqgridCurrency_RowAdding"
                    OnRowDeleting="JqgridCurrency_RowDeleting" OnRowEditing="JqgridCurrency_RowEditing">
                    <Columns>
                        <cc1:JQGridColumn DataField="CurrencyId" PrimaryKey="True" Width="55" Visible="False" />
                        <cc1:JQGridColumn HeaderText="Currency Code" DataField="CurrencyCode" Editable="True"
                            TextAlign="Center" />
                        <cc1:JQGridColumn HeaderText="Currency Name" DataField="CurrencyName" Editable="True"
                            TextAlign="Center" />
                        <cc1:JQGridColumn HeaderText="Base Currency" DataField="IsBaseLabel" Editable="True"
                            EditType="DropDown" EditValues="0:False;1:True" TextAlign="Center" />
                        <cc1:JQGridColumn HeaderText="Conversion Rate" DataField="ConversionRate" Editable="True"
                            TextAlign="Center" />
                        <cc1:JQGridColumn HeaderText="Status" DataField="StatusLabel" Editable="True" EditType="DropDown"
                            EditValues="0:InActive;1:Active" TextAlign="Center" />
                    </Columns>
                    <AddDialogSettings CloseAfterAdding="False" />
                    <EditDialogSettings CloseAfterEditing="True"  />
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
