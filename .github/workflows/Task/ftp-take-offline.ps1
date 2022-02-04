$server = $env:SERVER
$remoteDirectory = $env:REMOTE_DIRECTORY
$user = $env:USER
$password = $env:PASSWORD

$libDirectory = $env:LIB_DIRECTORY
$artifactDirectory = $env:ARTIFACT_DIRECTORY

Write-Host "Server: ${server}"
Write-Host "Remote directory: ${remoteDirectory}"
Write-Host "User: ${user}"
Write-Host "Password: ${password}"
Write-Host "Lib directory: ${libDirectory}"
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

if([string]::IsNullOrEmpty($libDirectory))
{
    Throw "Require LIB_DIRECTORY env variable."
}

Write-Host "Take app offline"
$remoteDirectory = $remoteDirectory.ToLower()
$credentials = New-Object System.Net.NetworkCredential($user, $password)

function UploadFile($url, $content)
{
    Write-Host "Upload file $url"
    $request = [Net.WebRequest]::Create($url)
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

if([string]::IsNullOrEmpty($artifactDirectory))
{
    Throw "Require ARTIFACT_DIRECTORY env variable."
}

$fileName = "app_offline.htm"
$content = $false

$customFilePath = "${artifactDirectory}/_${fileName}"
$defaultFilePath = "${libDirectory}/${fileName}"

Write-Host "Custom: ${customFilePath}"
Write-Host "Default: ${defaultFilePath}"

if([System.IO.File]::Exists($customFilePath))aa
{
    Write-Host "Using custom ${fileName}"
    $content = [System.IO.File]::ReadAllBytes($customFilePath)
}
elseif([System.IO.File]::Exists($defaultFilePath))
{
    Write-Host "Using default ${fileName}"
    $content = [System.IO.File]::ReadAllBytes($defaultFilePath)
}

if($content)
{
    $url = "ftp://${server}/${remoteDirectory}/${fileName}";

    UploadFile $url $content

    Write-Host "Wait for app shutdown..."
    Start-Sleep -s 10
    Write-Host "Finished."
}
