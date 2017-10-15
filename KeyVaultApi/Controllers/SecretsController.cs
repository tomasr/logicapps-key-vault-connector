using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Winterdom.KeyVaultApi.Models;

namespace Winterdom.KeyVaultApi.Controllers {
  [Route("api/{vaultName}/[controller]")]
  [Authorize]
  public class SecretsController : KeyVaultController {

    public SecretsController(IConfiguration configuration, ILogger<SecretsController> logger)
      : base(configuration, logger) {
    }

    [HttpGet()]
    [SwaggerOperation(OperationId = "List Secrets")]
    [Summary("Lists all secrets")]
    [Description("Lists the secrets stored in Key Vault")]
    public  async Task<IEnumerable<Secret>> Get(String vaultName) {
      Logger.LogTrace("GetSecrets({0})", vaultName);
      var client = new KeyVaultClient(this.Authenticate);
      var results = new List<Secret>();
      var secrets = await client.GetSecretsAsync(GetVaultUrl(vaultName));
      do {
        foreach ( var s in secrets ) {
          results.Add(new Secret {
            Id = s.Id,
            Name = s.Identifier.Name,
            ContentType = s.ContentType
          });
        }

        secrets = await client.GetSecretsNextAsync(secrets.NextPageLink);
      } while ( !String.IsNullOrEmpty(secrets.NextPageLink) );
      return results;
    }

    [HttpGet("{name}")]
    [SwaggerOperation(OperationId = "Get Secret")]
    [SwaggerResponse(200, typeof(SecretValue))]
    [SwaggerResponse(404, description: "Secret not found")]
    [Summary("Get the value of a secret")]
    [Description("Gets the value of a secret stored in Key Vault")]
    public async Task<IActionResult> Get(String vaultName, String name, [FromQuery]String version = "") {
      var client = new KeyVaultClient(this.Authenticate);
      var secret = await client.GetSecretAsync(GetVaultUrl(vaultName), name, version);
      if ( secret != null ) {
        Logger.LogTrace("Found secret: {0}", secret.Id);
        return Ok(new SecretValue {
          Id = secret.Id,
          Name = secret.SecretIdentifier.Name,
          Version = secret.SecretIdentifier.Version,
          ContentType = secret.ContentType,
          Value = secret.Value
        });
      }
      return NotFound();
    }
  }
}
