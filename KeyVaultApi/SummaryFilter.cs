using System;
using System.Linq;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;

namespace Winterdom.KeyVaultApi {

  [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter)]
  public class SummaryAttribute : Attribute {
    public String Summary { get; private set; }

    public SummaryAttribute(String summary) {
      this.Summary = summary;
    }
  }

  public class SummaryFilter : IOperationFilter {
    public void Apply(Operation operation, OperationFilterContext context) {
      var apiDesc = context.ApiDescription;
      var summary = apiDesc.ActionAttributes()
                           .OfType<SummaryAttribute>()
                           .FirstOrDefault();
      if ( summary != null ) {
        operation.Summary = summary.Summary;
      }
    }
  }
}
