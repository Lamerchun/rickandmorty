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
$url = "ftp://${server}/${remoteDirectory}/";

Write-Host "Start clean ${user}:${password}@${url}"
$credentials = New-Object System.Net.NetworkCredential($user, $password)
DeleteFtpFolder $url $true

function DeleteFtpFolder($url, $isRoot)
{
    $listRequest = [Net.WebRequest]::Create($url)
    $listRequest.Method = [System.Net.WebRequestMethods+Ftp]::ListDirectoryDetails
    $listRequest.Credentials = $credentials

    $lines = New-Object System.Collections.ArrayList

    $listResponse = $listRequest.GetResponse()
    $listStream = $listResponse.GetResponseStream()
    $listReader = New-Object System.IO.StreamReader($listStream)

    while (!$listReader.EndOfStream)
    {
        $line = $listReader.ReadLine()
        $lines.Add($line) | Out-Null
    }

    $listReader.Dispose()
    $listStream.Dispose()
    $listResponse.Dispose()

    foreach ($line in $lines)
    {
        $tokens = $line.Split(" ", 9, [StringSplitOptions]::RemoveEmptyEntries)
        $name = $tokens[8]
        $permissions = $tokens[0]

        $fileUrl = ($url + $name)

        if ($permissions[0] -eq 'd')
        {
            if($isRoot -and ($name -eq "App_Data")) 
            {
                Write-Host "Skip deleting folder $fileUrl"
                continue;
            }
            
            if($isRoot -and ($name -eq "Cache")) 
            {
                Write-Host "Skip deleting folder $fileUrl"
                continue;
            }
            
            DeleteFtpFolder ($fileUrl + "/") $false
        }
        else
        {
            if($isRoot -and ($name -eq "app_offline.htm")) 
            {
                Write-Host "Skip deleting file $name"
                continue;
            }

            if($isRoot -and ($name -eq "web.config")) 
            {
                Write-Host "Skip deleting file $name"
                continue;
            }

            Write-Host "Deleting file $name"
            $deleteRequest = [Net.WebRequest]::Create($fileUrl)
            $deleteRequest.Credentials = $credentials
            $deleteRequest.Method = [System.Net.WebRequestMethods+Ftp]::DeleteFile
            $deleteRequest.GetResponse() | Out-Null
        }
    }

    if(!$isRoot)
    {
        Write-Host "Deleting folder $url"
        $deleteRequest = [Net.WebRequest]::Create($url)
        $deleteRequest.Credentials = $credentials
        $deleteRequest.Method = [System.Net.WebRequestMethods+Ftp]::RemoveDirectory
        $deleteRequest.GetResponse() | Out-Null
    }
    else
    {
        Write-Host "Skip deleting folder $url"
    }
}
