<%@ Control Language="C#" AutoEventWireup="true" Inherits="CancerGov.Dictionaries.SnippetControls.DrugDictionary.DrugDictionaryDefinitionView" %>
<%@ Register TagPrefix="DictionarySearchBlock" TagName="SearchBlock" Src="~/SnippetTemplates/TermDictionary/DictionarySearchBlock.ascx" %>
<%@ Import Namespace="NCI.Web.Dictionary.BusinessObjects" %>
 
<DictionarySearchBlock:SearchBlock id="dictionarySearchBlock" runat="server" />
    
<asp:Repeater ID="drugDictionaryDefinitionView" runat="server" OnItemDataBound="drugDictionaryDefinitionView_OnItemDataBound">
<ItemTemplate>
    <div class="results" data-dict-type="drug">
        <!-- Term and def -->
        <dl>
            <dt>
                <dfn data-cdr-id="<%# ((DictionaryTerm)(Container.DataItem)).ID%>">
                    <span><%# ((DictionaryTerm)(Container.DataItem)).Term%></span>
                </dfn>
            </dt>
            <dd class="info-summary" id="ddPatientInfo" runat="server" Visible="false">
                <asp:HyperLink ID="hlPatientInfo" runat="server" Visible="true">
                    <img src="/images/btn-patient-info.gif" alt="Patient Information" title="Patient Information" width="139" height="20" hspace="12" border="0"  align="absmiddle" />
                </asp:HyperLink>
            </dd>
            <dd class="definition">

                <%# ((DictionaryTerm)(Container.DataItem)).Definition.Html%>
                
                <%-- Location to display the list of aliases.  Code behind will make it visible if
                there's anything to show. --%>
                <asp:PlaceHolder id="phAliasList" runat="server" Visible="false">
                    <%# GenerateTermAliasList(Container.DataItem)%>
                </asp:PlaceHolder>
                
            </dd>
        </dl>
    </div>    
</ItemTemplate> 
</asp:Repeater>
