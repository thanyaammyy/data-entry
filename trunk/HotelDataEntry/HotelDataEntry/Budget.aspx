﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true" CodeBehind="Budget.aspx.cs" Inherits="HotelDataEntry.Budget" %>
<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ui-datepicker-calendar
        {
            display: none;
        }
        .ui-datepicker-month {
            display: none;
        }
        .ui-datepicker-prev {
            display: none;
        }
        .ui-datepicker-next {
            display: none;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function validateCurrency(value, column) {
            var pattern = /(?:^\d{1,3}(?:\.?\d{3})*(?:,\d{2})?$)|(?:^\d{1,3}(?:,?\d{3})*(?:\.\d{2})?$)/;
            if (pattern.test(value))
                return [true, ""];
            else
                return [false, "Please enter a number format"];
        }
        function validateRooms(value, column) {
            var pattern = /(?:^[1-9]\d*$)/;
            if (pattern.test(value))
                return [true, ""];
            else
                return [false, "Please enter a valid number format"];
        }

        function disableDate(value, column) {
            $("#PositionMonth").attr("disabled", "true");
        }

        $(document).ready(function () {
            $('input[name="calendar"]').blur();
            $('input[name="calendar"]').datepicker({
                changeMonth: false,
                changeYear: true,
                yearRange: "2008:2020",
                dateFormat: 'yy',
                onClose: function (dateText, inst) {
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, 1, 1));
                    document.getElementById("<%= hiddenMonthYear.ClientID %>").value = year;
                }
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" id="hiddenMonthYear" runat="server" />
    <div class="headerMenuLabel">
        Budget Entry</div>
        
    <div>
        <table class="TextBlack12" cellpadding="3" cellspacing="3">
            <tr>
                <td>
                    Property
                </td>
                <td>
                    <asp:DropDownList ID="ddlCompany" ToolTip="Select a property" DataSourceID="PropertyDataSource"
                        AutoPostBack="True" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged" OnDataBound="CurrencyLabel_DataBound"
                        DataValueField="PropertyId" DataTextField="PropertyCode" Width="150" runat="server">
                    </asp:DropDownList>
                    <asp:Label ID="lbCompany" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Year
                </td>
                <td>
                    <%if (Session["year"] == null)
                      {
                    %>
                    <input type="text" id="calendar" name="calendar" />
                    <%}
                      else
                      {%>
                    <input type="text" id="Text1" name="calendar" value="<%= Session["year"] %>" />
                    <% } %>
                    <asp:Label ID="lbCalendar" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Currency
                </td>
                <td>
                    <asp:Label runat="server" OnLoad="CurrencyLabel_DataBound" ID="lbCurerncy"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button runat="server" ID="btnCreateForm" OnClick="btnCreateForm_Click" Text="Show" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Label Visible="False" runat="server" ID="lbError" Text="Please select the required feilds"
                        CssClass="redText"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:ObjectDataSource ID="PropertyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
            SelectMethod="AccessProperty" TypeName="HotelDataEntryLib.Page.PropertyHelper" runat="server">
            <SelectParameters>
                <asp:SessionParameter Name="userId" SessionField="userId" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <div style="padding-top: 20px; display: none" runat="server" id="divJqgrid">
            <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <cc1:JQGrid ID="JqGridBudgetEntry" AutoWidth="True" runat="server" Height="80%" OnRowEditing="JqGridDataEntry_RowEditing">
                        <Columns>
                            <cc1:JQGridColumn DataField="BudgetId" Searchable="False" PrimaryKey="True" Width="55"
                                Visible="False" />
                            <cc1:JQGridColumn DataField="HotelBudgetId" Searchable="False" Width="55" Visible="False" />
                            <cc1:JQGridColumn HeaderText="Month/Year" DataField="PositionMonth" Editable="True" 
                                TextAlign="Center" FooterValue="Total:">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn CssClass="occupied" Width="200" HeaderText="Occupied Rooms (rms)" DataField="OccupiedRoom" Editable="True" TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateRooms" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Total Room Revenues" DataField="TotalRoomRevenues" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Food" DataField="Food" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Beverage" DataField="Beverage" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Service" DataField="Service" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Product" DataField="SpaProduct" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Others" DataField="Others" Editable="True" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Total" DataField="Total" Editable="False" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                        </Columns>
                        <HeaderGroups>
                            <cc1:JQGridHeaderGroup StartColumnName="OccupiedRoom" TitleText="Room" NumberOfColumns="2"/>
                            <cc1:JQGridHeaderGroup StartColumnName="Food" TitleText="Food & Beverage" NumberOfColumns="2"/>
                            <cc1:JQGridHeaderGroup StartColumnName="Service" TitleText="Spa" NumberOfColumns="2"/>
                        </HeaderGroups>
                        <ToolBarSettings ShowRefreshButton="True" ShowEditButton="True" />
                        <EditDialogSettings  Modal="True" Width="350" TopOffset="350" LeftOffset="500" CloseAfterEditing="True"
                            Caption="Edit Revenue Entry"></EditDialogSettings>
                        <PagerSettings PageSize="32" />
                        <AppearanceSettings ShowRowNumbers="true" ShowFooter="true" HighlightRowsOnHover="True" />
                        <ClientSideEvents AfterEditDialogShown="disableDate"></ClientSideEvents>
                    </cc1:JQGrid>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>