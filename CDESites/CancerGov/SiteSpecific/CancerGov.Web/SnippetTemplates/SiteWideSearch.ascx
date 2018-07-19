<%@ Control Language="C#" AutoEventWireup="true" Inherits="NCI.Web.CancerGov.Apps.SiteWideSearchModule"  %>
<%@ Register assembly="NCILibrary.Web.UI.WebControls" namespace="NCI.Web.UI.WebControls" tagprefix="cc1" %>
<script runat="server">

    protected void searchResults_ItemCreated(object sender, RepeaterItemEventArgs e)
    {

    }
</script>


<script type="text/javascript">
    var ids = {
    txtSWRKeyword: "<%=txtSWRKeyword.ClientID %>"
}
</script>


	<!-- Main Area -->
	<div class="results">
		<!-- Main Content Area -->

        <cc1:JavascriptProbeControl ID="jsProbe" runat="server" />
        <h3><asp:Label id="lblResultsForText" Text="Results for:" runat="server"/> <asp:Label CssClass="term" ID="lblResultsForKeyword" runat="server" /></h3>
     
        <!-- Best Bets Here --> 
        <asp:Repeater ID="rptBestBets" EnableViewState="false" runat="server">
            <ItemTemplate>
                <div class="featured sitewide-results" data-events="event10">
                 <h2>
                        <asp:Label
                            ID="lblBBCatName" 
                            Text='<%# (PageDisplayInformation.Language == NCI.Web.CDE.DisplayLanguage.Spanish ? "Mejores resultados para " : "Best Bets for ") + Eval("CategoryName")%>'
                            runat="server" />
                    </h2>
                    <asp:Literal ID="Literal1" runat="server" Text='<%# BestBetResultsHyperlinkOnclick(Container) %>'>
                    </asp:Literal>
                </div>    
            </ItemTemplate>
        </asp:Repeater>
                        
        <!-- Results x-y of z for: Keyword -->
        <div class="sitewide-results">
            <h4><asp:Label ID="lblTopResultsXofY" runat="server" /> <span class="term"><asp:Label ID="lblTopResultsXofYKeyword" runat="server" /></span></h4>
        
            <!-- Error message, used to be ResultsView -->
            <asp:PlaceHolder ID="phError" runat="server">
                <div>
                    <asp:Literal ID="litError" runat="server" Text="Please enter a search phrase." />
                </div>
            </asp:PlaceHolder>
           
            <cc1:MultiTemplatedRepeater ID="rptResults" EnableViewState="false" runat=server visible="true">
                <HeaderTemplate>
                    <div class="sitewide-list" >
                        <ul class="no-bullets">
                </HeaderTemplate>
                <ItemTemplates>
                    <cc1:TemplateItem TemplateType="Default">
                        <Template>
                            <li>
                                <div>
                                <asp:HyperLink
                                    id="HyperLink1"
                                    runat="server"
                                    onclick='<%# ResultsHyperlinkOnclick(Container) %>'                                   
                                    NavigateUrl='<%# Eval("Url") %>'
                                    Text='<%# Eval("Title") %>' />
                                </div>
                                <div class="description">
		                            <%# Eval("Description") %>
		                        </div> 
		                        <div>
		                            <cite class="url">
		                            <%# Eval("Url") %>
		                            </cite>
		                        </div>
		                    </li>                    
                        </Template>		                    
                    </cc1:TemplateItem>
                    <cc1:TemplateItem TemplateType="Media">
                        <Template>
                            <li>
                                <div>
                                <asp:HyperLink
                                    id="lnkResultLink"
                                    runat="server"
                                    onclick='<%# ResultsHyperlinkOnclick(Container) %>'                                   
                                    NavigateUrl='<%# Eval("Url") %>'
                                    Text='<%# Eval("Title") %>' /> 
                                     <span class="media-type">(<%# Eval("Label") %>)</span>
                                </div>
		                        <div class="description">
		                            <%# Eval("Description") %>
		                        </div>
                                <div>
		                            <cite class="url">
		                            <%# Eval("Url") %>
		                            </cite>
		                        </div>

		                    </li>
                        </Template>		                    
                    </cc1:TemplateItem>                                            
                </ItemTemplates>
                <FooterTemplate>
                        </ul>
                    </div>
                </FooterTemplate>
            </cc1:MultiTemplatedRepeater>


            
        </div>
        
        <form id="resultForm" runat="server" class="sitewide-search-results">
            <!-- pager and selection -->
            <div class="results-pager">
                <h2 class="results-count"><asp:Label ID="lblBottomResultsXofY" runat="server" /></h2>
                <span>
                    <asp:Label id="lblDDLPageUnitShowText" Text="Show" runat="server"/>
                    <asp:Label ID="lblPageUnit" runat="server" CssClass="hidden" Text="Number of results to show on a page" AssociatedControlID="ddlPageUnit" />
                    <asp:DropDownList
                        id="ddlPageUnit"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="ChangeItemsPerPageAndBind"
                        Style="width: 88px"
                        runat="server">
                        <asp:ListItem Text="10" Value="10" />
                        <asp:ListItem Text="20" Value="20" />
                        <asp:ListItem Text="50" Value="50" />
                    </asp:DropDownList>
                    <noscript>
                        <asp:Button 
                            ID="btnTextChangePageUnit" 
                            Text="Go"
                            OnClick="ChangePageUnitBtnClick"
                            runat="server" />
                    </noscript>
			        <asp:Label id="lblDDLPageUnitResultsPPText" Text="results per page." runat="server"/>
			    </span>
                <cc1:simpleulpager ID="spPager" runat="server" CssClass="pagination"></cc1:simpleulpager>
	        </div>
	    
	        <h4>
                <asp:Label ID="lblSearchWithinResultsFound" runat="server" />
                <asp:Label CssClass="term" ID="lblSearchWithinResultKeyword" runat="server" />
            </h4>
		    <asp:Panel ID="pnlSWR" runat="server">

                <asp:RadioButtonList CssClass="radio" ID="rblSWRSearchType" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
                    <asp:ListItem Selected="True" Text="New Search" Value="1" />
                    <asp:ListItem Text="Search Within Results" Value="2" />
                </asp:RadioButtonList>
          
                <div class="row collapse">
                    <div class="small-12">
                        <asp:Label AssociatedControlID="txtSWRKeyword" ID="lblSWRKeywordLabel" runat="server" CssClass="hidden">Keyword</asp:Label>
                        <asp:TextBox CssClass="input" ID="txtSWRKeyword" size=40 Rows="40" MaxLength="40" runat="server" />
                        <asp:Button CssClass="button submit" id="btnSWRTxtSearch" text="Search" runat="server" OnClick="SearchWithinResults" />
                    </div>
                </div>

		    </asp:Panel>
        </form>                    				
	</div>
	<!-- End Main Area -->
