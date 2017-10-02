[CmdletBinding()]
param(
  [Parameter(Mandatory=$true)]
  [String]$applicationName,
  [Parameter(Mandatory=$true)]
  [String]$apiAppName,
  [Parameter(Mandatory=$true)]
  [String]$tenantDomain
)

Import-Module AzureRm

function New-Password {
  $rng = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
  [byte[]]$buffer = new-Object byte[] 32
  $rng.GetBytes($buffer, 0, $buffer.Length)

  return [Convert]::ToBase64String($buffer)
}

$baseUrl = "https://${apiAppName}.azurewebsites.net/"
$appId = "https://${tenantDomain}/$applicationName"

$app = New-AzureRmADApplication -DisplayName $applicationName `
                                -HomePage $baseUrl `
                                -IdentifierUris $appId `
                                -ReplyUrls $baseUrl `
                                -AvailableToOtherTenants $true

#$sp = New-AzureRmADServicePrincipal -ApplicationId $app.ApplicationId

$applicationKey = New-Password

$credential = New-AzureRmADAppCredential -ObjectId $app.ObjectId -Password $applicationKey   

[PSCustomObject]@{
  ObjectId = $app.ObjectId;
  ApplicationId = $app.ApplicationId;
  ApplicationKey = $applicationKey
}