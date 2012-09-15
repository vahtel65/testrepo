<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true" CodeBehind="View.aspx.cs" Inherits="Aspect.UI.Web.Editarea.View" %>
<%@ Register Assembly="Aspect.UI.Web.Controls" Namespace="Aspect.UI.Web.Controls" TagPrefix="ctrl" %>
<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">

    <script type="text/javascript">
        function setSelectedValue(valID, val, textID, text) {
            document.getElementById(valID).value = val;
            document.getElementById(textID).innerHTML = text;
        }
    </script>

    <script type="text/javascript">
        $().ready(function() {
            // Vertical splitter. Set min/max/starting sizes for the left pane.
            $("#MySplitter").splitter({
                splitHorizontal: true,
                outline: true,
                sizeLeft: true,
                anchorToWindow: true,
                cookie: 'spliterCookieHorizontalEdit',
                accessKey: "H"
            });
        });
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
<div style="float: right; padding: 10px;">
        <asp:Label ID="HeaderLiteral" Font-Bold="false" Font-Size="Large" runat="server"
            Text="Редактирование {0} Версия {1}"></asp:Label>
        &nbsp;&nbsp;
        <asp:Label ID="HeaderDateLiteral" runat="server" Text="(на {0} {1})"></asp:Label>
    </div>
    <div style="clear: both; height: 1px;">
    </div>
    <div id="MySplitter">
        <div id="TopPane">
            <div style="float: left; padding-right: 20px; padding-left: 20px;">
                <asp:Repeater ID="GeneralPropertyRepeater" runat="server">
                    <HeaderTemplate>
                        <table>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td style="padding: 2px 10px 2px 0px;">
                                <asp:HiddenField ID="HiddenID" runat="server" Value='<%# Eval("First.ID") %>' />
                                <asp:Label ID="Title" runat="server" Text='<%# Eval("First.Name") %>' />:
                            </td>
                            <td style="padding: 2px 0px 2px 0px;">
                                <asp:TextBox ID="PropertyValue" Enabled="false" runat="server" MaxLength="300" Text='<%# Eval("Second.Value") %>' Visible="false" />
                                <ctrl:EditControl ID="PropertyValueAdv" CssClass="WideInput" Enabled="false" runat="server" ControlType='<%# Eval("First.Type.Value") %>' Value='<%# Eval("Second.Value") %>'  />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table></FooterTemplate>
                </asp:Repeater>
            </div>
            <div style="float: left;">
                <asp:Repeater ID="DictionaryPropertyRepeater" runat="server">
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
                                <asp:Label ID="PropertyValueSelector" runat="server" Text='<%# Eval("ValueText") %>' />
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table>
                    </FooterTemplate>
                </asp:Repeater>
            </div>
        </div>
        <div id="BottomPane">
             <div style="float: right; padding: 30px;">
                <asp:LinkButton ID="SaveButton" runat="server" Enabled="false" Text="Сохранить" />
            </div>
        </div>
    </div>
</asp:Content>
