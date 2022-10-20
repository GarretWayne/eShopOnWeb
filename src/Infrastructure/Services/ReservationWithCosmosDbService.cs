using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
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
        var requestUri = _configuration["OrderReserveToCosmosDbAzFunction"];
        await _orderReservationCommunicator.OnPostOrderAsync(content, requestUri);
    }
}
