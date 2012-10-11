﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Site1.Master" AutoEventWireup="true"
    CodeBehind="User.aspx.cs" Inherits="HotelDataEntry.User" %>

<%@ Register TagPrefix="cc1" Namespace="Trirand.Web.UI.WebControls" Assembly="Trirand.Web" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript">
        function populateAccessProperty(value, editOptions,a,b,c) {
            var grid = jQuery("#<%= JqgridUser.ClientID %>");
            var rowKey = grid.getGridParam("selrow");
            var userId = rowKey==null?0:rowKey;
            var table = "";
            $.ajax({
                url: "User.aspx?userid=" + userId,
                type: "GET",
                async: false,
                cache:false,
                success: function (altercompany) {
                    var company = altercompany.split("|");
                    var alterCompanyHtml = "";
                    for (var i = 0; i < company.length; i++) {
                        var str = company[i].split(",");
                        if(i!=0) alterCompanyHtml += "|";
                        if(str[2]=="checked") {
                            alterCompanyHtml += '<span><input type="checkbox" name="chkAccessProperty" checked=checked value="' + str[1] + '"/>' + str[1]+"</span>";
                        }
                        else {
                            alterCompanyHtml += '<span><input type="checkbox" name="chkAccessProperty" value="' + str[1] + '"/>' + str[1]+"</span>";
                        }
                    }
                    table = createDynamicTable(alterCompanyHtml);
                }                
            });
            return table;
        }
        function afterAddDialogShown() {
            var grid = jQuery("#<%= JqgridUser.ClientID %>");
            grid.jqGrid('resetSelection');
        }

        function getAccessPropertyValue(elem) {
            var element = $(elem).find("input:checkbox:checked");
            var checked = "";
            for(var i=0; i<element.length;i++)
            {
                if(i!=0) checked += ",";
                checked += element[i].value;
            }
            return checked;
        }
        
        function createDynamicTable(data) {
            var tbody = $("<table>");
            if (tbody == null || tbody.length < 1) return "";
            if(data.indexOf("|")!= -1) {
                var str = data.split("|");
                var rows = Math.ceil(str.length/5);
                var cols = 5;
                var colsRows = 0;
                for (var r = 1; r <= rows; r++) {
                    var trow = $("<tr>");
                    for (var c = 1; c <= cols; c++) {
                        var cellText = colsRows<=str.length?str[++colsRows]:"";
                        $("<td>")
                            .html(cellText)
                            .appendTo(trow);
                    }
                    trow.appendTo(tbody);
                }    
            }   
            return tbody;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="headerMenuLabel">
        User Management</div>
    <asp:UpdatePanel ID="updatepanel1" UpdateMode="Conditional" runat="server">
        <ContentTemplate>
            <asp:DropDownList ID="ddlProperty" ToolTip="Property is required." DataSourceID="PropertyDataSource"
                DataValueField="PropertyId" DataTextField="PropertyCode" Width="90" runat="server">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="PropertyDataSource" DataObjectTypeName="HotelDataEntryLib.Property"
                SelectMethod="ListProperites" TypeName="HotelDataEntryLib.Page.PropertyHelper"
                runat="server"></asp:ObjectDataSource>
            <asp:DropDownList ID="ddlPermission" ToolTip="Permission is required." DataSourceID="PermissionDataSource"
                DataValueField="PermissionId" DataTextField="PermissionName" Width="90" runat="server">
            </asp:DropDownList>
            <asp:ObjectDataSource ID="PermissionDataSource" DataObjectTypeName="HotelDataEntryLib.Permission"
                SelectMethod="ListPermissions" TypeName="HotelDataEntryLib.Page.PermissionHelper"
                runat="server"></asp:ObjectDataSource>
            <cc1:JQGrid ID="JqgridUser" AutoWidth="True" runat="server" Height="80%" OnRowAdding="JqgridUser_RowAdding"
                OnRowDeleting="JqgridUser_RowDeleting" OnRowEditing="JqgridUser_RowEditing">
                <Columns>
                    <cc1:JQGridColumn DataField="UserId" Searchable="False" PrimaryKey="True" Width="55"
                        Visible="False" />
                    <cc1:JQGridColumn HeaderText="Property" DataField="PropertyCode" EditorControlID="ddlProperty"
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
                    <cc1:JQGridColumn HeaderText="Access Properties" DataField="AccessProperties" Editable="true"
                        TextAlign="Center" EditType="Custom" EditTypeCustomCreateElement="populateAccessProperty"
                        EditTypeCustomGetValue="getAccessPropertyValue" />
                    <cc1:JQGridColumn HeaderText="Permission" DataField="PermissionName" Editable="True"
                        EditType="DropDown" EditorControlID="ddlPermission" TextAlign="Center">
                    </cc1:JQGridColumn>
                    <cc1:JQGridColumn HeaderText="Status" DataField="StatusLabel" EditType="DropDown"
                        EditValues="1:Active;0:InActive" Editable="True" TextAlign="Center" />
                </Columns>
                <AddDialogSettings CloseAfterAdding="False" />
                <EditDialogSettings CloseAfterEditing="True" />
                <ToolBarSettings ShowEditButton="True" ShowDeleteButton="true" ShowAddButton="True"
                    ShowRefreshButton="True" ShowSearchButton="True" />
                <AppearanceSettings ShowRowNumbers="true" />
                <PagerSettings PageSize="20" />
                <DeleteDialogSettings LeftOffset="497" TopOffset="241"></DeleteDialogSettings>
                <AddDialogSettings Width="400" Modal="True" TopOffset="250" LeftOffset="500" Height="300"
                    CloseAfterAdding="True" Caption="Add User" ClearAfterAdding="True"></AddDialogSettings>
                <EditDialogSettings Width="400" Modal="True" TopOffset="250" LeftOffset="500" Height="300"
                    CloseAfterEditing="True" ReloadAfterSubmit="True"  Caption="Edit User"></EditDialogSettings>
                <ClientSideEvents AfterEditDialogShown="afterAddDialogShown"></ClientSideEvents>
            </cc1:JQGrid>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
