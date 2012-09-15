<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="SummaryWeight.aspx.cs" Inherits="Aspect.UI.Web.Popup.SummaryWeight" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div style="min-width:300px;width:100%;overflow:auto;float:left;border-right:dotted 1px black;height:100%;height:450px;">
     <table class="type1">
        <tr>
            <th colspan="2">
                Суммарный вес разузлованного состава                
            </th>
        </tr>
        <tr>
            <td>
                Наименование продукта:
            </td>
            <td>
                <%=divProductName %>
            </td>
        </tr>
        <tr>
            <td>
                Суммарный вес разузлованного состава:
            </td>
            <td>
                <%=divSummaryWeight %>
            </td>
        </tr>
        <tr>
            <td>
                Количество проигнорированных продуктов:
            </td>
            <td>
                <%=divIgnore %>
            </td>
        </tr>
    </table>    
</div>

<div style="border-top:solid 1px black;width:100%;float:right">
    <div style="float:right;padding:15px 30px 0px 0px;">
        <a onclick="self.parent.tb_remove();return false;" href="javascript:void(0);">Закрыть</a>
    </div>
</div>
</asp:Content>
