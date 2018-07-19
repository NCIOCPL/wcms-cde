<%@ Control Language="C#" AutoEventWireup="true" Inherits="CancerGov.Dictionaries.SnippetControls.GeneticsTermDictionary.GeneticsTermDictionaryDefintionView" %>
<%@ Import Namespace="NCI.Web.Dictionary.BusinessObjects" %>
<%@ Register TagPrefix="DictionarySearchBlock" TagName="SearchBlock" Src="~/SnippetTemplates/TermDictionary/DictionarySearchBlock.ascx" %>

<DictionarySearchBlock:SearchBlock ID="dictionarySearchBlock" runat="server" />

<asp:Repeater ID="termDictionaryDefinitionView" runat="server" OnItemDataBound="termDictionaryDefinitionView_OnItemDataBound">
    <ItemTemplate>
        <!-- Term and def -->
        <div class="results" data-dict-type="genetic" data-events="event11">
            <dl class="dictionary-list">
                <dt><dfn data-cdr-id="<%# ((DictionaryTerm)(Container.DataItem)).ID%>">
                    <%# ((DictionaryTerm)(Container.DataItem)).Term%>
                </dfn></dt>
                <asp:PlaceHolder ID="phPronunciation" runat="server">
                    <dd class="pronunciation">
                        <a id="pronunciationLink" runat="server" class="CDR_audiofile"><span class="hidden">
                            listen</span></a>
                        <asp:Literal ID="pronunciationKey" runat="server" />
                    </dd>
                </asp:PlaceHolder>
                <dd class="definition">
                    <%# ((DictionaryTerm)(Container.DataItem)).Definition.Html%>
                    <asp:Panel runat="server" ID="pnlRelatedInfo">
                        <div class="related-resources">
                            <h6>
                                <asp:Literal ID="litMoreInformation" runat="server" /></h6>
                            <asp:Repeater ID="relatedExternalRefs" runat="server" Visible="false">
                                <HeaderTemplate>
                                    <ul class="no-bullets">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li><a href="<%# ((RelatedExternalLink)(Container.DataItem)).Url  %>">
                                        <%# ((RelatedExternalLink)(Container.DataItem)).Text  %></a></li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            </asp:Repeater>
                            <asp:Repeater ID="relatedSummaryRefs" runat="server" Visible="false">
                                <HeaderTemplate>
                                    <ul class="no-bullets">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li><a href="<%# ((RelatedSummary)(Container.DataItem)).url  %>">
                                        <%# ((RelatedSummary)(Container.DataItem)).Text  %></a></li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            </asp:Repeater>
                            <asp:Repeater ID="relatedDrugInfoSummaries" runat="server" Visible="false">
                                <HeaderTemplate>
                                    <ul class="no-bullets">
                                </HeaderTemplate>
                                <ItemTemplate>
                                    <li><a href="<%# ((RelatedDrugSummary)(Container.DataItem)).url  %>">
                                        <%# ((RelatedDrugSummary)(Container.DataItem)).Text  %></a></li>
                                </ItemTemplate>
                                <FooterTemplate>
                                    </ul>
                                </FooterTemplate>
                            </asp:Repeater>
                            <asp:PlaceHolder ID="phRelatedTerms" runat="server" Visible="false">
                                <p>
                                    <asp:Label ID="labelDefintion" runat="server" class="related-definition-label" />
                                    <asp:Repeater ID="relatedTerms" runat="server" OnItemDataBound="relatedTerms_OnItemDataBound">
                                        <ItemTemplate>
                                            <asp:HyperLink ID="relatedTermLink" runat="server" /><asp:Literal ID="relatedTermSeparator"
                                                runat="server" Text="," Visible="false" />
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </p>
                            </asp:PlaceHolder>

                            <asp:Repeater ID="relatedImages" runat="server" Visible="false" OnItemDataBound="relatedImages_OnItemDataBound">
                                <ItemTemplate>
                                    <figure class="image-left-medium">
                                    <a id="termEnlargeImage" runat="server" target="_blank" class="article-image-enlarge no-resize">Enlarge</a>
                                    <img id="termImage" runat="server" src="" alt="" />
                                        <figcaption>
                                            <div class="caption-container no-resize">
                                            <p><%# ((ImageReference)(Container.DataItem)).Caption  %></p>
                                            </div>
                                        </figcaption>
                                 </figure>
                                </ItemTemplate>
                            </asp:Repeater>

                            <asp:Repeater ID="relatedVideos" runat="server" Visible="false" OnItemDataBound="relatedVideos_OnItemDataBound">
                                <ItemTemplate>
                                <figure runat="server" id="videoContainer">
                                    <h4 runat="server" visible="false" id="videoTitle"></h4>
                                    <div id="ytplayer-<%# ((VideoReference)(Container.DataItem)).UniqueID %>"
                                         class="flex-video widescreen"
                                         data-video-id="<%# ((VideoReference)(Container.DataItem)).UniqueID %>"
                                         data-video-title="<%# ((VideoReference)(Container.DataItem)).Title %>">
                                        <noscript><p><a href="https://www.youtube.com/watch?v=<%# ((VideoReference)(Container.DataItem)).UniqueID %>" target="_blank">View this video on YouTube.</a></p></noscript>
                                    </div>
                                    <figcaption class="caption-container no-resize" id="captionContainer" Visible="false" runat="server"></figcaption>
                                </figure>
                                </ItemTemplate>
                            </asp:Repeater>

                        </div>
                    </asp:Panel>
                </dd>
            </dl>
        </div>
    </ItemTemplate>
</asp:Repeater>
