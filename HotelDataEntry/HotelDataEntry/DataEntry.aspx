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
        function reloadGrid() {
            alert("reload");
            var grid = $("#<%= JqGridDataEntry.ClientID %>");
            grid.trigger("reloadGrid");
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
                    var my = intMonth + "/" + year;
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
                    <asp:DropDownList ID="ddlCompany" ToolTip="Select a property" DataSourceID="PropertyDataSource"
                        DataValueField="PropertyId" DataTextField="PropertyCode" Width="120" runat="server">
                    </asp:DropDownList>
                    <asp:Label ID="lbCompany" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Revenue
                </td>
                <td>
                    <asp:DropDownList ID="ddlMenu" ToolTip="Select a revenue" Width="120" runat="server"
                        DataSourceID="RevenueDataSource" DataValueField="DataEntryTypeId" DataTextField="DataEntryTypeName">
                    </asp:DropDownList>
                    <asp:Label ID="lbMenu" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button runat="server" ID="btnCreateForm" OnClick="btnCreateForm_Click" Text="Show" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label Visible="False" runat="server" ID="lbError" Text="Please select any required feilds"
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
        <div style="padding-top: 20px; display: none" runat="server" id="divJqgrid">
            <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <cc1:JQGrid ID="JqGridDataEntry" AutoWidth="True" runat="server" Height="80%" OnRowEditing="JqGridDataEntry_RowEditing"
                        OnDataRequested="JqGridDataEntry_DataRequested">
                        <Columns>
                            <cc1:JQGridColumn HeaderText="Edit" Width="32" Searchable="False" TextAlign="Center" EditActionIconsDeleteEnabled="False"
                                EditActionIconsEditEnabled="True" EditActionIconsColumn="True" />
                            <cc1:JQGridColumn DataField="DataEntryId" Searchable="False" PrimaryKey="True" Width="55"
                                Visible="False" />
                            <cc1:JQGridColumn DataField="HotelEntryId" Searchable="False" Width="55" Visible="False" />
                            <cc1:JQGridColumn HeaderText="Date" DataField="PositionDate" Editable="False" TextAlign="Center"
                                FooterValue="Total:">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Actual" DataField="ActualData" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:NumberValidator />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Budget" DataField="Budget" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:NumberValidator />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="YTD Actual" DataField="YTDActual" Editable="True" TextAlign="Right"
                                DataFormatString="{0:#,##0.00;(#,##0.00);0}">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:NumberValidator />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="YTD Budget" DataField="YTDBudget" Editable="True" TextAlign="Right"
                                DataFormatString="{0:#,##0.00;(#,##0.00);0}">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:NumberValidator />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                        </Columns>
                        <ToolBarSettings ShowRefreshButton="True" ShowSearchButton="True" />
                        <PagerSettings PageSize="32" />
                        <AppearanceSettings ShowRowNumbers="true" ShowFooter="true" />
                        <ClientSideEvents AfterSubmitCell="reloadGrid"></ClientSideEvents>
                    </cc1:JQGrid>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
