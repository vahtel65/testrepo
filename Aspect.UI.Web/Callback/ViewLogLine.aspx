<%@ Page Title="" Language="C#" MasterPageFile="~/Callback/CallbackMaster.Master" AutoEventWireup="true" CodeBehind="ViewLogLine.aspx.cs" Inherits="Aspect.UI.Web.Callback.ViewLogLine" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:repeater visible="true" runat="server" id="ProductCard1">
        <HeaderTemplate><table class="type2"></HeaderTemplate>
        <AlternatingItemTemplate>
             <tr class="row2">
                <th>
                    <%# Eval("Name")%>
                </th>
                <td>
                    <%# Eval("NewValue") %>
                </td>
                <td>
                    <%# Eval("OldValue") %>
                </td>
            </tr>
        </AlternatingItemTemplate>
        <ItemTemplate>
            <tr>
                <th>
                    <%# Eval("Name")%>
                </th>
                <td>
                    <%# Eval("NewValue") %>
                </td>
                <td>
                    <%# Eval("OldValue") %>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:repeater>
</asp:Content>