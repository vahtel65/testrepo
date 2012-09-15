<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="Selector.aspx.cs" Inherits="Aspect.UI.Web.Popup.Selector" %>
<%@ Register TagPrefix="ctrls" TagName="Pager" Src="~/Controls/Pager.ascx" %>

<asp:Content ID="Content2" ContentPlaceHolderID="headPlace" runat="server">
    <link rel="Stylesheet" type="text/css" href="/css/thickbox.css" />
    <script type="text/javascript" src="/scripts/jquery.js" ></script>
    <script type="text/javascript" src="/scripts/jquery-ext.js" ></script>
    <script type="text/javascript" src="/scripts/aspect.js" ></script>
    <script type="text/javascript">
        function refresh() {
            __doPostBack('ctl00$ContentPlaceHolder1$RefreshButton', '');
        }
        function setSearchExp(exp) {
            var sID = '<%= SearchExp.ClientID %>';
            document.getElementById(sID).value = exp;
            refresh();
        }
        function fnSelectRow(id,text, RowID) {
            var listSelected = $('#ctl00_ContentPlaceHolder1_MultiSelection').val();
            var items = (listSelected == '') ? new Array() : listSelected.split(';');            
            
            var newSelected = true;
            var forRemove = -1;
                        
            for (i = 0; i < items.length; i++) {
                var id_text = items[i].split(':');
                if (id_text[0] == id) {
                    newSelected = false;
                    forRemove = i;
                }
            }            

            if (!newSelected) {
                $('.' + items[forRemove].split(':')[0]).removeClass('RowSelected');
                items.splice(forRemove, 1);                
            } else {
                items.push(id + ':' + $.base64Encode(text));
                $('.' + id).addClass('RowSelected');
            }

            var hiddenValue = '';
            for (i = 0; i < items.length; i++) {
                hiddenValue = hiddenValue + items[i] + ((i == items.length - 1) ? '' : ';');
            }

            $('#ctl00_ContentPlaceHolder1_MultiSelection').val(hiddenValue);
        }
    </script>
    
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="min-width:300px;width:100%;overflow:auto;float:left;border-right:dotted 1px black;height:100%;height:450px;">
    <div id="toolbar">
    <ul>
        <li><a id="ProductSearch" title="поиск" runat="server" class="btn-search" href="#">
        <img src="../img/ico_5_2.gif" />
        </a>
        <asp:LinkButton ID="RefreshButton" runat="server" CssClass="btn-refresh" ToolTip="Обновить" OnClick="RefreshButton_Click">
        <img src="../img/ico_11.gif" /></asp:LinkButton></li>        
    </ul>
    </div>
    
    <asp:HiddenField ID="SearchExp" runat="server" />
    <asp:HiddenField ID="MultiSelection" runat="server" />
    <asp:GridView ID="SelectorGrid" runat="server" GridLines="Vertical" BorderColor="LightGray"
        AutoGenerateColumns="false" PageSize="20" AllowSorting="true" AllowPaging="true" ShowHeader="true"
        CssClass="type1" BorderStyle="None"  OnRowCreated="SelectorGrid_RowCreated" OnSorting="SelectorGrid_Sorting">
        <AlternatingRowStyle CssClass="row2" />
        <HeaderStyle ForeColor="Black" Font-Bold="false" HorizontalAlign="Center" Font-Names="Tahoma" />
        <PagerTemplate>
        </PagerTemplate>
    </asp:GridView>
</div>
<div style="border-top:solid 1px black;width:100%;float:right">
    <div style="float:left;padding: 0px 30px 0px 0px;">
        <ctrls:Pager ID="GridPager" runat="server" PageSize="20" OnCurrentPageChanged="GridPager_CurrentPageChanged" Range="4" />
    </div>
    <div style="float:right;padding:15px 30px 0px 0px;">
        <asp:HyperLink ID="MultiInsert" runat="server" Text="Вставить" NavigateUrl="javascript:void(0);" />
        <a style="margin-left: 10px" onclick="self.parent.tb_remove();return false;" href="javascript:void(0);">Закрыть</a>
    </div>
</div>    
</asp:Content>
