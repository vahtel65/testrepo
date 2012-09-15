<%@ Page Title="" Language="C#" MasterPageFile="~/AspectMaster.Master" AutoEventWireup="true" CodeBehind="Content.aspx.cs" Inherits="Aspect.UI.Web.Content" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="width:35%;float:left;border-right:solid 1px black;height:800px;">
    <table class="dev-message">
        <tr>
            <th class="top" colspan="5">
                Применяемость СПЦ123.00.001 (на 28.05.2009 11:23:98)
            </th>
        </tr>
        <tr>
            <th>Обозначение</th>
            <th>Наименование</th>
            <th>Раздел специф</th>
            <th>Количество</th>
            <th>Ед. изм</th>
        </tr>
        <tr>
            <td>СПЦ125.00.001</td>
            <td>Конвейер</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
        </tr>
        <tr>
            <td>СПЦ125.00.001</td>
            <td>Конвейер</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
        </tr>
        <tr>
            <td>СПЦ125.00.001</td>
            <td>Конвейер</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
        </tr>           
        <tr><td colspan="5" style="text-align:right">
            <asp:LinkButton ID="Prev" runat="server" Text="<<Предидущая" Font-Size="Small" ForeColor="Green" Font-Underline="true"></asp:LinkButton>
            <asp:LinkButton ID="LinkButton3" runat="server" Text="1" Font-Size="Small" Font-Bold="true" ForeColor='Black' Font-Underline="false"></asp:LinkButton>
            <asp:LinkButton ID="LinkButton2" runat="server" Text="2" Font-Size="Small" ForeColor="Green" Font-Underline="true"></asp:LinkButton>
            <asp:LinkButton ID="LinkButton1" runat="server" Text="Следующая>>" Font-Size="Small" ForeColor="Green"  Font-Underline="true"></asp:LinkButton>
        </td></tr>
    </table>
    <br />
    <h1>В допускаемых заменах</h1>
    <table class="dev-message">
        <tr>
            <th class="top" colspan="5">
                Применяемость СПЦ123.00.001 (на 28.05.2009 11:23:98)
            </th>
        </tr>
        <tr>
            <th>Обозначение</th>
            <th>Наименование</th>
            <th>Раздел специф</th>
            <th>Количество</th>
            <th>Ед. изм</th>
        </tr>
        <tr>
            <td>СПЦ125.00.001</td>
            <td>Конвейер</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
        </tr>
    </table>
</div>
<div style="width:28%;float:left;border-right:solid 1px black;height:800px;">
<h1>Объекты</h1>
    <table class="dev-message">
        <tr>
            <th colspan="2">Обозначение</th>
            <th>Наименование</th>
            <th>№ заказа</th>
        </tr>
        <tr>
            <td colspan="2">
            <asp:LinkButton ID="ShowFirst" runat="server" Font-Underline="true" OnClick="ShowFirst_Click" Text="СПЦ123.00.000"></asp:LinkButton></td>
            <td>Конвейер</td>
            <td>123</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td >
            <asp:LinkButton ID="LinkButton8" runat="server" Font-Underline="true" OnClick="ShowSecond_Click" Text="СПЦ123.00.001"></asp:LinkButton>
            </td>
            <td>Конвейер</td>
            <td>123.1</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td >СПЦ123.00.002</td>
            <td>Конвейер</td>
            <td>123.2</td>
        </tr>
        <tr>
            <td>&nbsp;</td>
            <td >СПЦ123.00.003</td>
            <td>Конвейер</td>
            <td>123.3</td>
        </tr>
    </table>
