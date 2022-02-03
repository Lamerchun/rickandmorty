$server = $env:SERVER
$remoteDirectory = $env:REMOTE_DIRECTORY
$user = $env:USER
$password = $env:PASSWORD

Write-Host "Server: ${server}"
Write-Host "Remote directory: ${remoteDirectory}"
Write-Host "User: ${user}"
Write-Host "Password: ${password}"

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

$remoteDirectory = $remoteDirectory.ToLower()
$url = "ftp://${server}/${remoteDirectory}/app_offline.htm";
$credentials = New-Object System.Net.NetworkCredential($user, $password)

function DeleteFile($url)
{
    Write-Host "Deleting file $url"
    $deleteRequest = [Net.WebRequest]::Create($url)
    $deleteRequest.Credentials = $credentials
    $deleteRequest.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile
    $deleteRequest.GetResponse() | Out-Null
}

Write-Host "Take app online"
DeleteFile $url $true
