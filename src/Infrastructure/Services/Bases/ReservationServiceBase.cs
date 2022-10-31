using System;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Dtos;
using Microsoft.eShopWeb.Infrastructure.Services.Azure;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services.Bases;

public abstract class ReservationServiceBase : IOrderReservationService
{
    protected readonly IConfiguration _configuration;
    protected readonly IAppLogger<ReservationWithAzureFunctionToCosmosDb> _logger;
    protected readonly IAzureCommunicatorService _communicator;
    protected readonly IOrderRequestContentAssembler _assembler;
    protected readonly ISecretBrokerService _secretBrokerService;

    public string RequestTargetUri { get; set; }

    protected ReservationServiceBase
    (
        IConfiguration configuration,
        IAppLogger<ReservationWithAzureFunctionToCosmosDb> logger,
        IAzureCommunicatorService communicator,
        IOrderRequestContentAssembler assembler,
        ISecretBrokerService secretBrokerService
        )
    {
        _configuration = configuration;
        _logger = logger;
        _communicator = communicator;
        _assembler = assembler;
        _secretBrokerService = secretBrokerService;
        RequestTargetUri = String.Empty;
    }

    public async Task ReserveItems(Order order)
    {
        try
        {
            _logger.LogInformation("START - Order reservation to be sent");

            var orderContent = await AssembleRequestContent(order);
            await SendOrderAsync(orderContent);

            _logger.LogInformation("END - Order has been sent");
        }
        catch (Exception e)
        {
            _logger.LogCritical($"FAILED - Reservation Of Items failed -{e}");
            throw;
        }
    }

    protected async Task<string> AssembleRequestContent(Order order)
    {
        try
        {
            _logger.LogInformation("START - Request Content Building");

            var dtoToJsonify = new OrderDtoToReserve(order);

            _logger.LogInformation("PART - Dto -- Dto Assembled");

            var result = await _assembler.AssembleRequestContentAsync(dtoToJsonify);

            _logger.LogInformation("DONE - Request Content Building");
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError($"FAILED - Assembling the RequestContent -{e}");
            throw;
        }
    }

    protected abstract Task SendOrderAsync(string content);
    

    protected async Task<string> RetrieveRequestTargetUri(string appSettingsSecretName)
    {
        return await _secretBrokerService.GetSecretAsStringByNameAsync(_configuration[appSettingsSecretName]);
    }
}
