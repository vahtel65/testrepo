<%@ Page Title="" Language="C#" MasterPageFile="~/Configuration/ConfigurationMaster.Master"
    AutoEventWireup="true" CodeBehind="Usage.aspx.cs" Inherits="Aspect.UI.Web.Configuration.Usage" %>

<asp:Content ContentPlaceHolderID="headPlace" runat="server" ID="Content2">

    <script type="text/javascript">
        $().ready(function() {

	        // Main vertical splitter, anchored to the browser window
	        $("#MySplitter").splitter({
		        type: "v",
		        outline: true,
		        minLeft: 100, sizeLeft: 550, maxLeft: 1000,
		        anchorToWindow: true,
		        cookie: 'spliterCookieVerticalConfigUsage1',
		        accessKey: "L"
	        });
	        // Second vertical splitter, nested in the right pane of the main one.
	        $("#CenterAndRight").splitter({
		        type: "v",
		        outline: true,
		        minRight: 100, sizeRight: 550, maxRight: 1000,
		        cookie: 'spliterCookieVerticalConfigUsage2',
		        accessKey: "R"
	        });
        });
    </script>

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="MySplitter">
        <div class="SplitterPane">
            <table class="type1" style="margin-top: 15px; margin-bottom: 40px;">
                <tr><td colspan="9" style="border: none;"><h2 style="text-align: left;padding-bottom:5px;">Применяемость</h2></td></tr>
                <tr>
                    <th>
                        Идентификатор
                    </th>
                    <th>
                        Раздел специф
                    </th>
                    <th>
                        Версия
                    </th>
                    <th>
                        Основная версия
                    </th>
                    <th>
                        Номер приказа
                    </th>
                    <th>
                        Год приказа
                    </th>
                    <th>
                        Кол-во
                    </th>
                    <th>
                        Ед. изм.
                    </th>
                    <th>
                        Кол-во узлов
                    </th>
                </tr>
                <asp:Repeater ID="ProductUsage" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:HyperLink ID="ActionLink" runat="server" NavigateUrl='<%# String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", Eval("Product.ID"), Guid.Empty ) %>'
                                    CssClass="thickbox" Text='<%# Eval("Product.PublicName")%>' />
                            </td>
                            <td>
                                <%# ((Guid?)Eval("Product._dictNomen._dictHSID")).HasValue ? Eval("Product._dictNomen._dictH.hsn1") : string.Empty%>
                            </td>
                            <td>
                                <%# Eval("Product.Version")%>
                            </td>
                            <td>
                                <asp:CheckBox ID="MainVersion" runat='server' Enabled="false" Checked='<%# (Eval("Product.MainVersion") == null || Convert.ToInt32(Eval("Product.MainVersion")) == 0) ? false : true %>' />
                            </td>
                            <td>
                                <%# Eval("Product.OrderNumber") %>
                            </td>
                            <td>
                                <%# Eval("Product.OrderYear")%>
                            </td>
                            <td>
                                <%# Eval("Configuration.Quantity")%>
                            </td>
                            <td>
                                <%# Eval("Configuration._dictUM.umn1")%>
                            </td>
                            <td>
                                <%# Eval("Configuration.QuantityInclusive")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
                <tr><td colspan="9">&nbsp;</td></tr>
                <tr class="table-header">
                    <th colspan="9" style="text-align:left;">
                        В допускаемых заменах
                    </th>
                </tr>
                <tr>
                    <th>
                        Идентификатор
                    </th>
                    <th>
                        Раздел специф
                    </th>
                    <th>
                        Версия
                    </th>
                    <th>
                        Основная версия
                    </th>
                    <th>
                        Номер приказа
                    </th>
                    <th>
                        Год приказа
                    </th>
                    <th>
                        Кол-во
                    </th>
                    <th>
                        Ед. изм.
                    </th>
                    <th>
                        Кол-во узлов
                    </th>
                </tr>
                <asp:Repeater ID="ProductUsageChange" runat="server">
                    <ItemTemplate>
                        <tr>
                            <td>
                                <asp:HyperLink ID="ActionLink" runat="server" NavigateUrl='<%# String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", Eval("Product.ID"), Guid.Empty ) %>'
                                    CssClass="thickbox" Text='<%# Eval("Product.PublicName")%>' />
                            </td>
                            <td>
                                <%# ((Guid?)Eval("Product._dictNomen._dictHSID")).HasValue ? Eval("Product._dictNomen._dictH.hsn1") : string.Empty%>
                            </td>
                            <td>
                                <%# Eval("Product.Version")%>
                            </td>
                            <td>
                                <asp:CheckBox ID="MainVersion" runat='server' Enabled="false" Checked='<%# (Eval("Product.MainVersion") == null || Convert.ToInt32(Eval("Product.MainVersion")) == 0) ? false : true %>' />
                            </td>
                            <td>
                                <%# Eval("Product.OrderNumber") %>
                            </td>
                            <td>
                                <%# Eval("Product.OrderYear")%>
                            </td>
                            <td>
                                <%# Eval("Configuration.Quantity")%>
                            </td>
                            <td>
                                <%# Eval("Configuration._dictUM.umn1")%>
                            </td>
                            <td>
                                <%# Eval("Configuration.QuantityInclusive")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>
        </div>
        <div id="CenterAndRight">

            <div class="SplitterPane">
                <h2 style="text-align:left;padding-left:5px;">Объект</h2>
                <br />
                <h2 style="text-align:left;white-space:nowrap;padding-left:5px;"><asp:Literal ID="ProductName5" runat="server"></asp:Literal></h2>
                <br />
                <asp:Label ID="HeaderDateLiteral" runat="server" Text="(на {0} {1})"></asp:Label>
                <br />
                <table class="type1">
                    <tr>
                        <th>
                        </th>
                        <th>
                            Даta
                        </th>
                        <th>
                            Версия
                        </th>
                        <th>
                            Основная версия
                        </th>
                        <th>
                            Номер приказа
                        </th>
                        <th>
                            Год приказа
                        </th>
                    </tr>
                    <asp:Repeater ID="ProductVersions" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td>
                                    <asp:HiddenField ID="HiddentID" runat="server" Value='<%# Eval("ID") %>' />
                                    <asp:RadioButton ID="RDButton" runat="server" OnCheckedChanged="SelectProduct_Click"
                                        AutoPostBack="true" Checked='<%# this.CurrentProductID.Equals(new Guid(Eval("ID").ToString())) %>' />
                                </td>
                                <td>
                                    <asp:HyperLink ID="LinkButton1" CssClass="thickbox" runat="server" Text='<%# Eval("CreatedDate") %>'
                                        NavigateUrl='<%# String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", Eval("ID"), Guid.Empty ) %>' />
                                </td>
                                <td>
                                    <asp:HyperLink ID="LinkButton2" CssClass="thickbox" runat="server" Text='<%# Eval("Version") %>'
                                        NavigateUrl='<%# String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", Eval("ID"), Guid.Empty ) %>' />
                                </td>
                                <td>
                                    <asp:CheckBox ID="MainVersion" runat='server' Enabled="false" Checked='<%# (Eval("MainVersion") == null || Convert.ToInt32(Eval("MainVersion")) == 0) ? false : true %>' />
                                </td>
                                <td>
                                    <asp:HyperLink ID="HyperLink2" CssClass="thickbox" runat="server" Text='<%# Eval("OrderNumber") %>'
                                        NavigateUrl='<%# String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", Eval("ID"), Guid.Empty ) %>' />
                                </td>
                                <td>
                                    <asp:HyperLink ID="HyperLink3" CssClass="thickbox" runat="server" Text='<%# Eval("OrderYear") %>'
                                        NavigateUrl='<%# String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", Eval("ID"), Guid.Empty ) %>' />
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </div>
            
            <div class="SplitterPane">
                    <table class="type1" style="margin-top: 15px; margin-bottom: 40px;">
                        <tr><td colspan="11" style="border: none;"><h2 style="text-align: left;padding-bottom:5px;">Спецификация</h2></td></tr>
                        <tr>
                            <th>
                                Идентификатор
                            </th>
                            <th>
                                Раздел специф
                            </th>
                            <th>
                                Версия
                            </th>
                            <th>
                                Основная версия
                            </th>
                            <th>
                                Номер приказа
                            </th>
                            <th>
                                Год приказа
                            </th>
                            <th>
                                Зона
                            </th>
                            <th>
                                Поз
                            </th>
                            <th>
                                Примеч
                            </th>
                            <th>
                                Кол-во
                            </th>
                            <th colspan="2">
                                Ед. изм.
                            </th>
                            <th>
                                Группа замены
                            </th>
                            <th>
                                Кол-во узлов
                            </th>
                        </tr>
                        <asp:Repeater ID="ProductSpec" runat="server">
                            <ItemTemplate>
                                <tr style='font-weight: <%# (Eval("Configuration.GroupToChange") != null && Convert.ToBoolean(Eval("Configuration.GroupToChange"))) ? "bold" : "notmal" %>;'>
                                    <td>
                                        <asp:HyperLink ID="ActionLink" runat="server" NavigateUrl='<%# String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", Eval("Product.ID"), Guid.Empty ) %>'
                                            CssClass="thickbox" Text='<%# Eval("Product.PublicName")%>' />
                                    </td>
                                    <td>
                                        <%# ((Guid?)Eval("Product._dictNomen._dictHSID")).HasValue ? Eval("Product._dictNomen._dictH.hsn1") : string.Empty%>
                                    </td>
                                    <td>
                                        <%# Eval("Product.Version")%>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="MainVersion" runat='server' Enabled="false" Checked='<%# (Eval("Product.MainVersion") == null || Convert.ToInt32(Eval("Product.MainVersion")) == 0) ? false : true %>' />
                                    </td>
                                    <td>
                                        <%# Eval("Product.OrderNumber") %>
                                    </td>
                                    <td>
                                        <%# Eval("Product.OrderYear")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.Zone")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.Position")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.Comment")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.Quantity")%>
                                    </td>
                                    <td colspan="2">
                                        <%# Eval("Configuration._dictUM.umn1")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.GroupToChange")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.QuantityInclusive")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr><td colspan="14">&nbsp;</td></tr>
                        <tr class="table-header">
                            <th colspan="14" style="text-align:left;">
                                Допускаемые замены
                            </th>
                        </tr>
                        <tr>
                            <th>
                                Идентификатор
                            </th>
                            <th>
                                Раздел специф
                            </th>
                            <th>
                                Версия
                            </th>
                            <th>
                                Основная версия
                            </th>
                            <th>
                                Номер приказа
                            </th>
                            <th>
                                Год приказа
                            </th>
                            <th colspan="3">&nbsp;
                            </th>
                            <th>
                                Кол-во
                            </th>
                            <th>
                                Ед. изм.
                            </th>
                            <th>
                                Вариант замены
                            </th>
                            <th>
                                Группа замены
                            </th>
                            <th>
                                Кол-во узлов
                            </th>
                        </tr>
                        <asp:Repeater ID="ProductSpecChange" runat="server">
                            <ItemTemplate>
                                <tr style='font-weight: <%# (Eval("Configuration.GroupToChange") != null && Convert.ToBoolean(Eval("Configuration.GroupToChange"))) ? "bold" : "notmal" %>;'>
                                    <td>
                                        <asp:HyperLink ID="ActionLink" runat="server" NavigateUrl='<%# String.Format("~/Popup/ProductActions.aspx?height=500&width=400&ID={0}&CID={1}", Eval("Product.ID"), Guid.Empty ) %>'
                                            CssClass="thickbox" Text='<%# Eval("Product.PublicName")%>' />
                                    </td>
                                    <td>
                                        <%# ((Guid?)Eval("Product._dictNomen._dictHSID")).HasValue ? Eval("Product._dictNomen._dictH.hsn1") : string.Empty%>
                                    </td>
                                    <td>
                                        <%# Eval("Product.Version")%>
                                    </td>
                                    <td>
                                        <asp:CheckBox ID="MainVersion" runat='server' Enabled="false" Checked='<%# (Eval("Product.MainVersion") == null || Convert.ToInt32(Eval("Product.MainVersion")) == 0) ? false : true %>' />
                                    </td>
                                    <td>
                                        <%# Eval("Product.OrderNumber") %>
                                    </td>
                                    <td>
                                        <%# Eval("Product.OrderYear")%>
                                    </td>
                                    <td colspan="3">&nbsp;</td>
                                    <td>
                                        <%# Eval("Configuration.Quantity")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration._dictUM.umn1")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.GroupNumber")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.GroupToChange")%>
                                    </td>
                                    <td>
                                        <%# Eval("Configuration.QuantityInclusive")%>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
            </div>
        </div>
    </div>
</asp:Content>
