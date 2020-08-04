@echo off
SETLOCAL

REM Insert placeholders for web.config / app.config
set placeholder=%WORKSPACE%\tools\build\resources\placeholder.config
copy "%placeholder%" "CDEFramework\Libraries\NCILibrary\Code\NCILibrary.Services.Dictionary\Web.config"

copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\CancerGov.Web\web.config"
copy "%placeholder%" "CDESites\DCEG\SiteSpecific\DCEG.Web\web.config"
copy "%placeholder%" "CDESites\Imaging\SiteSpecific\Imaging.Web\web.config"
copy "%placeholder%" "CDESites\MobileCancerGov\SiteSpecific\MobileCancerGov.Web\web.config"
copy "%placeholder%" "CDESites\Proteomics\SiteSpecific\Proteomics.Web\web.config"
copy "%placeholder%" "CDESites\TCGA\SiteSpecific\TCGA.Web\web.config"

copy "%placeholder%" "CDEFramework\Libraries\LegacyDictionarySupport\app.config"
copy "%placeholder%" "CDEFramework\Libraries\NCILibrary\Code\NCILibrary.Modules.Search\app.config"
copy "%placeholder%" "CDEFramework\Libraries\NCILibrary\Code\NCILibrary.Search.BestBets\app.config"
copy "%placeholder%" "CDEFramework\Libraries\NCILibrary\Code\NCILibrary.Web.CDE\app.config"
copy "%placeholder%" "CDEFramework\Libraries\NCILibrary\Code\NCILibrary.Web.CDE.UI\app.config"
copy "%placeholder%" "CDEFramework\Libraries\NCILibrary\UnitTests\NCILibrary.Web.CDE.Test\app.config"
copy "%placeholder%" "CDESites\CancerGov\CancerGovSpecific\CancerGov.Handlers\app.config"
copy "%placeholder%" "CDESites\CancerGov\CancerGovSpecific\CancerGov.Modules\app.config"
copy "%placeholder%" "CDESites\CancerGov\PresentationClasses\app.config"
copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\Modules\CancerGov.APICTS.Test\app.config"
copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\Modules\CancerGov.BasicCTS\app.config"
copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\Modules\CancerGov.BasicCTSv2\app.config"
copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\Modules\CancerGov.ClinicalTrialsAPI\app.config"
copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\Modules\CancerGov.ClinicalTrialsAPI.Test\app.config"
copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\Modules\CancerGov.Search.AutoSuggest\app.config"
copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\Modules\CancerGov.Search.BestBets\app.config"
copy "%placeholder%" "CDESites\CancerGov\SiteSpecific\Modules\CancerGov.Search.Endeca\app.config"

REM Insert placeholder web.config transform files
set placeholder=%WORKSPACE%\tools\build\resources\placeholder-transform.config
for %%a in (Blue COLO DT Pink Prod QA Red Stage Training ) do  (
    for %%b in (CancerGov DCEG TCGA ) do (
        for %%c in (live preview) do (
            copy "%placeholder%" "CDESites\%%b\SiteSpecific\%%b.Web\Web.%%a-%%c.config"
        )
    )
)