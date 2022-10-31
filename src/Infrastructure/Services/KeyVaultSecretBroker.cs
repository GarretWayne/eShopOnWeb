using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services;

internal class KeyVaultSecretBroker : ISecretBroker
{
    private readonly IConfiguration _configuration;
    private readonly IAppLogger<KeyVaultSecretBroker> _logger;
    private readonly SecretClient _secretClient;

    public KeyVaultSecretBroker(IConfiguration configuration, IAppLogger<KeyVaultSecretBroker> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _secretClient = SetupSecretClient(_configuration);
    }

    private SecretClient SetupSecretClient(IConfiguration configuration)
    {
        try
        {
            _logger.LogInformation("Start - KeyVaultClient");

            string keyVaultName = configuration["SecretNames:KeyVault"];
            var keyVaultUri = "https://" + keyVaultName + ".vault.azure.net";

            var secretClient = new SecretClient(new Uri(keyVaultUri),
                new DefaultAzureCredential());


            _logger.LogInformation("DONE - KeyVault Connected");
            return secretClient;
        }
        catch (Exception e)
        {
            _logger.LogCritical($"FAILED - KeyVault Connection Setup + {e}");
            throw;
        }
    }

    public async Task<string> GetSecretAsStringByNameAsync(string secretName)
    {
        var result = await _secretClient.GetSecretAsync(secretName);
        return result.Value.Value;
    }
}
