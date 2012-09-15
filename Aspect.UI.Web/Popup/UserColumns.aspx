<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="UserColumns.aspx.cs" Inherits="Aspect.UI.Web.Popup.UserColumns" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="min-width:300px;width:49%;overflow:auto;float:left;border-right:dotted 1px black;height:100%;height:450px;">
    <asp:TreeView ID="DictionaryTreeView" ShowCheckBoxes="Leaf" runat="server" ExpandDepth="1" ShowLines="true" >
        <NodeStyle ForeColor="Black" Font-Underline="false" />
    </asp:TreeView>
</div>    
<div style="min-width:300px;width:50%;overflow:auto;float:right;height:100%;height:450px;">
    <asp:Repeater ID="GeneralColumnRepeater" runat="server">
        <HeaderTemplate><table></HeaderTemplate>
        <ItemTemplate>
            <tr><td style="padding:2px 0px 2px 0px;">
            <asp:HiddenField ID="HiddenID" runat="server" Value='<%# Eval("ID") %>' />
            <asp:CheckBox ID="Seleteced" runat="server" Checked='<%# Eval("Selected") %>' />
            <asp:Label ID="Title" runat="server" Text='<%# Eval("Name") %>' /><br />
            </td></tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:Repeater>
</div>
<div style="border-top:solid 1px black;width:100%;float:right">
    <div style="float:right;padding:15px 30px 0px 0px;">
        <asp:LinkButton ID="Save" runat="server" Text="Сохранить" OnClick="Save_Click" />&nbsp;&nbsp;&nbsp;
        <a onclick="self.parent.tb_remove();return false;" href="javascript:void(0);">Закрыть</a>
    </div>
</div>
</asp:Content>
