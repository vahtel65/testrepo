<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditDict.aspx.cs" Inherits="Aspect.UI.Web.Editarea.EditDict" MasterPageFile="~/SiteMaster.Master" %>
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
                cookie: 'spliterCookieHorizontalEditDict',
                accessKey: "H"
            });
        });
        function cbSelectClass(CID, Title) {
            $("#ctl00_mainPlace_userCID").val(CID);
            $("#ctl00_mainPlace_SelectorButton").text("Класс (" + Title + ")");
        }
    </script>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="mainPlace" runat="server">
    <asp:HiddenField runat="server" ID="userCID" Value="55c7b455-0638-4acb-ac2e-5b4992e48462" />
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
            <div >
                <asp:ValidationSummary ID="ValidationSummary1" DisplayMode="List" EnableClientScript="true" runat="server" Font-Names="Trebuchet MS" HeaderText="Валидация данных." />
                <asp:CustomValidator ID="UniqueValueValidator" runat="server" ErrorMessage='Значение Общ. наименование должно быть уникальным' ValidationGroup="EditValidationGroup" ControlToValidate="stumbTextBox">!</asp:CustomValidator>
                <asp:TextBox ID="stumbTextBox" runat="server" style="visibility:hidden;"></asp:TextBox>
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
                                <asp:TextBox ID="PropertyValue" runat="server" Text='<%# Eval("Second") %>' Visible="false" />
                                <ctrl:EditControl ID="PropertyValueAdv" CssClass="WideInput" runat="server" ControlType='<%# Eval("First.Type.Value") %>' Value='<%# Eval("Second") %>' Title='<%# Eval("First.Name") %>' />
                                <asp:CustomValidator ID="PropertyValidator" runat="server" ErrorMessage='<%# String.Format("Неверный формат поля: {0}", Eval("First.Name")) %>' ControlToValidate="PropertyValue" ValidationGroup="EditValidationGroup">!</asp:CustomValidator>
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
            <div style="float: left; padding: 15px">
                <asp:CheckBox ID="CopyMainVersion" Visible="false" runat="server" Text="Копировать состав из основной версии" />
            </div>
            <div style="float: right; padding: 15px 15px 15px 0;">
                <asp:HyperLink ID="SelectorButton" runat="server" Text="Класс (МатРесурсы)"  NavigateUrl="javascript:tb_show('Выбор класса для продукта','/Popup/SelectTreeClass.aspx?TB_iframe=true&width=400&height=500&callback=simple');" />
            </div>
            <div style="float: right; padding: 15px 15px 15px 0;">
                <asp:LinkButton ID="ClearButton" runat="server" Text="Очистить" OnClick="ClearButton_Click" />
            </div>
            <div style="float: right; padding: 15px 15px 15px 0;">
                <asp:LinkButton ID="SaveButton" runat="server" Text="Сохранить" OnClick="SaveButton_Click" />
            </div>
        </div>
    </div>
</asp:Content>
