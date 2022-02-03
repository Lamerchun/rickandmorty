$server = $env:SERVER
$remoteDirectory = $env:REMOTE_DIRECTORY
$user = $env:USER
$password = $env:PASSWORD
$artifactDirectory = $env:ARTIFACT_DIRECTORY

Write-Host "Server: ${server}"
Write-Host "Remote directory: ${remoteDirectory}"
Write-Host "User: ${user}"
Write-Host "Password: ${password}"
Write-Host "Artifact directory: ${artifactDirectory}"

if([string]::IsNullOrEmpty($remoteDirectory))
{
    Throw "Require remote REMOTE_DIRECTORY env variable."
}

if([string]::IsNullOrEmpty($server))
{
    Throw "Require SERVER env variable."
}

if([string]::IsNullOrEmpty($user))
{
    Throw "Require USER env variable."
}

if([string]::IsNullOrEmpty($password))
{
    Throw "Require PASSWORD env variable."
}

if([string]::IsNullOrEmpty($artifactDirectory))
{
    Throw "Require ARTIFACT_DIRECTORY env variable."
}

$reset = $false
$remoteDirectory = $remoteDirectory.ToLower()
$serverUrl = "ftp://$server/$remoteDirectory"
$credentials = New-Object System.Net.NetworkCredential($user, $password)

function GetLocalFiles($prefix, $path)
{
    $result = @{}

    $files = Get-ChildItem $path -File

    foreach ($file in $files)
    {
        $relative = "$prefix$($file.Name)"
        $localPath = $path + '\\' + $file.Name
        $hash = Get-FileHash $localPath -Algorithm MD5
        $result[$relative] = $hash.Hash
    }

    $directories = Get-ChildItem $path -Directory

    foreach ($directory in $directories)
    {
        if($prefix -eq '/' -and $directory.Name -eq 'App_Data')
        {
            continue;
        }

        if($prefix -eq '/' -and $directory.Name -eq 'Cache')
        {
            continue;
        }

        $sub = GetLocalFiles "$prefix$($directory.Name)/" "$path\\$($directory.Name)"

        if($sub)
        {
            $result += $sub
        }
    }

    return $result
}

function DeleteFile($relative) 
{
    Try
    {
        Write-Host "Delete file $relative"
        $deleteRequest = [Net.WebRequest]::Create($serverUrl + $relative)
        $deleteRequest.Credentials = $credentials
        $deleteRequest.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile
        $deleteRequest.GetResponse() | Out-Null
    }
    Catch
    {
        Write-Host "Not found file $relative"
    }
}

function UploadFile($relative) 
{
    $content = [System.IO.File]::ReadAllBytes($artifactDirectory + $relative)
    UploadContent $relative $content
}

function UploadContentRaw($relative, $content) 
{
    $request = [Net.WebRequest]::Create($serverUrl + $relative)
    $request.Credentials = $credentials
    $request.Method = [System.Net.WebRequestMethods+Ftp]::UploadFile
    $request.UseBinary = $true;
    $request.UsePassive = $true;
    $request.ContentLength = $content.Lenght

    $rs = $request.GetRequestStream()
    $rs.Write($content, 0, $content.Length)
    $rs.Close()
    $rs.Dispose()
}

function UploadContent($relative, $content) 
{
    Try
    {
        Write-Host "Upload file $relative"
        UploadContentRaw $relative $content
    }
    Catch
    {
        MakeDirectory $relative
        UploadContentRaw $relative $content
    }
}

function MakeDirectory($relative, $content)
{
    $directory = Split-Path -Path $relative
    Write-Host "Create directory $directory"
    $request = [Net.WebRequest]::Create($serverUrl + $directory)
    $request.Credentials = $credentials
    $request.Method = [System.Net.WebRequestMethods+Ftp]::MakeDirectory
    $request.GetResponse() | Out-Null
}

function DownloadString($relative) 
{
    Write-Host "Download file $relative"
    $request = [Net.WebRequest]::Create($serverUrl + $relative)
    $request.Credentials = $credentials
    $request.Method = [System.Net.WebRequestMethods+Ftp]::DownloadFile
    $request.UseBinary = $true;

    $response = $request.GetResponse()
    $rs = $response.GetResponseStream()
    $reader = New-Object System.IO.StreamReader($rs)
    $result = $reader.ReadToEnd();
    $rs.Close()
    $rs.Dispose()
    $reader.Close()
    $reader.Dispose()

    return $result;
}

$files = GetLocalFiles '/' $artifactDirectory
$json = ConvertTo-Json $files

$lastrun = @{}
if(!$reset)
{
    Try 
    {
        $lastrunContent = DownloadString '/.lastrun'
        (ConvertFrom-Json $lastrunContent).psobject.properties | Foreach { $lastrun[$_.Name] = $_.Value }
    }
    Catch 
    {
        Write-Host 'No /.lastrun found'
    }
}

$toDelete = New-Object System.Collections.ArrayList
foreach($key in $lastrun.Keys)
{
    if(!$files.ContainsKey(($key)))
    {
        $toDelete.Add($key)  > $null
    }
}

$toUpload = New-Object System.Collections.ArrayList
foreach($key in $files.Keys)
{
    if(!$lastrun.ContainsKey($key) -or ($lastrun[$key] -ne $files[$key]))
    {
        $toUpload.Add($key) > $null
    }
}

Write-Host "Delete $($toDelete.Count)"
Write-Host "Upload $($toUpload.Count)"

foreach($relative in $toDelete)
{
    DeleteFile $relative
}

foreach($relative in $toUpload)
{
    UploadFile $relative
}

if(($toDelete.Count + $toUpload.Count) -gt 0)
{
    $enc = [system.Text.Encoding]::UTF8
    $jsonBytes = $enc.GetBytes($json) 
    UploadContent '/.lastrun' $jsonBytes
}
