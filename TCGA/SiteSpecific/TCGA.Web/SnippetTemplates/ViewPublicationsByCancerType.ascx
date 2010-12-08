﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ViewPublicationsByCancerType.ascx.cs"
    Inherits="TCGA.Web.SnippetTemplates.ViewPublicationsByCancerType" %>
<%@ Register TagPrefix="TCGA" Assembly="NCILibrary.Web.UI.WebControls" Namespace="NCI.Web.UI.WebControls" %>
<Style>
.pager {
	float: left;
	font-weight: bold;
	padding-right: 8px;
}
.pager input {
	background: none;
	border: none;
	cursor: pointer;
	color: #9C3303;
	font-size: 12px;
	padding: 0px 0px 0px 0px;
	text-decoration: underline;
}    
</Style>
<form runat="server" id="frmViewPublications">
    <h2>
        Publications</h2>
    <p>
        All data generated by The Cancer Genome Atlas (TCGA) Research Network are made open
        to the public through the Data Coordinating Center and the TCGA Data Portal.</p>
    <p>
        View Publications by Cancer Type</p>
    <asp:DropDownList AutoPostBack="true" runat="server" ID="ddlCancerType" Width="600px"
        OnSelectedIndexChanged="ddlCancerType_SelectedIndexChanged">
    </asp:DropDownList>
    <p>
        * = TCGA Research Network</p>
    <asp:Repeater runat="server" ID="rptPublicationResults">
        <HeaderTemplate>
            <ul class="unbulleted">
        </HeaderTemplate>
        <ItemTemplate>
            <li>
                <div class="citation">
                    <%# Eval("isTCGANetworkType")%>&nbsp;<%# Eval("description")%>&nbsp;(<%# Eval("publicationDate")%>)&nbsp;<%# Eval("journalTitle")%>&nbsp<%# Eval("associatedLink")%>
                </div>
            </li>
        </ItemTemplate>
        <FooterTemplate>
            </ul>
        </FooterTemplate>
    </asp:Repeater>
    
    <TCGA:PostBackButtonPager id="pager" runat="server" cssclass="pager"
        onpagechanged="pager_PageChanged" ShowNumPages="5">
        <pagerstylesettings nextpagetext="Next &gt;" prevpagetext="&lt; Prev" />
    </TCGA:PostBackButtonPager>
    <asp:HiddenField ID="itemsPerPage" Value="5" runat="server" />
</form>
