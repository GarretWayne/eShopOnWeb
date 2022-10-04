using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

internal class OrderServiceWithReservation : OrderService
{
    private readonly IOrderReservationService _reservationService;

    public OrderServiceWithReservation(IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IRepository<Order> orderRepository,
        IUriComposer uriComposer,
        IOrderReservationService reservationService)
        : base(basketRepository, itemRepository, orderRepository, uriComposer)
    {
        this._reservationService = reservationService;
    }

    public override async Task CreateOrderAsync(int basketId, Address shippingAddress)
    {
        //Order Assembly
        var order = await AssembleOrder(basketId, shippingAddress);

        await AddOrderToRepo(order);
        await ReserveOrderWithService(order);
    }

    protected async Task ReserveOrderWithService(Order order)
    {
        await _reservationService.ReserveItems(order);
    }
}
