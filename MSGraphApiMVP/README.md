# MS Graph Api MVP

## Requirements

- File appsettings.json in the root directory
```json
{
  "AzureAd": {
    "Instance": "https://login.microsoftonline.com/",
    "Domain": "<domain url>",
    "TenantId": "<tenant id>",
    "ClientId": "<client id>",
    "ClientCredentials": [
      // Some kind of credential, for example:
      {
        "SourceType": "ClientSecret",
        "ClientSecret": "<client secret>"
      }
    ]
  },
}
```
