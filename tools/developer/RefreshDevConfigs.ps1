<#
	.SYNOPSIS
	Tool for preparing placeholder filled configs with real values for development.

	.DESCRIPTION
	Tool for preparing placeholder filled configs with real values for development.

	.PARAMETER SourceRoot
	Where the configs are going to go.  Required.

	.PARAMETER Branch
	The name of the CDE/Config branch where the configs should be transformed from.  Required.

	.PARAMETER SubstituteList
	The file path to the placeholder substitution file.  Optional.

	.PARAMETER ConfigRepo
	The path to the local wcms-cde-config repository.  Optional.

#>

Param(
	[Parameter(mandatory=$true, ValueFromPipeline=$false)]
	[string]$SourceRoot,

	[Parameter(mandatory=$true, ValueFromPipeline=$false)]
	[string]$Branch,
	
	[Parameter(mandatory=$false, ValueFromPipeline=$false)]
	[string]$SubstituteList,

	[Parameter(mandatory=$false, ValueFromPipeline=$false)]
	[string]$ConfigRepo
	
)

## Function that actually makes the configs
Function Make-Configs($cdeSrc, $configSrc, $branchName, $subsFile)
{
	## Check the Repos
	$doesConfigSrcExist = [string](git -C $configSrc rev-parse --is-inside-work-tree )
	$doesCDESrcExist =  [string](git -C $cdeSrc rev-parse --is-inside-work-tree)
	
	if ($doesConfigSrcExist -ne "true")
	{
		Write-Host "Configuration source directory, $configSrc , is not a Git repository"
		Exit
	}
	
	if ($doesCDESrcExist -ne "true")
	{
		Write-Host "CDE source directory, $cdeSrc, is not a Git repository"
		Exit
	}
	
	## Check the current branches
	$configSrcBranch = [string](git -C $configSrc rev-parse --abbrev-ref HEAD )
	$cdeSrcBranch =  [string](git -C $cdeSrc rev-parse --abbrev-ref HEAD )
	
	if ($configSrcBranch -ne $branchName)
	{
		Write-Host "Config source directory, $configSrc, is not using branch $branchName"
		Exit
	}
	
	if ($cdeSrcBranch -ne $branchName)
	{
		Write-Host "CDE source directory, $cdeSrc, is not using branch $branchName"
		Exit
	}

	$substCommand = $configSrc + "\tools\build\build-tools\text-substitution\substitution.ps1"
	
	$sites = @("CancerGov", "DCEG", "MobileCancerGov", "TCGA")
	
	foreach ($site in $sites) {
		$sitePath = "CDESites\$site\SiteSpecific\$site.Web"
		$fullCommand = "$substCommand -InputFile `"$configSrc\$sitePath\Web.config`" -OutputFile `"$cdeSrc\$sitePath\Web.config`" -SubstituteList `"$subsFile`""
		
		Invoke-Expression $fullCommand
	}
	
}


############################
### Future GUI Functions

## Load up the GUI assemblies
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Windows.Forms")
[void] [System.Reflection.Assembly]::LoadWithPartialName("System.Drawing")

Function Get-ConfigSrcFolder()
{
	$OpenFolderDialog = New-Object System.Windows.Forms.FolderBrowserDialog
	if ($OpenFolderDialog.ShowDialog() -eq "OK")
	{
		$folder += $OpenFolderDialog.SelectedPath
	}
	
	return $folder
}


Function Get-SubstFileName($initialDirectory)
{
	$OpenFileDialog = New-Object System.Windows.Forms.OpenFileDialog
	$OpenFileDialog.initialDirectory = $initialDirectory
	$OpenFileDialog.filter = "XML (*.xml)| *.xml"
	$OpenFileDialog.ShowDialog() | Out-Null
}

## Code to load UI for selecting Config repo and substitution file.
Function Load-Form()
{
	$Form = New-Object System.Windows.Forms.Form
	$Form.Text = "This should be form/window title"
	$Form.Size = New-Object System.Drawing.Size(300,170)
	$Form.StartPosition = "CenterScreen"
	$Form.KeyPreview = $True
	$Form.MaximumSize = $Form.Size
	$Form.MinimumSize = $Form.Size

	
	$Panel = New-Object System.Windows.Forms.FlowLayoutPanel
	
	$onConfigSrcBtn_Click =
	{
		$folder = Get-ConfigSrcFolder
		Write-Host $folder
	}
	
	## Add selection controls here
	$ConfigSrcButton = New-Object System.Windows.Forms.Button
	##$ConfigSrcButton.Location = New-Object System.Drawing.Size(140,38)
	$ConfigSrcButton.Size = New-Object System.Drawing.Size(175,23)
	$ConfigSrcButton.Text = "Select Config File Repo Directory"
	$ConfigSrcButton.Add_Click($onConfigSrcBtn_Click)
	$Panel.Controls.Add($ConfigSrcButton)

	$onSubstFileBtn_Click =
	{
		$file = Get-SubstFileName $SourceRoot
		Write-Host $file
	}

	## Add selection controls here
	$SubstFileButton = New-Object System.Windows.Forms.Button
	##$SubstFileButton.Location = New-Object System.Drawing.Size(140,38)
	$SubstFileButton.Size = New-Object System.Drawing.Size(175,23)
	$SubstFileButton.Text = "Select Substitution File"
	$SubstFileButton.Add_Click($onSubstFileBtn_Click)
	$Panel.Controls.Add($SubstFileButton)

	$on_submit = 
		{
		##make thing happen
		Write-Host "Button clicked"
		}
		
	$GoButton = New-Object System.Windows.Forms.Button
	##$GoButton.Location = New-Object System.Drawing.Size(140,38)
	$GoButton.Size = New-Object System.Drawing.Size(175,23)
	$GoButton.Text = "Go"
	$GoButton.Add_Click($on_submit)
	$Panel.Controls.Add($GoButton)

	$Form.Controls.Add($Panel)
	$Form.Topmost = $True
	$Form.Add_Shown({$Form.Activate()})
	[void] $Form.ShowDialog()
}

############################################
### Main Function
############################################

if (!$SubstituteList -or !$ConfigRepo)
{
	##Display the form
	Write-Host "One of the config params are missing"	
} else {
	Make-Configs $SourceRoot $ConfigRepo $Branch $SubstituteList
}




