using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;

namespace Winterdom.KeyVaultApi {
  public class Startup {
    public Startup(IHostingEnvironment env) {
      var builder = new ConfigurationBuilder()
          .SetBasePath(env.ContentRootPath)
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
          .AddEnvironmentVariables();
      Configuration = builder.Build();
    }

    public IConfigurationRoot Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services) {
      // Add framework services.
      services.AddMvc();
      services.AddSingleton<IConfiguration>(Configuration);
      services.AddAuthentication(o => {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
      }).AddJwtBearer(options => {
        options.Authority = Configuration["Auth:Authority"];
        options.Audience = Configuration["Auth:Audience"];
        options.SaveToken = true;
        // hack: don't validate issuers for now to support multi-tenant
        options.TokenValidationParameters.ValidateIssuer = false;
      });
      services.AddSwaggerGen(c => {
        c.SwaggerDoc("v1", new Info { Title = "KeyVaultController", Version = "v1" });
        c.AddSecurityDefinition("AAD", new OAuth2Scheme {
          AuthorizationUrl = "https://login.windows.net/common/oauth2/authorize",
          TokenUrl = "https://login.windows.net/common/oauth2/token",
          Flow = "accessCode",
          Scopes = new Dictionary<String, String>()
        });
        c.OperationFilter<SummaryFilter>();
        c.OperationFilter<DescriptionFilter>();
      });
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) {
      app.UseAuthentication();
      app.UseMvc();
      app.UseSwagger();
      app.UseSwaggerUI(c => {
        c.RoutePrefix = "/swagger/ui";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Key Vault Connector");
      });
    }
  }
}
