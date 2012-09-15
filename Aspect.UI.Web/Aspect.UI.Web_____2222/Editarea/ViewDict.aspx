<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="ViewDict.aspx.cs" Inherits="Aspect.UI.Web.Editarea.ViewDict" %>
<%@ Register Assembly="Aspect.UI.Web.Controls" Namespace="Aspect.UI.Web.Controls" TagPrefix="ctrl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
   <div style="float: right; padding: 10px;">
        <asp:Label ID="HeaderLiteral" Font-Bold="false" Font-Size="Large" runat="server"
            Text="Редактирование {0}"></asp:Label>
        &nbsp;&nbsp;
        <asp:Label ID="HeaderDateLiteral" runat="server" Text="(на {0} {1})"></asp:Label>
    </div>
    <div style="clear: both; height: 1px;">
    </div>
    <div id="MySplitter">
        <div id="TopPane">
            <div style="float:left;padding-right:20px;">
                <asp:ValidationSummary ID="ValidationSummary1" ValidationGroup="EditValidationGroup" DisplayMode="List" EnableClientScript="true" runat="server" Font-Names="Trebuchet MS" HeaderText="Значение следующих полей не сохранено." />
            </div>
            <div style="float: left;padding-right:20px;padding-left:20px;">
                
                <asp:Repeater ID="GeneralPropertyRepeater" runat="server" OnItemDataBound="GeneralPropertyRepeater_ItemDataBound">
                    <HeaderTemplate>
                        <table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="padding: 2px 10px 2px 0px;">
                                <asp:HiddenField ID="HiddenID" runat="server" Value='<%# Eval("First.ColumnName") %>' />
                                <asp:Label ID="Title" runat="server" Text='<%# Eval("First.Name") %>' />:
                            </td>
                            <td style="padding: 2px 0px 2px 0px;">
                                <ctrl:EditControl ID="PropertyValueAdv" CssClass="WideInput" runat="server" Enabled="false" ControlType='<%# Eval("First.Type.Value") %>' Value='<%# Eval("Second") %>'  />
                                <asp:TextBox ID="PropertyValue" runat="server" Enabled="false" Text='<%# Eval("Second") %>' Visible='false' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table></FooterTemplate>
                </asp:Repeater>
            </div>
            <div style="float: left;">
                <asp:Repeater ID="DictionaryPropertyRepeater" runat="server" >
                    <HeaderTemplate>
                        <table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="padding: 2px 10px 2px 0px;" valign="top">
                                <asp:HiddenField ID="HiddenTreeID" runat="server" Value='<%# Eval("DictionaryTreeID") %>' />
                                <asp:Label ID="Title" runat="server" Text='<%# Eval("Title") %>' />:
                            </td>
                            <td style="padding: 2px 0px 2px 0px;">
                                <asp:HiddenField ID="HiddenID" runat="server" Value='<%# Eval("ID") %>' />
                                <asp:Label ID="PropertyValueSelector" runat="server" Text='<%# Eval("ValueText") %>'
                                    NavigateUrl="#" />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
    </div>
</asp:Content>
