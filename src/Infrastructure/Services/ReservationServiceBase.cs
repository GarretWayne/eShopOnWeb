using System;
using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.Infrastructure.Dtos;
using Microsoft.Extensions.Configuration;

namespace Microsoft.eShopWeb.Infrastructure.Services;

public abstract class ReservationServiceBase : IOrderReservationService
{
    protected IConfiguration _configuration;
    protected IAppLogger<ReservationWithCosmosDbService> _logger;
    protected IAzureOrderReservationCommunicatorService _orderReservationCommunicator;
    protected IOrderContentAssembler _assembler;
    protected ISecretBroker _secretBroker;

    protected ReservationServiceBase
    (
        IConfiguration configuration,
        IAppLogger<ReservationWithCosmosDbService> logger,
        IAzureOrderReservationCommunicatorService orderReservationCommunicator,
        IOrderContentAssembler assembler,
        ISecretBroker secretBroker
        )
    {
        _configuration = configuration;
        _logger = logger;
        _orderReservationCommunicator = orderReservationCommunicator;
        _assembler = assembler;
        _secretBroker = secretBroker;
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
        return await _secretBroker.GetSecretAsStringByNameAsync(_configuration[appSettingsSecretName]);
    }
}
