using System;
using System.Threading.Tasks;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public interface ISecretBrokerService
{
    Task<String> GetSecretAsStringByNameAsync(string secretName);
}