</div>
<asp:PlaceHolder ID="FirstHolder" runat="server">
<div style="width:35%;float:left;height:800px;">
<h1>Состав&nbsp;<asp:Literal ID="SelectedObjectName" runat="server"></asp:Literal> </h1>
 <table class="dev-message">
        <tr>
            <th class="top" colspan="8">
                Редактирование
            </th>
        </tr>
        <tr>
            <th>Обозначение</th>
            <th>Наименование</th>
            <th>Раздел специф</th>
            <th>Количество</th>
            <th>Ед. изм</th>
            <th>Вариант замены</th>
            <th>Группа замены</th>
            <th>Скрытый</th>
        </tr>
        <tr>
            <td>СПЦ13.00.001</td>
            <td>Редуктор</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
            <td>1</td>
            <td>2</td>
            <td></td>
        </tr>
        <tr>
            <td>КРУП334.60.00.400</td>
            <td>Стакан</td>
            <td>Сб</td>
            <td>45</td>
            <td>Шт.</td>
            <td></td>
            <td>-</td>
            <td></td>
        </tr>
        <tr>
            <td>СП301.120.01.150</td>
            <td>Направляющая</td>
            <td>Ст</td>
            <td>1232</td>
            <td>Шт.</td>
            <td></td>
            <td></td>
            <td></td>
        </tr>    
        <tr><td colspan="5" style="text-align:right">
            <asp:LinkButton ID="LinkButton4" runat="server" Text="<<Предидущая" Font-Size="Small" ForeColor="Green" Font-Underline="true"></asp:LinkButton>
            <asp:LinkButton ID="LinkButton5" runat="server" Text="1" Font-Size="Small" Font-Bold="true" ForeColor='Black' Font-Underline="false"></asp:LinkButton>
            <asp:LinkButton ID="LinkButton6" runat="server" Text="2" Font-Size="Small" ForeColor="Green" Font-Underline="true"></asp:LinkButton>
            <asp:LinkButton ID="LinkButton7" runat="server" Text="Следующая>>" Font-Size="Small" ForeColor="Green"  Font-Underline="true"></asp:LinkButton>
        </td></tr>
    </table>
    <br />
    <h1>Допускаемые замены</h1>
    <table class="dev-message">
        <tr>
            <th class="top" colspan="8">
                Состав СПЦ123.00.001 (на 28.05.2009 11:23:98)
            </th>
        </tr>
        <tr>
            <th>Обозначение</th>
            <th>Наименование</th>
            <th>Раздел специф</th>
            <th>Количество</th>
            <th>Ед. изм</th>
            <th>Вариант замены</th>
            <th>Группа замены</th>
            <th>Скрытый</th>
        </tr>
        <tr>
            <td>СПЦ13.00.001-01</td>
            <td>Редуктор</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
            <td>2</td>
            <td>3</td>
            <td></td>
        </tr>
        <tr>
            <td>СПЦ13.00.001-02</td>
            <td>Редуктор</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
            <td>3</td>
            <td></td>
            <td></td>
        </tr>
    </table>
</div>
</asp:PlaceHolder>
<asp:PlaceHolder ID="SecondHolder" runat="server" Visible="false">
<div style="width:35%;float:left;height:800px;">
<h1>Состав&nbsp;<asp:Literal ID="Literal1" runat="server"></asp:Literal> </h1>
 <table class="dev-message">
        <tr>
            <th class="top" colspan="8">
                Редактирование
            </th>
        </tr>
        <tr>
            <th>Обозначение</th>
            <th>Наименование</th>
            <th>Раздел специф</th>
            <th>Количество</th>
            <th>Ед. изм</th>
            <th>Вариант замены</th>
            <th>Группа замены</th>
            <th>Скрытый</th>
        </tr>
        <tr>
            <td>СПЦ13.00.001</td>
            <td>Редуктор</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
            <td>1</td>
            <td>2</td>
            <td></td>
        </tr>
    </table>
    <br />
    <h1>Допускаемые замены</h1>
    <table class="dev-message">
        <tr>
            <th class="top" colspan="8">
                Состав СПЦ123.00.001 (на 28.05.2009 11:23:98)
            </th>
        </tr>
        <tr>
            <th>Обозначение</th>
            <th>Наименование</th>
            <th>Раздел специф</th>
            <th>Количество</th>
            <th>Ед. изм</th>
            <th>Вариант замены</th>
            <th>Группа замены</th>
            <th>Скрытый</th>
        </tr>
        <tr>
            <td>СПЦ13.00.001-01</td>
            <td>Редуктор</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
            <td>2</td>
            <td>3</td>
            <td></td>
        </tr>
        <tr>
            <td>СПЦ13.00.001-02</td>
            <td>Редуктор</td>
            <td>Сб</td>
            <td>1</td>
            <td>Шт.</td>
            <td>3</td>
            <td></td>
            <td></td>
        </tr>
    </table>
</div>
</asp:PlaceHolder>

</asp:Content>
