using namespace System;
using namespace System.IO;
using namespace System.Collections.Generic;
function Assert-Path([string]$path,[string]$pathName)
{
    if([String]::IsNullOrEmpty($path))
    {
        throw $pathName + "can't be null or empty";
    }
    
    if(!(Test-Path $path))
    {
        throw ($pathName + "is not a valid path!");
    }    
}

#Function for copy,
#When the last modification times of the target file and source file are the same,
#The copy will not be executed;
function Copy-FileEx([string]$originFileName,[string]$targetFileName)
{
    if([File]::Exists($targetFileName))
    {
        $originFileInfo = New-Object FileInfo($originFileName);
        $targetFileInfo = New-Object FileInfo($targetFileName);
        #if the last modification times of the target file and source file are the same,
        #The copy will not be executed;
        if($originFileInfo.LastWriteTime -eq $targetFileInfo.LastWriteTime)
        {
            $msg = $originFileInfo.Name + " No need to copy"
            Write-Output $msg
            return;
        }
    }
    
    
    #If the target directory doesn't exist, create it;
    [string]$targetDirectory = [Path]::GetDirectoryName($targetFileName);
    if(!([Directory]::Exists($targetDirectory)))
    {
        [Directory]::CreateDirectory($targetDirectory);
    }

    
    Copy-Item $originFileName $targetFileName
    $msg = $originFileName + " copied"
    Write-Output $msg
}

#Get all valid lines from a file;
#A valid line is not empty and not start with "#";
function Get-ValidLines([string]$fileName)
{
    $allLines = [File]::ReadAllLines($fileName);
    $validLines = New-Object List[String];
    foreach($line in $allLines)
    {
        $line = [string]$line;
        if([String]::IsNullOrEmpty($line))
        {
            continue;
        }

        if($line.StartsWith("#"))
        {
            continue;
        }

        $validLines.Add($line);
    }

    return $validLines;
}

#Copy the directory recursively;
function Copy-DirectoryEx([string]$originDirectoryName,[string]$tartgetDirectoryName)
{
    Assert-Path $originDirectoryName;
    if(![Directory]::Exists($originDirectoryName))
    {
        $msg = $originDirectoryName + " doesn't exist."
        Write-Error ($msg);
        Exit;
    }

    if(![Directory]::Exists($tartgetDirectoryName))
    {
        [Directory]::CreateDirectory($tartgetDirectoryName);
    }
    
    #Unify the path format;
    $originDirectoryName = [Path]::GetDirectoryName($originDirectoryName+"\\");
    $tartgetDirectoryName = [Path]::GetDirectoryName($tartgetDirectoryName+"\\");
    $innerDirectories = [Directory]::EnumerateDirectories($originDirectoryName,"*.*", [System.IO.SearchOption]::AllDirectories);
    $innerFiles = [Directory]::EnumerateFiles($originDirectoryName,"*", [System.IO.SearchOption]::AllDirectories);

    foreach($directoryName in $innerDirectories)
    {
        $directoryName = [string]$directoryName;
        
        $innerDirectoryName = $directoryName.Substring($originDirectoryName.Length + 1);
        $thisTargetDirectoryName = [Path]::Combine($tartgetDirectoryName,$innerDirectoryName);
        if(![Directory]::Exists($thisTargetDirectoryName))
        {
            [Directory]::CreateDirectory($thisTargetDirectoryName);
        }
    }

    foreach($fileName in $innerFiles)
    {
        $fileName = [string]$fileName;
        
        $innerFileName = $fileName.Substring($originDirectoryName.Length + 1);
        $thisTargetFileName = [Path]::Combine($tartgetDirectoryName,$innerFileName);
        
        Copy-FileEx $fileName $thisTargetFileName
    }
}
