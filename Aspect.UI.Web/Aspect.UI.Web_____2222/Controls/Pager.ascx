<%@ Control Language="C#" AutoEventWireup="true" Inherits="Aspect.UI.Web.Controls.Pager" %>
<div class="navigation">
	<ul>
	    <li><asp:LinkButton ID="BtnBegin" runat="server" CssClass="begin" Text="В начало" OnClick="BtnBegin_Click" /></li>
		<li><asp:LinkButton ID="BtnPrevious" runat="server" Text="Предыдущая" OnClick="BtnPrevious_Click" /></li>
		<asp:PlaceHolder ID="PreviousDots" runat="server" Visible="false">
		    <li>...</li>
		</asp:PlaceHolder>
		<asp:Repeater ID="PagesRepeater" runat="server" OnItemDataBound="PagesRepeater_ItemDataBound">
            <ItemTemplate>
                <li>
                    <asp:HiddenField runat="server" ID="PageIndex" Value="<%# Container.DataItem %>" />
                    <asp:LinkButton ID="BtnPage" runat="server" OnClick="BtnPage_Click" 
                        Visible='<%# ((int)Container.DataItem) != this.CurrentPage %>' 
                        Text='<%# ((int)Container.DataItem+1).ToString() %>' />
                    <asp:Label ID="BtnActivePage" runat="server" 
                        Visible='<%# ((int)Container.DataItem) == this.CurrentPage %>' 
                        Text='<%# ((int)Container.DataItem+1).ToString() %>' />
                 </li>
            </ItemTemplate>
        </asp:Repeater>
        <asp:PlaceHolder ID="NextDots" runat="server" Visible="false">
		    <li>...</li>
		</asp:PlaceHolder>
		<li><asp:LinkButton ID="BtnNext" runat="server" OnClick="BtnNext_Click" Text="Следующая" /></li>
		<li><asp:LinkButton ID="BtnEnd" runat="server" CssClass="end" OnClick="BtnEnd_Click" Text="В конец" /></li>
		<li>Всего записей: <asp:Label Font-Bold="true" ID="TotalRecordsLabel" runat="server"></asp:Label> </li>
	</ul>
	
</div>