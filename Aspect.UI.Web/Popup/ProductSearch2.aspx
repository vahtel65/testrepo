<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="ProductSearch2.aspx.cs" Inherits="Aspect.UI.Web.Popup.ProductSearch2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="min-width:300px;width:99%;overflow:auto;float:left;height:100%;height:450px;">
<asp:Repeater ID="SearchRepeater" runat="server">
    <HeaderTemplate>
        <table>
    </HeaderTemplate>
    <ItemTemplate>
        <tr>
            <td style="font-size:larger;width:30%;padding:10px;white-space:nowrap;"><asp:HiddenField ID="ColumnID" runat="server" Value='<%# Eval("FieldID") %>' /><%# Eval("FieldName") %>:</td>
            <td style="width:70%;"><asp:TextBox ID="SearchExpression" Width="100%" runat="server" Text='<%# Eval("FieldValue") %>' /></td>
        </tr>
    </ItemTemplate>
    <FooterTemplate>
        </table>
    </FooterTemplate>
</asp:Repeater>
</div>
<div style="border-top:solid 1px black;width:100%;float:right">
    <div style="float:right;padding:15px 30px 0px 0px;">
        <asp:LinkButton ID="Search" runat="server" Text="Поиск" OnClick="Search_Click" />&nbsp;&nbsp;&nbsp;
        <a onclick="self.parent.tb_remove();return false;" href="javascript:void(0);">Закрыть</a>
    </div>
</div>
</asp:Content>
