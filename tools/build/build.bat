@echo off
SETLOCAL

@set PATH=C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\CommonExtensions\Microsoft\TestWindow;C:\Program Files (x86)\Microsoft SDKs\F#\3.1\Framework\v4.0\;C:\Program Files (x86)\Microsoft SDKs\TypeScript\1.0;C:\Program Files (x86)\MSBuild\12.0\bin;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\IDE\;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\BIN;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Common7\Tools;C:\Windows\Microsoft.NET\Framework\v4.0.30319;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\VCPackages;C:\Program Files (x86)\HTML Help Workshop;C:\Program Files (x86)\Microsoft Visual Studio 12.0\Team Tools\Performance Tools;C:\Program Files (x86)\Windows Kits\8.1\bin\x86;C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1A\bin\NETFX 4.5.1 Tools\;%PATH%
@set INCLUDE=C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\INCLUDE;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\ATLMFC\INCLUDE;C:\Program Files (x86)\Windows Kits\8.1\include\shared;C:\Program Files (x86)\Windows Kits\8.1\include\um;C:\Program Files (x86)\Windows Kits\8.1\include\winrt;
@set LIB=C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\LIB;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\ATLMFC\LIB;C:\Program Files (x86)\Windows Kits\8.1\lib\winv6.3\um\x86;
@set LIBPATH=C:\Windows\Microsoft.NET\Framework\v4.0.30319;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\LIB;C:\Program Files (x86)\Microsoft Visual Studio 12.0\VC\ATLMFC\LIB;C:\Program Files (x86)\Windows Kits\8.1\References\CommonConfiguration\Neutral;C:\Program Files (x86)\Microsoft SDKs\Windows\v8.1\ExtensionSDKs\Microsoft.VCLibs\12.0\References\CommonConfiguration\neutral;

REM Get the branch from the command line
set my_branch=%1

REM Determine the Build Environment Name.  This is used for tagging and proper config deployment

rem Get the build environment name from the command line
set my_target=%2

REM Validate Branch and Target
set FAIL=
IF "%my_branch%"=="" set FAIL=True
IF "%my_target%"=="" set FAIL=True
IF "%WORKSPACE%"=="" set FAIL=True
IF "%TEMP%"=="" set FAIL=True
IF "%BUILD_NUMBER%"=="" set FAIL=True
IF "%GITHUB_TOKEN%"=="" set FAIL=True


IF "%FAIL%" NEQ "" (
	ECHO.
	ECHO You must pass the branch and build environment names and to this script.
	ECHO USAGE:
	ECHO 	c:\Build.bat ^<branch^> ^<environment^>
	ECHO Additonally, the following environment variables must be set:
	ECHO 	WORKSPACE - directory containing the source code.
	ECHO	TEMP - Location for temporary files
	ECHO	BUILD_NUMBER - Build number ^(automatically generated/set by Jenkins^)
	ECHO	GITHUB_TOKEN - GitHub access token for the build user.
	GOTO :EOF
)

rem Set the temp directory for the config files.
set TEMP_BUILD=%TEMP%\wcms-cde-%RANDOM%

REM Determine the current Git commit hash.
FOR /f %%a IN ('git rev-parse --verify HEAD') DO SET COMMIT_ID=%%a

REM Insert placeholder configuration files.
call "%WORKSPACE%\tools\build\insert-placeholders.bat"

ECHO Building for %my_target% using Branch %my_branch%
msbuild /fileLogger /t:All "/p:TargetEnvironment=%my_target%;Branch=%my_branch%"  "%WORKSPACE%\tools\build\BuildCDE.xml"
IF errorlevel 1 GOTO ERROR

Echo Retrieving configuration file build
REM Create a directory for the configuration source.
set ConfigDownload=%TEMP_BUILD%\Config
mkdir %ConfigDownload%
pushd %ConfigDownload%
REM Copy the configuration files from GitHub.
git clone --recurse-submodules --branch %my_branch% https://github.com/NCIOCPL/wcms-cde-config %ConfigDownload%
IF errorlevel 1 GOTO ERROR

REM SET environment variables for Config file build
set OLD_WORKSPACE=%WORKSPACE%
set WORKSPACE=%ConfigDownload%
set OLD_GH_REPO_NAME=%GH_REPO_NAME%
set GH_REPO_NAME=wcms-cde-config
SET CDE_COMMIT_ID=%COMMIT_ID%
FOR /f %%a IN ('git rev-parse --verify HEAD') DO SET CONFIG_COMMIT_ID=%%a
call tools\build\BuildConfig.bat %my_branch% %my_target%
IF errorlevel 1 GOTO ERROR

REM Restore environment variables.
set WORKSPACE=%OLD_WORKSPACE%
set GH_REPO_NAME=%OLD_GH_REPO_NAME%

REM Clean up.
popd %ConfigDownload%
rmdir /q/s %ConfigDownload%
rmdir /q/s %TEMP_BUILD%

GOTO :EOF
:ERROR
echo. An error has occured.
exit /b 1
