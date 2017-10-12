using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;

namespace Winterdom.KeyVaultApi.Controllers {
  public abstract class KeyVaultController : Controller {
    protected IConfiguration Configuration { get; private set; }
    protected ILogger Logger { get; private set; }

    public KeyVaultController(IConfiguration configuration, ILogger logger) {
      this.Configuration = configuration;
      this.Logger = logger;
    }

    protected async Task<String> Authenticate(String tenantId, String resource, String scope) {
      Logger.LogTrace("Authenticate({0}, {1}, {2})", tenantId, resource, scope);
      String clientId = Configuration["Auth:ClientId"];
      String clientSecret = Configuration["Auth:ClientSecret"];

      var appCredentials = new ClientCredential(clientId, clientSecret);

      var assertion = await GetUserAssertion();

      var context = new AuthenticationContext(tenantId);
      var token = await context.AcquireTokenAsync(resource, appCredentials, assertion);
      if ( token != null ) {
        Logger.LogTrace("New token is: {0}", token.AccessToken);
      }
      return token.AccessToken;
    }

    protected async Task<UserAssertion> GetUserAssertion() {
      var auth = await this.HttpContext.AuthenticateAsync();
      var token = auth.Properties.GetTokenValue("access_token");
      Logger.LogTrace("User Assertion = {0}", token);
      return new UserAssertion(token);
    }

    protected String GetVaultUrl(String vaultName) {
      return $"https://{vaultName}.vault.azure.net/";
    }
  }
}
