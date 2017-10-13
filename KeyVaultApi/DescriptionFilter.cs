using System;
using System.ComponentModel;
using System.Linq;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Winterdom.KeyVaultApi {
  public class DescriptionFilter : IOperationFilter {
    public void Apply(Operation operation, OperationFilterContext context) {
      var apiDesc = context.ApiDescription;
      var desc = apiDesc.ActionAttributes()
                        .OfType<DescriptionAttribute>()
                        .FirstOrDefault();
      if ( desc != null ) {
        operation.Description = desc.Description;
      }
    }
  }
}
