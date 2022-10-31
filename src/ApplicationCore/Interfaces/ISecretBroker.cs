using System;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public interface ISecretBroker
{
    Task<String> GetSecretAsStringByNameAsync(string secretName);
}
