<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="Buffer.aspx.cs" Inherits="Aspect.UI.Web.Popup.Buffer" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="min-width:300px;width:100%;overflow:auto;float:left;border-right:dotted 1px black;height:100%;height:450px;">
     <table class="type1">
        <tr>
            <th colspan="2">
                Буфер
            </th>
        </tr>
        <asp:Repeater ID="BufferRepeater" runat="server">
            <ItemTemplate>
                <tr>
                <td>
                    <asp:Label ID="Name" runat="server" Text='<%# Eval("PublicName") %>' />
                </td>
                <td>
                    <asp:LinkButton ID="Delete" runat="server" OnCommand="Delete" CommandArgument='<%# Eval("ID") %>' Text="удалить" />
                </td>
                </tr>
            </ItemTemplate>
        </asp:Repeater>
    </table>
</div>
<div style="border-top:solid 1px black;width:100%;float:right">
    <div style="float:right;padding:15px 30px 0px 0px;">
        <asp:HyperLink Style="margin-right:10px" ID="CompareButton" runat="server" Visible="false" Text="Сравнить спецификации" NavigateUrl="" onclick="self.parent.tb_remove();" Target="_blank" />
        <asp:LinkButton Style="margin-right:10px" ID="ClearButton" OnCommand="DeleteAll" runat="server" Text="Удалить все" />
        <a onclick="self.parent.tb_remove();return false;" href="javascript:void(0);">Закрыть</a>
    </div>
</div>
</asp:Content>
