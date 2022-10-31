using System;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Dtos;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services.Bases;

public abstract class ReservationServiceBase : IOrderReservationService
{
    protected IConfiguration _configuration;
    protected IAppLogger<ReservationWithCosmosDbService> _logger;
    protected IAzureCommunicatorService _communicator;
    protected IOrderRequestContentAssembler _assembler;
    protected ISecretBrokerService _secretBrokerService;

    protected ReservationServiceBase
    (
        IConfiguration configuration,
        IAppLogger<ReservationWithCosmosDbService> logger,
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
