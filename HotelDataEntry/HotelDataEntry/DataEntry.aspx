<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="DataEntry.aspx.cs" Inherits="HotelDataEntry.DataEntry" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ui-datepicker-calendar
        {
            display: none;
        }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $('input[name="calendar"]').blur();
            $('input[name="calendar"]').datepicker({
                changeMonth: true,
                changeYear: true,
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                }
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
            <tr>
                <td colspan="2">
                    <asp:Button runat="server" ID="btnCreateForm" onclick="btnCreateForm_Click" Text="Show"/>
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="PropertyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
            SelectMethod="ListCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper" runat="server">
        </asp:ObjectDataSource>
        <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
            <ContentTemplate>
                <cc1:JQGrid ID="JqGridDataEntry" AutoWidth="True" runat="server" Height="80%">
                    <Columns>
                        <cc1:JQGridColumn DataField="HotelEntryId" PrimaryKey="True" Width="55" Visible="False" />
                        <cc1:JQGridColumn HeaderText="Edit" Width="40" TextAlign="Center" EditActionIconsColumn="True" />
                        <cc1:JQGridColumn HeaderText="Date" DataField="PositionDate" EditType="TextBox" DataType="DateTime"
                            DataFormatString="{0:dd/MM/yyyy}" Editable="True" TextAlign="Center">
                            <EditClientSideValidators>
                                <cc1:RequiredValidator />
                            </EditClientSideValidators>
                        </cc1:JQGridColumn>
                        <cc1:JQGridColumn HeaderText="Actual" DataField="Actual" Editable="True" TextAlign="Right">
                            <EditClientSideValidators>
                                <cc1:RequiredValidator />
                                <cc1:NumberValidator />
                            </EditClientSideValidators>
                        </cc1:JQGridColumn>
                        <cc1:JQGridColumn HeaderText="Budget" DataField="Budget" Editable="True" TextAlign="Right">
                            <EditClientSideValidators>
                                <cc1:RequiredValidator />
                                <cc1:NumberValidator />
                            </EditClientSideValidators>
                        </cc1:JQGridColumn>
                        <cc1:JQGridColumn HeaderText="YTD Actual" DataField="YTDActual" Editable="True" TextAlign="Right">
                            <EditClientSideValidators>
                                <cc1:EmailValidator />
                            </EditClientSideValidators>
                        </cc1:JQGridColumn>
                        <cc1:JQGridColumn HeaderText="YTD Budget" DataField="YTDBudget" Editable="True" TextAlign="Right">
                            <EditClientSideValidators>
                                <cc1:EmailValidator />
                            </EditClientSideValidators>
                        </cc1:JQGridColumn>
                    </Columns>
                    <AddDialogSettings CloseAfterAdding="False" />
                    <EditDialogSettings CloseAfterEditing="True" />
                    <ToolBarSettings ShowAddButton="True"
                        ShowRefreshButton="True" ShowSearchButton="True" />
                    <AppearanceSettings ShowRowNumbers="true" />
                    <DeleteDialogSettings LeftOffset="497" TopOffset="241"></DeleteDialogSettings>
                    <ClientSideEvents RowSelect="bindCalendarDialog" AfterAddDialogShown="bindCalendarDialog2"
                        AfterEditDialogShown="bindCalendarDialog2" />
                </cc1:JQGrid>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
