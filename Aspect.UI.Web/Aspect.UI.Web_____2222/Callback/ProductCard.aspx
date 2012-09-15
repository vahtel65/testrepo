<%@ Page Title="" Language="C#" MasterPageFile="~/Callback/CallbackMaster.Master"
    AutoEventWireup="true" CodeBehind="ProductCard.aspx.cs" Inherits="Aspect.UI.Web.Callback.ProductCard" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <table>
    <tr>
        <td>
    <asp:repeater visible="true" runat="server" id="ProductCard1">
        <HeaderTemplate><table class="type2"></HeaderTemplate>
        <AlternatingItemTemplate>
             <tr class="row2">
                <th>
                    <%# Eval("First")%>
                </th>
                <td>
                    <%# Eval("Second") %>
                </td>
            </tr>
        </AlternatingItemTemplate>
        <ItemTemplate>
            <tr>
                <th>
                    <%# Eval("First")%>
                </th>
                <td>
                    <%# Eval("Second") %>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:repeater>
            </td>
        <td>
    <asp:repeater visible="true" runat="server" id="ProductCard2">
        <HeaderTemplate><table class="type2"></HeaderTemplate>
        <AlternatingItemTemplate>
             <tr class="row2">
                <th>
                    <%# Eval("First")%>
                </th>
                <td>
                    <%# Eval("Second") %>
                </td>
            </tr>
        </AlternatingItemTemplate>
        <ItemTemplate>
            <tr>
                <th>
                    <%# Eval("First")%>
                </th>
                <td>
                    <%# Eval("Second") %>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:repeater>
            </td>
    </tr>
    </table>
</asp:Content>
