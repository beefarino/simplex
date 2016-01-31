#
# Copyright (c) 2014 Code Owls LLC
#
# Permission is hereby granted, free of charge, to any person obtaining a copy
# of this software and associated documentation files (the "Software"), to
# deal in the Software without restriction, including without limitation the
# rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
# sell copies of the Software, and to permit persons to whom the Software is
# furnished to do so, subject to the following conditions:
#
# The above copyright notice and this permission notice shall be included in
# all copies or substantial portions of the Software.
#
# THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
# IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
# FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
# AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
# LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
# FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
# IN THE SOFTWARE.
#
# 	psake build script for the Simplex module
#
# 	valid configurations:
#  		Debug
#  		Release
#
# notes:
#

properties {
	$config = 'Debug';
	$slnFile = @(
		'./src/Simplex.sln'
	);
    $targetPath = "./src/CodeOwls.ScriptProvider/bin";

    $moduleName = "Simplex";
	$moduleSource = "./src/Modules";
    $metadataAssembly = 'CodeOwls.ScriptProvider.dll'
    $currentReleaseNotesPath = '.\src\Modules\$moduleName\en-US\about_Simplex_Version.help.txt'
};

framework '4.0'

$private = "this is a private task not meant for external use";

task default -depends Install;

# private tasks

task __InitializeBuild -description $private {
	$error.clear();
}

task __VerifyConfiguration -depends __InitializeBuild -description $private {
	Assert ( @('Debug', 'Release') -contains $config ) "Unknown configuration, $config; expecting 'Debug' or 'Release'";
	Assert ( Test-Path $slnFile ) "Cannot find solution, $slnFile";

	Write-Verbose ("packageDirectory: " + ( get-packageDirectory ));
}

task __CreatePackageDirectory -description $private {
	get-packageDirectory | create-packageDirectory;
}

task __CreateModulePackageDirectory -description $private {
	get-modulePackageDirectory | create-packageDirectory;
}

# primary targets

task Build -depends __VerifyConfiguration -description "builds any outdated dependencies from source" {
	exec {
		msbuild $slnFile /p:Configuration=$config /t:Build
	}
}

task Clean -depends __VerifyConfiguration,CleanModule -description "deletes all temporary build artifacts" {
	exec {
		msbuild $slnFile /p:Configuration=$config /t:Clean
	}
}

task Rebuild -depends Clean,Build -description "runs a clean build";

task Package -depends PackageModule -description "assembles distributions in the source hive"

# clean tasks

task CleanModule -depends __CreateModulePackageDirectory -description "clears the module package staging area" {
    get-modulePackageDirectory |
        remove-item -recurse -force;
}

# package tasks

task PackageModule -depends CleanModule,Build,__CreateModulePackageDirectory -description "assembles module distribution file hive" -action {
	$mp = get-modulePackageDirectory;
    $psdFile = "$mp/$moduleName/$moduleName.psd1";
    $bin = "$mp/$moduleName/bin";
	$version = get-packageVersion;

    write-verbose "package module $moduleName in $mp with version $version";

	# copy module src hive to distribution hive
	Copy-Item $moduleSource -container -recurse -Destination $mp -Force;

	# copy bins to module bin area
    mkdir $bin -force | out-null;
	get-targetOutputPath | ls | copy-item -dest $bin -recurse -force;

    $psd = get-content $psdFile;
    $psd -replace "ModuleVersion = '[\d\.]+'","ModuleVersion = '$version'" | out-file $psdFile;
}

# install tasks

task Uninstall -description "uninstalls the module from the local user module repository" {
	$modulePath = $Env:PSModulePath -split ';' | select -First 1 | Join-Path -ChildPath $moduleName;
	if( Test-Path $modulePath )
	{
		Write-Verbose "uninstalling from local module repository at $modulePath";

		$modulePath | ri -Recurse -force;
	}
}

task Install -depends InstallModule -description "installs the module to the local machine";

task InstallModule -depends PackageModule -description "installs the module to the local user module repository" {
	$packagePath = get-modulePackageDirectory;
	$modulePath = $Env:PSModulePath -split ';' | select -First 1;
	Write-Verbose "installing to local module repository at $modulePath";

	ls $packagePath | Copy-Item -recurse -Destination $modulePath -Force;
}

function get-packageDirectory
{
	return "." | resolve-path | join-path -child "/build/$config";
}

function get-modulePackageDirectory
{
    return "." | resolve-path | join-path -child "/build/$config/Modules";
}

function create-PackageDirectory( [Parameter(ValueFromPipeline=$true)]$packageDirectory )
{
    process
    {
        write-verbose "checking for package path $packageDirectory ..."
        if( !(Test-Path $packageDirectory ) )
    	{
    		Write-Verbose "creating package directory at $packageDirectory ...";
    		mkdir $packageDirectory | Out-Null;
    	}
    }
}

function get-targetOutputPath
{
    $targetPath | join-path -childPath $config
}

function get-packageVersion
{
    $md = $targetPath | join-path -childPath $config | join-path -ChildPath $metadataAssembly;
	( get-item $md | select -exp versioninfo | select -exp productversion )
}
