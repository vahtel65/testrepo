<%@ Page Title="" Language="C#" MasterPageFile="~/Configuration/ConfigurationMaster.Master" AutoEventWireup="true" CodeBehind="Compare.aspx.cs" Inherits="Aspect.UI.Web.Configuration.Compare" %>
<asp:Content ID="Content2" ContentPlaceHolderID="headPlace" runat="server">
    <script type="text/javascript">
        function scrollTd2Wrap() {
            $("#td2_wrap").unbind("scroll");
            $("#td2_wrap").attr({ scrollTop: $("#td1_wrap").attr("scrollTop") });
            $("#td2_wrap").attr({ scrollLeft: $("#td1_wrap").attr("scrollLeft") });
            $("#td2_wrap").bind("scroll", scrollTd1Wrap);
        }

        function scrollTd1Wrap() {
            $("#td1_wrap").unbind("scroll");
            $("#td1_wrap").attr({ scrollTop: $("#td2_wrap").attr("scrollTop") });
            $("#td1_wrap").attr({ scrollLeft: $("#td2_wrap").attr("scrollLeft") });
            $("#td1_wrap").bind("scroll", scrollTd2Wrap);
        }
        
        $().ready(function() {
            // Vertical splitter. Set min/max/starting sizes for the left pane.
            $("#PanelSplitter").bind("resize", function() {
                $("#td1_wrap").css("height", $("#TopPane").css("height"));
                $("#td2_wrap").css("height", $("#TopPane").css("height"));
                $("#ProductCardLeft").css("height", $("#BottomPane").css("height"));
                $("#ProductCardRight").css("height", $("#BottomPane").css("height"));
            });
            $("#PanelSplitter").splitter({
                type: 'h',
                outline: true,
                anchorToWindow: true,
                cookie: 'spliterCookiePanel',
                accessKey: "H"
            });
            $("#td1_wrap").bind("scroll", scrollTd2Wrap);
            $("#td2_wrap").bind("scroll", scrollTd1Wrap);
        });
    </script>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div style="width: 100%; padding: 10px;">
        <table width="100%">
        <tr><td width="50%">
            <asp:Label ID="HeaderLiteralLeft" Font-Bold="false" Font-Size="Large" runat="server"
                Text="Спецификация {0} Версия {1}"></asp:Label>
            &nbsp;&nbsp;
        </td><td width="50%">
            <asp:Label ID="HeaderLiteralRight" Font-Bold="false" Font-Size="Large" runat="server"
                Text="Спецификация {0} Версия {1}"></asp:Label>
            &nbsp;&nbsp;            
        </td></tr>        
        </table>        
    </div>
    <div style="clear: both; height: 1px;"></div>
    <table class="type1" style="width: 100%">
            <tr class="table-header">
                <th>Сравнение спецификаций</th>
            </tr>
    </table>
    <div id="PanelSplitter">
        <div id="TopPane">
            <table width="100%" border="1px" style="height:100%; table-layout: fixed;">
            <tr style="font-family: Courier New" ><td width="50%" >
            <div id="td1_wrap" style="overflow: scroll;">
                <asp:GridView ID="LeftGridView" runat="server" GridLines="Vertical" BorderColor="LightGray"
                AutoGenerateColumns="false" AllowSorting="false" AllowPaging="false"
                CssClass="type1" BorderStyle="None" 
                OnRowCreated="LeftGridView_RowCreated" OnRowDataBound="LeftGridView_RowDateBound">
                <AlternatingRowStyle CssClass="row2" />
                <Columns>
                    <asp:BoundField DataField="hideConfigurationID" Visible="false" ReadOnly="True" />
                    <asp:BoundField DataField="hideProductID" Visible="false" ReadOnly="True" />
                    <asp:BoundField DataField="hideType" Visible="false" ReadOnly="True" />                    
                    <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink ImageUrl='<%# Eval("ImageCompare") %>' runat="server"
                            NavigateUrl='<%# String.Format("~/Configuration/Compare.aspx?PID1={0}&PID2={1}", Eval("hideProductID"), Eval("hideAlternateID")) %>' />
                    </ItemTemplate>        
                    </asp:TemplateField>                    
                    <asp:BoundField DataField="Name" HeaderText="Идентификатор" ReadOnly="True" />
                    <asp:BoundField DataField="Version" HeaderText="Версия" ReadOnly="True" />
                    <asp:BoundField DataField="Count" HeaderText="Кол-во" ReadOnly="True" />                    
                    <asp:BoundField DataField="Comment" HeaderText="Примечание" ReadOnly="True" />
                    <asp:BoundField DataField="Position" HeaderText="Позиция" ReadOnly="True" />
                    <asp:BoundField DataField="Zone" HeaderText="Зона" ReadOnly="True" />
                    <asp:BoundField DataField="Ngroup" HeaderText="Вариант замены" ReadOnly="True" />
                    <asp:BoundField DataField="Ngrchange" HeaderText="Группа замены" ReadOnly="True" />                    
                </Columns>
                </asp:GridView>
            </div>
            </td><td width="50%">
            <div id="td2_wrap" style="overflow: scroll;">
                <asp:GridView ID="RightGridView" runat="server" GridLines="Vertical" BorderColor="LightGray"
                AutoGenerateColumns="false" AllowSorting="false" AllowPaging="false"
                CssClass="type1" BorderStyle="None" 
                OnRowCreated="RightGridView_RowCreated"  OnRowDataBound="LeftGridView_RowDateBound">
                <AlternatingRowStyle CssClass="row2" />
                <Columns>                    
                    <asp:BoundField DataField="hideConfigurationID" Visible="false" ReadOnly="True" />
                    <asp:BoundField DataField="hideProductID" Visible="false" ReadOnly="True" />
                    <asp:BoundField DataField="hideType" Visible="false" ReadOnly="True" />                                        
                    <asp:TemplateField>
                    <ItemTemplate>
                        <asp:HyperLink Target="_blank" ImageUrl='<%# Eval("ImageCompare") %>' runat="server"
                            NavigateUrl='<%# String.Format("~/Configuration/Compare.aspx?PID1={0}&PID2={1}", Eval("hideProductID"), Eval("hideAlternateID")) %>' />
                    </ItemTemplate>        
                    </asp:TemplateField>                                        
                    <asp:BoundField DataField="Name" HeaderText="Идентификатор" ReadOnly="True" />
                    <asp:BoundField DataField="Version" HeaderText="Версия" ReadOnly="True" />
                    <asp:BoundField DataField="Count" HeaderText="Кол-во" ReadOnly="True" />                    
                    <asp:BoundField DataField="Comment" HeaderText="Примечание" ReadOnly="True" />
                    <asp:BoundField DataField="Position" HeaderText="Позиция" ReadOnly="True" />
                    <asp:BoundField DataField="Zone" HeaderText="Зона" ReadOnly="True" />
                    <asp:BoundField DataField="Ngroup" HeaderText="Вариант замены" ReadOnly="True" />
                    <asp:BoundField DataField="Ngrchange" HeaderText="Группа замены" ReadOnly="True" />                    
                </Columns>
                </asp:GridView>       
            </div>
            </td></tr>
            </table>     
        </div>        
        <div id="BottomPane">
        <table width="100%" border="1px" style="height:100%; table-layout: fixed;">
        <tr>
            <td height="100%" width="50%">
                <div id="ProductCardLeft" style="overflow: scroll"></div>
            </td>
            <td height="100%" width="50%">
                <div id="ProductCardRight" style="overflow: scroll"></div>
            </td>
        </tr>
        </table>
        </div>
    </div>    
</asp:Content>
