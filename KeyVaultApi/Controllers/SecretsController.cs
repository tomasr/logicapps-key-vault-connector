using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace Winterdom.KeyVaultApi.Controllers {
  [Route("api/{vaultName}/[controller]")]
  [Authorize]
  public class SecretsController : KeyVaultController {

    public SecretsController(IConfiguration configuration) : base(configuration) {
    }

    [HttpGet("{id}")]
    public async Task<String> Get(String vaultName, String id) {
      var client = new KeyVaultClient(this.Authenticate);
      var secret = await client.GetSecretAsync(GetVaultUrl(vaultName), id);
      return secret?.Value;
    }
  }
}
