<%@ Page Title="" Language="C#" MasterPageFile="~/Popup/PopupMaster.Master" AutoEventWireup="true" CodeBehind="ProductActions.aspx.cs" Inherits="Aspect.UI.Web.Popup.ProductActions" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HyperLink ID="EditObject" runat="server" Font-Size="20px" NavigateUrl="~/Editarea/Edit.aspx" onclick="tb_remove();" Target="_blank">Редактировать</asp:HyperLink><br /><br />
    <asp:HyperLink ID="ViewObject" runat="server" Font-Size="20px" NavigateUrl="~/Editarea/View.aspx" onclick="tb_remove();" Target="_blank">Просмотр</asp:HyperLink><br /><br />
    <asp:HyperLink ID="NewObject" runat="server" Font-Size="20px" NavigateUrl="~/Editarea/Edit.aspx" onclick="tb_remove();" Target="_blank">Добавить новый</asp:HyperLink><br /><br />
    <asp:HyperLink ID="AttachedFiles" runat="server" Font-Size="20px">Присоединяемые файлы</asp:HyperLink><br /><br />
    <!-- Только для продуктов -->
    <asp:HyperLink ID="NewObjectWithConfs" runat="server" Font-Size="20px" NavigateUrl="~/Editarea/NewVersion.aspx" onclick="tb_remove();" Target="_blank">Добавить новую версию с составом</asp:HyperLink><br /><br />
    <hr /><br />
    <asp:HyperLink ID="Edit" runat="server" Font-Size="20px" NavigateUrl="~/Configuration/Edit.aspx" onclick="tb_remove();" Target="_blank">Редактировать спецификацию</asp:HyperLink><br /><br />
    <asp:HyperLink ID="EditReadOnly" runat="server" Font-Size="20px" NavigateUrl="~/Configuration/Edit.aspx" onclick="tb_remove();" Target="_blank">Просмотр спецификации</asp:HyperLink><br /><br />
    <asp:HyperLink ID="Usage" runat="server" Font-Size="20px" NavigateUrl="~/Configuration/Usage.aspx" onclick="tb_remove();" Target="_blank">Применяемость по спецификациям</asp:HyperLink><br /><br />
    <asp:HyperLink ID="UsageWaves" runat="server" Font-Size="20px" NavigateUrl="~/Technology/Applicability.aspx" onclick="tb_remove();" Target="_blank">Применяемость по изделиям</asp:HyperLink><br /><br />
    <asp:HyperLink ID="View" runat="server" Font-Size="20px" NavigateUrl="~/Configuration/View.aspx" onclick="tb_remove();" Target="_blank">Состав</asp:HyperLink><br /><br />
    <asp:HyperLink ID="Tree" runat="server" Font-Size="20px" NavigateUrl="~/Configuration/Tree.aspx" onclick="tb_remove();" Target="_blank">Дерево состава </asp:HyperLink> <asp:HyperLink ID="TreeEx" runat="server" Font-Size="20px" NavigateUrl="~/Configuration/TreeEx.aspx" onclick="tb_remove();" Target="_blank">(расширенное)</asp:HyperLink><br /><br />
    <asp:HyperLink ID="TreeWithKmh" runat="server" Font-Size="20px" NavigateUrl="~/Technology/TreeWithKmh.aspx" onclick="tb_remove();" Target="_blank">Разузлованный технологический состав</asp:HyperLink><br /><br />
    <asp:HyperLink ID="EditKmh" runat="server" Font-Size="20px" NavigateUrl="~/Technology/EditorKmh.aspx" onclick="tb_remove();" Target="_blank">Редактировать КМХ</asp:HyperLink><br /><br />
</asp:Content>
