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

# Create the Web Api application
$baseUrl = "https://${apiAppName}.azurewebsites.net/"
$appId = "https://${tenantDomain}/$applicationName"

$app = New-AzureRmADApplication -DisplayName "$applicationName Connector" `
                                -HomePage $baseUrl `
                                -IdentifierUris $appId `
                                -ReplyUrls $baseUrl `
                                -AvailableToOtherTenants $true

$applicationKey = New-Password
$credential = New-AzureRmADServicePrincipal -ApplicationId $app.ApplicationId -Password $applicationKey   

[PSCustomObject]@{
  Application = "WebApi Connector";
  ApplicationId = $app.ApplicationId;
  ApplicationKey = $applicationKey
}

# Create the client application that will be used by LogicApps
$clientAppId = "https://${tenantDomain}/${applicationName}-connector"
$clientKey = New-Password
$clientApp = New-AzureRmADApplication -DisplayName "$applicationName Connector Client" `
                                      -HomePage "https://login.windows.net" `
                                      -IdentifierUris $clientAppId `
                                      -ReplyUrls "https://msmanaged-na.consent.azure-apim.net/redirect"


$clientCredential = New-AzureRmADAppCredential -ObjectId $clientApp.ObjectId -Password $clientKey

[PSCustomObject]@{
  Application = "Client";
  ApplicationId = $clientApp.ApplicationId;
  ApplicationKey = $clientKey
}