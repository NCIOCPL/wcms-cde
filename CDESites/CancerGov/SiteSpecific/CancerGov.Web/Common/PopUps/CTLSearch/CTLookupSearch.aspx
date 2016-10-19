<%@ Page language="c#" Codebehind="CTLookupSearch.aspx.cs" AutoEventWireup="True" Inherits="CancerGov.Web.CTLookupSearch" ValidateRequest="true" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html>
    <head>
        <link rel="stylesheet" href="/PublishedContent/Styles/nvcg.css" />
		<script language="javascript">
			function doSubmit()
			{
			    userAg = navigator.userAgent;
                bName = navigator.appName;
                bVer  = parseInt(navigator.appVersion);

                window.parent.frames[2].location.href = '/Common/PopUps/CTLSearch/CTLookupSelect.aspx?fld=<%=Request.Params["fld"]%>&type=<%=Request.Params["type"]%>';
				document.lookupSearch.submit();
			}
		</script>
    </head>
  
	<body class="popup">
        <!-- Top Header Section -->
        <div class="clearfix">
		    <div class="popuplogo">
              <a href="/">
                <img alt="National Cancer Institute" src="/publishedcontent/images/images/design-elements/logos/nci-logo-full.svg"/>
              </a> 
            </div>

	    </div> 
        <!-- end Top Header Section -->

        <form name="lookupSearch" class="ct-popup-form" method="get" action="/Common/PopUps/CTLSearch/CTLookupResults.aspx" target="results" onsubmit="javascript: document.forms[0].alphaIndex.value=''; doSubmit();">
	        <input type="hidden" name="title" value="<%=Title%>">
	        <input type="hidden" name="alphaIndex" value="<%=InputAlphaIndex%>">
	        <input type="hidden" name="fld" value="<%=Request.Params["fld"]%>">
            <h5><%=Title%></h5>
            <p id="caption"><%=Caption%></p>
            
            <%=AlphaIndexLinks%>
            <div class="ct-popup-line">
                <label class="ct-popup-label" for="SearchBox"><%=TextInputPrompt%>:</label>
                <input type="text" id="SearchBox" name="keyword" size="20" value="<%=InputKeyword%>" />
                <button class="button action">Search</button>
            </div>
         </form>
	</body>
</html>
