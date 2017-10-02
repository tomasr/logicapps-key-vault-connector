using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;

namespace Winterdom.KeyVaultApi.Controllers {
  public abstract class KeyVaultController : Controller {
    protected IConfiguration Configuration { get; private set; }

    public KeyVaultController(IConfiguration configuration) {
      this.Configuration = configuration;
    }

    protected async Task<String> Authenticate(String tenantId, String resource, String scope) {
      String authority = $"https://sts.windows.net/{tenantId}";
      String clientId = Configuration["Auth:ClientId"];
      String clientSecret = Configuration["Auth:ClientSecret"];

      var appCredentials = new ClientCredential(clientId, clientSecret);

      var assertion = await GetUserAssertion();

      var context = new AuthenticationContext(authority);
      var token = await context.AcquireTokenAsync(resource, appCredentials, assertion);
      return token.AccessToken;
    }

    protected async Task<UserAssertion> GetUserAssertion() {
      var auth = await this.HttpContext.AuthenticateAsync();
      var token = auth.Properties.GetTokenValue("access_token");
      return new UserAssertion(token);
    }

    protected String GetVaultUrl(String vaultName) {
      return $"https://{vaultName}.vault.azure.net/";
    }
  }
}
