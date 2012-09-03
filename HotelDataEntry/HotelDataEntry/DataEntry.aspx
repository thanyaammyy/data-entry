<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="DataEntry.aspx.cs" Inherits="HotelDataEntry.DataEntry" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        $(document).ready(function () {
            $('input[name="calendar"]').blur();
            $('input[name="calendar"]').datepicker({
                dateFormat: 'dd/mm/yyyy'
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="headerMenuLabel">
        Data Entry</div>
    <div>
        <table class="TextBlack12">
            <tr>
                <td>
                    Position Date
                </td>
                <td>
                    <input type="text" id="calendar" name="calendar" />
                </td>
            </tr>
            <tr>
                <td>
                    Property
                </td>
                <td>
                    <asp:DropDownList ID="ddlCompany" ToolTip="Select a property" DataSourceID="PropertyDataSource"
                        DataValueField="PropertyId" DataTextField="PropertyCode" Width="120" runat="server">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>
                    Revenue
                </td>
                <td>
                    <asp:DropDownList ID="ddlMenu" ToolTip="Select a revenue" Width="120" runat="server">
                        <asp:ListItem Value="0">Select a menu</asp:ListItem>
                        <asp:ListItem Value="1">Room</asp:ListItem>
                        <asp:ListItem Value="2">F&amp;B</asp:ListItem>
                        <asp:ListItem Value="3">Spa</asp:ListItem>
                        <asp:ListItem Value="4">Other Revenue</asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="PropertyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
            SelectMethod="ListCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper" runat="server">
        </asp:ObjectDataSource>
        <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <%--<cc1:JQGrid ID="JqgridUser" AutoWidth="True" runat="server" Height="80%" OnRowAdding="JqgridUser_RowAdding"
                    OnRowDeleting="JqgridUser_RowDeleting" OnRowEditing="JqgridUser_RowEditing">
                    <Columns>
                        <cc1:JQGridColumn DataField="UserId" PrimaryKey="True" Width="55" Visible="False" />
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
                    <DeleteDialogSettings LeftOffset="497" TopOffset="241"></DeleteDialogSettings>
                    <AddDialogSettings Width="300" Modal="True" TopOffset="250" LeftOffset="500" Height="300"
                        CloseAfterAdding="True" Caption="Add Season" ClearAfterAdding="True"></AddDialogSettings>
                    <EditDialogSettings Width="300" Modal="True" TopOffset="250" LeftOffset="500" Height="300"
                        CloseAfterEditing="True" Caption="Edit Season"></EditDialogSettings>
                    <ClientSideEvents AfterEditDialogShown="populateAlterCompany" AfterAddDialogShown="populateAddAlterCompany" />
                </cc1:JQGrid>--%>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
