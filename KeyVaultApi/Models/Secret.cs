using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Winterdom.KeyVaultApi.Models {
  public class Secret {
    public String Id { get; set; }
    public String Name { get; set; }
    public String Version { get; set; }
    public String ContentType { get; set; }
  }

  public class SecretWithValue : Secret {
    public String Value { get; set; }
  }
}
