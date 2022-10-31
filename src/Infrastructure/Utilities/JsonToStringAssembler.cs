using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.eShopWeb.Infrastructure.Utilities;

public class JsonToStringAssembler : IOrderContentAssembler
{
    private readonly IAppLogger<JsonToStringAssembler> _logger;

    public JsonToStringAssembler(IAppLogger<JsonToStringAssembler> logger)
    {
        _logger = logger;
    }

    public Task<string> AssembleOrderContentAsync(object o)
    {
        var orderContent = JObject.FromObject(o);
        return Task.FromResult(orderContent.ToString(formatting: Formatting.None));

    }
}
