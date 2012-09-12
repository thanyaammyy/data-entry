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
        function validateCurrency(value, column) {
            var pattern = /(?:^\d{1,3}(?:\.?\d{3})*(?:,\d{2})?$)|(?:^\d{1,3}(?:,?\d{3})*(?:\.\d{2})?$)/;
            if (pattern.test(value))
                return [true, ""];
            else
                return [false, "Please enter a number format"];
        }

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
                    var intMonth = parseInt(month) + 1;
                    var strMonth = intMonth > 10 ? intMonth : 0 + "" + intMonth;
                    var my = strMonth + "/" + year;
                    document.getElementById("<%= hiddenMonthYear.ClientID %>").value = my;
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" id="hiddenMonthYear" runat="server" />
    <div class="headerMenuLabel">
        Data Entry</div>
    <div>
        <table class="TextBlack12" cellpadding="3" cellspacing="3">
            <tr>
                <td>
                    Position Date
                </td>
                <td>
                    <%if (Session["MonthYear"] == null)
                      {
                    %>
                    <input type="text" id="calendar" name="calendar" />
                    <%}
                      else
                      {%>
                    <input type="text" id="Text1" name="calendar" value="<%= Session["MonthYear"] %>" />
                    <% } %>
                    <asp:Label ID="lbCalendar" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Property
                </td>
                <td>
                    <asp:DropDownList ID="ddlCompany" ToolTip="Select a property" DataSourceID="PropertyDataSource" AutoPostBack="True" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"
                        DataValueField="PropertyId" DataTextField="PropertyCode" Width="150" runat="server">
                    </asp:DropDownList>
                    <asp:Label ID="lbCompany" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Revenue
                </td>
                <td>
                    <table border="0" cellspacing="0">
                        <tr>
                            <td>
                                <asp:DropDownList ID="ddlMenu" ToolTip="Select a revenue" Width="150" runat="server"
                                    AutoPostBack="True" DataSourceID="RevenueDataSource" DataValueField="DataEntryTypeId"
                                    DataTextField="DataEntryTypeName" OnSelectedIndexChanged="ddlMenu_SelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <asp:UpdatePanel ID="updateRevenuePanel" UpdateMode="Conditional" runat="server">
                                <ContentTemplate>
                                    <td>
                                        <asp:DropDownList ID="ddlSubMenu" Width="150" runat="server" DataSourceID="SubRevenueDataSource" OnSelectedIndexChanged="ddlSubMenu_SelectedIndexChanged"
                                            AutoPostBack="True" DataValueField="DataEntrySubTypeId" Enabled="False" DataTextField="DataEntrySubTypeName">
                                        </asp:DropDownList>
                                        <asp:Label ID="lbMenu" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                                    </td>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button runat="server" ID="btnCreateForm" OnClick="btnCreateForm_Click" Text="Show" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label Visible="False" runat="server" ID="lbError" Text="Please select a required feilds"
                        CssClass="redText"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="PropertyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
            SelectMethod="ListCompany" TypeName="HotelDataEntryLib.Page.PropertyHelper" runat="server">
        </asp:ObjectDataSource>
        <asp:ObjectDataSource ID="RevenueDataSource" DataObjectTypeName="HotelDataEntryLib.DataEntryType"
            SelectMethod="ListDataEntryType" TypeName="HotelDataEntryLib.Page.DataEntryTypeHelper"
            runat="server"></asp:ObjectDataSource>
        <asp:ObjectDataSource ID="SubRevenueDataSource" DataObjectTypeName="HotelDataEntryLib.DataEntrySubType"
            SelectMethod="ListDataEntrySubType" TypeName="HotelDataEntryLib.Page.DataEntryTypeHelper"
            runat="server">
            <SelectParameters>
                <asp:SessionParameter Name="DataEntryTypeId" SessionField="DataEntryTypeId" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <div style="padding-top: 20px; display: none" runat="server" id="divJqgrid">
            <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <cc1:JQGrid ID="JqGridDataEntry" AutoWidth="True" runat="server" Height="80%" OnRowEditing="JqGridDataEntry_RowEditing">
                        <Columns>
                            <cc1:JQGridColumn DataField="DataEntryId" Searchable="False" PrimaryKey="True" Width="55"
                                Visible="False" />
                            <cc1:JQGridColumn DataField="HotelEntryId" Searchable="False" Width="55" Visible="False" />
                            <cc1:JQGridColumn HeaderText="Date" DataField="PositionDate" Editable="False" DataType="DateTime" TextAlign="Center" DataFormatString="{0:dd/MM/yy}"
                                FooterValue="Total:">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Actual" DataField="ActualData" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Budget" DataField="Budget" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                        </Columns>
                        <ToolBarSettings ShowRefreshButton="True" ShowSearchButton="True" ShowEditButton="True" />
                        <PagerSettings PageSize="32" />
                        <AppearanceSettings ShowRowNumbers="true" ShowFooter="true" />
                        <EditDialogSettings Width="300" Modal="True" TopOffset="500" LeftOffset="500" CloseAfterEditing="True"
                            Caption="Edit Data Entry"></EditDialogSettings>
                    </cc1:JQGrid>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
