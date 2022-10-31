using System;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Dtos;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public class ReservationWithCosmosDbService : IOrderReservationService
{
    private readonly IConfiguration _configuration;
    private readonly IAppLogger<ReservationWithCosmosDbService> _logger;
    private readonly IAzureOrderReservationCommunicatorService _orderReservationCommunicator;
    private readonly IOrderContentAssembler _assembler;

    public ReservationWithCosmosDbService(IConfiguration configuration, IAppLogger<ReservationWithCosmosDbService> logger, IAzureOrderReservationCommunicatorService orderReservationCommunicator, IOrderContentAssembler assembler)
    {
        _configuration = configuration;
        _logger = logger;
        _orderReservationCommunicator = orderReservationCommunicator;
        _assembler = assembler;
    }

    public async Task ReserveItems(Order order)
    {
        _logger.LogInformation("START - Order reservation to be sent");

        var orderContent = await AssembleOrderContent(order);
        await SendOrderAsync(orderContent);

        _logger.LogInformation("END - Order has been sent");
    }

    private async Task<string> AssembleOrderContent(Order order)
    {
        var dtoToJsonify = new OrderDtoToReserve(order);
        return await _assembler.AssembleOrderContentAsync(dtoToJsonify);
    }

    public async Task SendOrderAsync(string content)
    {
        _logger.LogInformation("Start - KeyVault");
        string keyVaultName = "keyvaulttestazuretest";
        var keyVaultUri = "https://" + keyVaultName + ".vault.azure.net";

        var secretClient = new SecretClient(new Uri(keyVaultUri),
            new DefaultAzureCredential());
        var secretUriResponse = await secretClient.GetSecretAsync("ConnectionStrings--AzureFunc--OrderReserve");

        _logger.LogInformation("DONE - KeyVault");



        string requestUri = secretUriResponse.Value.Value;
        await _orderReservationCommunicator.OnSendOrderAsync(content, requestUri);
    }
}
