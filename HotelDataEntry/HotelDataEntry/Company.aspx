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
                        TextAlign="Center">
                        <EditClientSideValidators>
                            <cc1:RequiredValidator />
                        </EditClientSideValidators>
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Brand" DataField="BrandName" EditorControlID="ddlBrand"
                        Editable="True" TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="Company Name" DataField="PropertyName" Editable="True"
                        TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="Currency" DataField="CurrencyCode" Editable="True"
                        TextAlign="Center" />
                    <cc1:JQGridColumn HeaderText="Status" DataField="StatusLabel" EditType="DropDown"
                        EditValues="0:InActive;1:Active" Editable="True" TextAlign="Center" />
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
            <asp:DropDownList ID="ddlBrand" ToolTip="Brand is required." DataSourceID="BrandDataSource"
                DataTextField="BrandCodeWithName"  Width="90" runat="server">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="BrandDataSource" DataObjectTypeName="HotelDataEntryLib.Brand"
                SelectMethod="ListBrand" TypeName="HotelDataEntryLib.Page.BrandHelper" runat="server">
            </asp:ObjectDataSource>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
