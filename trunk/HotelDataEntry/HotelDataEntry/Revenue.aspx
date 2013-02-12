<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="Revenue.aspx.cs" Inherits="HotelDataEntry.DataEntry" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .ui-datepicker-calendar
        {
            display: none;
        }
        .weekend 
        {
            background-color: lightskyblue; background-image: none;
        }
    </style>
    <script type="text/javascript" language="javascript">
        function firstLogin() {
            $(".fancybox").fancybox({
                'width': 432,
                'closeBtn': false,
                'modal': true,
                'height': 'auto',
                scrolling: 'no',
                helpers: {
                    title: null
                }
            }).trigger('click');
        }

        function validateCurrency(value, column) {
            var pattern = /(?:^\d{1,3}(?:\.?\d{2})*(?:,\d{2})?$)|(?:^\d{1,3}(?:,?\d{3})*(?:\.\d{2})?$)/;
            if (pattern.test(value))
                return [true, ""];
            else
                return [false, "Please enter a valid number format"];
        }

        function validateRooms(value, column) {
            var occupancy = "";
            if(value.indexOf("%") > -1) {
                occupancy=value.substring(0, value.length-1);
            }
            else {
                occupancy = value;
            }
            var pattern = /^\d+(\.\d{1,2})?$/;
            if (pattern.test(occupancy))
                if(occupancy>100)
                    return [false, "The occupancy should not more than 100%"];
                else
                    return [true, ""];
            else
                return [false, "Please enter a valid number format"];
        }

        function disableDate(value, column) {
            $("#PositionDate").attr("disabled", "true");
            $("#tr_UpdateDateTimeMillisecond").hide();
            $("#tr_DateNowMillisecond").hide();
            $(".navButton").html("");
            var permission = <% = Session["permission"] %>;
            if(permission=="2") {//permissionId =2 is auditor role

                var updateDatetimeMillisec = value[0].UpdateDateTimeMillisecond.value;
                var dateNowMillisecond = value[0].DateNowMillisecond.value;
                var strRoomRevenue = value[0].RoomRevenue.value;
                var strFBRevenue = value[0].FBRevenue.value;
                var strSpaRevenue = value[0].SpaRevenue.value;
                var strOthers = value[0].Others.value;
                var strOccupancy = value[0].OccupancyRoom.value;

                var occupancy = 0;
                var FBRevenue = 0;
                var roomRevenue = 0;
                var spaRevenue = 0;
                var others = 0;
                
                if(strOccupancy) {
                    var str = "";
                    if(strOccupancy.indexOf("%") > -1) {
                        str=strOccupancy.substring(0, strOccupancy.length-1);
                    }
                    else {
                        str = strOccupancy;
                    }
                    occupancy = parseFloat(str);
                }
                if(strRoomRevenue) roomRevenue = parseFloat(strRoomRevenue);
                if(strFBRevenue) FBRevenue = parseFloat(strFBRevenue);
                if(strSpaRevenue) spaRevenue = parseFloat(strSpaRevenue);
                if(strOthers) others = parseFloat(strOthers);
                
                if(FBRevenue>0&&roomRevenue>0&&spaRevenue>0&&others>0&&occupancy>0) {
                    if(dateNowMillisecond-updateDatetimeMillisec>864000000000) {
                        $("#sData").css( 'cursor', 'default' );
                        $("#sData").attr("disabled", "true");
                    }
                }
            }
        }

        getColumnIndexByName = function(mygrid, columnName) {
            var cm = mygrid.jqGrid('getGridParam', 'colModel');
            for (var i = 0, l = cm.length; i < l; i++) {
                if (cm[i].name === columnName) {
                    return i; // return the index
                }
            }
            return -1;
        };

        function onLoadComplete(data) {
            var iCol = getColumnIndexByName($(this), 'Day'),
                cRows = this.rows.length, row, className;

            for (var iRow = 0; iRow < cRows; iRow++) {
                row = this.rows[iRow];
                className = row.className;
                if ($.inArray('jqgrow', className.split(' ')) > 0) { 
                    var x = $(row.cells[iCol]);
                    if (x[0].innerHTML == "0" || x[0].innerHTML=="6") {
                        if ($.inArray('weekend', className.split(' ')) === -1) {
                            row.className = className + ' weekend';
                        }
                    }
                }
            }
        }

        function formatColor(cellValue, options, rowObject) {
            var cellHtml = "";
            var value = cellValue==""?cellValue:cellValue+"%";
            if(rowObject[3]=="0"||rowObject[3]=="6") {
                cellHtml = "<span originalValue='" +
                                    value + "'>" + value + "</span>";
            }
            else {
                cellHtml = "<span class='cellWithoutBackground' style='background-color:thistle' originalValue='" +
                                    value + "'>" + value + "</span>";
            }
            return cellHtml;
        }

        function unformatColor(cellValue, options, cellObject) {
            return $(cellObject.html()).attr("originalValue");
        }

        function onClickCalendar() {
            $("#hiddenReport").css("display", "none");
            $("#hiddenJqgrid").css("display", "none");
        }

        $(document).ready(function () {
            $('input[name="calendar"]').blur();
            $('input[name="calendar"]').datepicker({
                changeMonth: true,
                changeYear: true,
                yearRange: "-1:+1",
                dateFormat: 'mm/yy',
                onClose: function (dateText, inst) {
                    var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                    var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                    $(this).datepicker('setDate', new Date(year, month, 1));
                    var intMonth = parseInt(month) + 1;
                    var strMonth = intMonth >= 10 ? intMonth : 0 + "" + intMonth;
                    var my = strMonth + "/" + year;
                    document.getElementById("<%= hiddenMonthYear.ClientID %>").value = my;
                },
                beforeShow: function (textbox, instance) {
                    instance.dpDiv.css({
                        marginTop: (-textbox.offsetHeight) + 'px',
                        marginLeft: textbox.offsetWidth + 'px'
                    });
                }
            });
            
            $("#btnMtd").click(function() {
                $(".various").trigger('click'); 
            });

            $(".various").fancybox({
                    maxWidth: 1200,
                    maxHeight: 550,
                    fitToView: true,
                    width: '90%',
                    height: '80%',
                    autoSize: false,
                    openEffect: 'none',
                    closeEffect: 'none',
                    showCloseButton: true,
                    overlayShow: true,
                    hideOnOverlayClick: false,
                    hideOnContentClick: false,
                    enableEscapeButton: false,
                    helpers: {
                        title: null
                    }
           });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <input type="hidden" id="hiddenMonthYear" runat="server" />
    <div class="headerMenuLabel">
        Daily Revenue Entry</div>
    <div>
        <table class="TextBlack12" cellpadding="3" cellspacing="3">
            <tr>
                <td>
                    Property
                </td>
                <td>
                    <asp:DropDownList ID="ddlCompany" ToolTip="Select a property" DataSourceID="PropertyDataSource"
                        AutoPostBack="True" OnSelectedIndexChanged="ddlCompany_SelectedIndexChanged"
                        DataValueField="PropertyId" DataTextField="PropertyCode" Width="150" OnDataBound="CurrencyLabel_DataBound"
                        runat="server">
                    </asp:DropDownList>
                    <asp:Label ID="lbCompany" Visible="False" CssClass="asteric" runat="server">*</asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    Position Date
                </td>
                <td>
                    <%if (Session["MonthYear"] == null)
                      {
                    %>
                    <input type="text" id="calendar" name="calendar" onclick="onClickCalendar();" />
                    <%}
                      else
                      {%>
                    <input type="text" id="Text1" name="calendar" onclick="onClickCalendar();" value="<%= Session["MonthYear"].ToString() %>" />
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
                <td align="center">
                    <div id="divReport" runat="server" style="display: none">
                        <div id="hiddenReport">
                            <input type="button" id="btnMtd" value="Switch to Month-To-Date View"/>
                            <div style="display: none">
                                <a data-fancybox-type="iframe"  href="Reports.aspx" class="various">Switch to Month-To-Date View</a>
                            </div>
                        </div>
                    </div>
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
            SelectMethod="AccessProperty" TypeName="HotelDataEntryLib.Page.PropertyHelper"
            runat="server">
            <SelectParameters>
                <asp:SessionParameter Name="userId" SessionField="userId" Type="Int32" />
            </SelectParameters>
        </asp:ObjectDataSource>
        <div style="padding-top: 20px; display: none" runat="server" id="divJqgrid">
            <div id="hiddenJqgrid">
                <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
                <ContentTemplate>
                    <cc1:JQGrid ID="JqGridRevenueEntry" AutoWidth="True" runat="server" Height="80%"
                        OnRowEditing="JqGridDataEntry_RowEditing" oninit="JqGridRevenueEntry_Init">
                        <Columns>
                            <cc1:JQGridColumn DataField="RevenueId" Searchable="False" PrimaryKey="True" Width="55"
                                Visible="False" />
                            <cc1:JQGridColumn DataField="HotelRevenueId" Searchable="False" Width="55" Visible="False" />
                            <cc1:JQGridColumn HeaderText="Date" DataField="PositionDate" Editable="true" DataType="DateTime"
                                TextAlign="Center" DataFormatString="{0:dd/MM/yyyy}" FooterValue="Total:">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Day" DataField="Day" Visible="False"
                                TextAlign="Center">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn  Width="200" HeaderText="Occupancy (%)" DataField="OccupancyRoom" 
                                Editable="True" TextAlign="Right">
                                <Formatter>
                                    <cc1:CustomFormatter FormatFunction="formatColor" UnFormatFunction="unformatColor"/>
                                </Formatter>
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateRooms" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn Width="200" HeaderText="Room Revenue" DataField="RoomRevenue" Editable="True"
                                DataFormatString="{0:#,##0.00;(#,##0.00);0}" TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="F&amp;B Revenue" DataField="FBRevenue" Editable="True"
                                DataFormatString="{0:#,##0.00;(#,##0.00);0}" TextAlign="Right">
                                <EditClientSideValidators>
                                    <cc1:RequiredValidator />
                                    <cc1:CustomValidator ValidationFunction="validateCurrency" />
                                </EditClientSideValidators>
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Spa Revenue" DataField="SpaRevenue" Editable="True"
                                DataFormatString="{0:#,##0.00;(#,##0.00);0}" TextAlign="Right">
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
                            <cc1:JQGridColumn HeaderText="Budget" DataField="Budget" Editable="False" DataFormatString="{0:#,##0.00;(#,##0.00);0}"
                                TextAlign="Right">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Update Date-Time Millisec" DataField="UpdateDateTimeMillisecond" Editable="True" Visible="False">
                            </cc1:JQGridColumn>
                            <cc1:JQGridColumn HeaderText="Date Now Millisec" DataField="DateNowMillisecond" Editable="True" Visible="False">
                            </cc1:JQGridColumn>
                        </Columns>
                        <ToolBarSettings ShowRefreshButton="True" ShowEditButton="false" />
                        <EditDialogSettings Modal="True" Width="350" TopOffset="500" LeftOffset="500" CloseAfterEditing="True"
                            Caption="Edit Revenue Entry"></EditDialogSettings>
                        <PagerSettings PageSize="32" />
                        <AppearanceSettings ShowRowNumbers="true" ShowFooter="true" HighlightRowsOnHover="True" />
                        <ClientSideEvents AfterEditDialogShown="disableDate" LoadComplete="onLoadComplete"></ClientSideEvents>
                    </cc1:JQGrid>
                </ContentTemplate>
            </asp:UpdatePanel>
            <div style="padding-top: 20px" id="divExportData" runat="server">
            <table class="TextBlack12" cellpadding="3" cellspacing="3">
                <tr>
                    <td>
                        Export
                    </td>
                    <td>
                        <asp:ImageButton Width="22" Height="22" ID="btnExcel" ImageUrl="Style/images/excel.png"
                            ToolTip="Export to Excel" runat="server" OnClick="btnExcel_Click" />
                    </td>
                    <td>
                        <asp:ImageButton Width="22" Height="22" ID="btnPDF" ImageUrl="Style/images/pdf.png"
                            ToolTip="Export to PDF" runat="server" OnClick="btnPDF_Click" />
                    </td>
                </tr>
            </table>
        </div>
            </div>
        </div>
        
        
    </div>
</asp:Content>
