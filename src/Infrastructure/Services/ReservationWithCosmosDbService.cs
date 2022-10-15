using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using BlazorAdmin.Services;
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
        return await _assembler.AssembleOrderContentAsync(new OrderDtoToReserve(order));
    }

    public async Task SendOrderAsync(string content)
    {
        var requestUri = _configuration.GetSection("AzureFunctions:AzFuncToCosmosDb").Key;
        await _orderReservationCommunicator.OnPostOrderAsync(content, requestUri);

    }

   
}
