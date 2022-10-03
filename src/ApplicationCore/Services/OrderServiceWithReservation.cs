using System.Threading.Tasks;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

internal class OrderServiceWithReservation : OrderService
{
    IOrderReservationService _reservationService;

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
        //Order Parts
        var basket = await this.RetrieveBasketFromRepositoryById(basketId);
        var catalogItems = await this.RetrieveCatalogItemsByBasket(basket);
        var items = this.RetrieveItems(basket, catalogItems);


        //Order Assembly
        var order = await AssembleOrder(basket, shippingAddress, items);


        await _orderRepository.AddAsync(order);
        await _reservationService.ReserveItems(order);
    }
}
