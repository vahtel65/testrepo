<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="UserColumnsOrder.aspx.cs" Inherits="Aspect.UI.Web.Popup.UserColumnsOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div>
    <div style="min-width:300px;width:92%;overflow:auto;float:left;border-right:dotted 1px black;height:100%;height:450px;">
        <asp:ListBox ID="OrderList" runat="server" DataTextField="Text" DataValueField="ID" Height="100%" Width="100%" >
        </asp:ListBox>
    </div>
    <div style="float:right;height:400px;width:7%;">
        <div style="padding-top:200px;">
        <asp:ImageButton ID="Up" runat="server" ImageUrl="~/img/ico_ArrowUp_1.gif" OnClick="Up_Click" /><br /><br />
        <asp:ImageButton ID="Down" runat="server" ImageUrl="~/img/ico_ArrowDn_1.gif" OnClick="Down_Click" />
        </div>
    </div>
</div>
<div style="border-top:solid 1px black;width:100%;float:right">
    <div style="float:right;padding:15px 30px 0px 0px;">
        <asp:LinkButton ID="Save" runat="server" Text="Сохранить" OnClick="Save_Click" />&nbsp;&nbsp;&nbsp;
        <a onclick="self.parent.tb_remove();return false;" href="javascript:void(0);">Закрыть</a>
    </div>
</div>
</asp:Content>
