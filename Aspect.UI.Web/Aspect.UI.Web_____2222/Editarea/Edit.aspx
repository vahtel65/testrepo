<%@ Page Title="" Language="C#" MasterPageFile="~/SiteMaster.Master" AutoEventWireup="true"
    CodeBehind="Edit.aspx.cs" Inherits="Aspect.UI.Web.Editarea.Edit" %>
    <%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
    <%@ Register Assembly="Aspect.UI.Web.Controls" Namespace="Aspect.UI.Web.Controls" TagPrefix="ctrl" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headPlace" runat="server">

    <script type="text/javascript">
        function setSelectedValue(valID, val, textID, text) {
            document.getElementById(valID).value = val;
            document.getElementById(textID).innerHTML = text;
            if ($("#" + textID).hasClass("selectorForNomen")) {
                $.ajax({
                    type: "POST",
                    url: "/Callback/MaxVersionForNomen.aspx?nid=" + val,
                    data: {},
                    contentType: "application/html; charset=utf-8",
                    success: function(msg) {
                        $("input[value='0789db1a-9baa-4574-b405-ae570c746c03']").parent().next().find("input").attr("value", msg);
                        $(".valueForWeight").find("input").attr("value", "");
                    }
                });
            }
        }
        function updateEditLocker(delay) {
            /*$.ajax({
                type: "POST",
                url: "/Callback/UpdateLocker.aspx?userid=<%=this.User.ID %>&targetid=<%=this.RequestProductID %>",
                data: {},
                contentType: "application/html; charset=utf-8",
                success: function(msg) {
                }
            });
            setTimeout('updateEditLocker('+delay+')', delay);*/
        }
        $(window).bind('unload', function() {
            /*$.ajax({
            type: "POST",
            url: "/Callback/DeleteLocker.aspx?userid=<%=this.User.ID %>&targetid=<%=this.RequestProductID %>",
            data: {},
            async: false,
            contentType: "application/html; charset=utf-8",
            success: function(msg) {
            }
            });*/
        });
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
            // timer to update locker
            // setTimeout('updateEditLocker(50000)', 50000);            
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
        <div id="TopPane" style="height:600px;">
            <div>
                <asp:ValidationSummary ID="ValidationSummary1" DisplayMode="List" EnableClientScript="true" runat="server" Font-Names="Trebuchet MS" HeaderText="Валидация данных." />
            </div>
            <div style="float: left; padding-right: 20px; padding-left: 20px;">
                <asp:Repeater ID="GeneralPropertyRepeater" runat="server" OnItemDataBound="GeneralPropertyRepeater_ItemDataBound">
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
                                <asp:TextBox ID="PropertyValue" Visible="false" runat="server" MaxLength="300" Text='<%# Eval("Second.Value") %>' />
                                <ctrl:EditControl ID="PropertyValueAdv" CssClass="WideInput" runat="server" ControlType='<%# Eval("First.Type.Value") %>' Value='<%# Eval("Second.Value") %>' Title='<%# Eval("First.Name") %>' />
                                <asp:CustomValidator ID="UniqueValueValidator" runat="server" ErrorMessage='<%# String.Format("Значение должно быть уникальным: {0}", Eval("First.Name")) %>' ControlToValidate="PropertyValue" ValidationGroup="EditValidationGroup">!</asp:CustomValidator>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate>
                        </table></FooterTemplate>
                </asp:Repeater>
            </div>
            <div style="float: left;">
                <asp:Repeater ID="DictionaryPropertyRepeater" runat="server" OnItemDataBound="DictionaryPropertyRepeater_ItemDataBound">
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
                                <asp:HyperLink ID="PropertyValueSelector" runat="server" Text='<%# Eval("ValueText") %>'
                                    NavigateUrl="#" />
                                 <asp:CustomValidator ID="UniqueValueValidator" runat="server" ErrorMessage='<%# String.Format("Значение должно быть уникальным: {0}", Eval("Title")) %>' ValidationGroup="EditValidationGroup">!</asp:CustomValidator>
                            </td>
                            <td style="padding: 2px 0px 2px 10px;">
                                <asp:HyperLink ID="DeleteLink" runat="server" Text="Удалить" NavigateUrl="javascript:void(0)" />
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
                <asp:LinkButton ID="ClearButton" runat="server" Text="Очистить" OnClick="ClearButton_Click" />
            </div>
            <div style="float: right; padding: 30px;">
                <asp:LinkButton ID="SaveButton" runat="server" Text="Сохранить" OnClick="SaveButton_Click" />
            </div>
        </div>
    </div>
</asp:Content>
