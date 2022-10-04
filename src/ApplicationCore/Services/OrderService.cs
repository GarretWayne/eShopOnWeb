using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Microsoft.eShopWeb.ApplicationCore.Entities;
using Microsoft.eShopWeb.ApplicationCore.Entities.BasketAggregate;
using Microsoft.eShopWeb.ApplicationCore.Entities.OrderAggregate;
using Microsoft.eShopWeb.ApplicationCore.Interfaces;
using Microsoft.eShopWeb.ApplicationCore.Specifications;

namespace Microsoft.eShopWeb.ApplicationCore.Services;

public class OrderService : IOrderService
{
    protected readonly IRepository<Order> _orderRepository;
    private readonly IUriComposer _uriComposer;
    private readonly IRepository<Basket> _basketRepository;
    private readonly IRepository<CatalogItem> _itemRepository;

    public OrderService(IRepository<Basket> basketRepository,
        IRepository<CatalogItem> itemRepository,
        IRepository<Order> orderRepository,
        IUriComposer uriComposer)
    {
        _orderRepository = orderRepository;
        _uriComposer = uriComposer;
        _basketRepository = basketRepository;
        _itemRepository = itemRepository;
    }

    public virtual async Task CreateOrderAsync(int basketId, Address shippingAddress)
    {
        //Order Assembly
        var order = await AssembleOrder(basketId, shippingAddress);

        await AddOrderToRepo(order);
    }

    //Helper Methods For Base Functionality
    protected async Task AddOrderToRepo(Order order)
    {
        await _orderRepository.AddAsync(order);
    }

    protected async Task<Order> AssembleOrder(int basketId, Address shippingAddress)
    {
        var basket = await RetrieveBasketFromRepositoryById(basketId);
        var catalogItems = await RetrieveCatalogItemsByBasket(basket);
        var items = RetrieveItems(basket, catalogItems);

        var result = new Order(basket.BuyerId, shippingAddress, items);
        return result;
    }

    protected List<OrderItem> RetrieveItems(Basket basket, List<CatalogItem> catalogItems)
    {
        var resultItems = basket.Items.Select(basketItem =>
        {
            var catalogItem = catalogItems.First(c => c.Id == basketItem.CatalogItemId);
            var itemOrdered = new CatalogItemOrdered(catalogItem.Id, catalogItem.Name, _uriComposer.ComposePicUri(catalogItem.PictureUri));
            var orderItem = new OrderItem(itemOrdered, basketItem.UnitPrice, basketItem.Quantity);
            return orderItem;
        }).ToList();
        return resultItems;
    }

    protected async Task<Basket> RetrieveBasketFromRepositoryById(int basketId)
    {
        var basketSpec = new BasketWithItemsSpecification(basketId);
        var resultBasket = await _basketRepository.GetBySpecAsync(basketSpec);

        Guard.Against.NullBasket(basketId, resultBasket);
        Guard.Against.EmptyBasketOnCheckout(resultBasket.Items);

        return resultBasket;
    }

    protected async Task<List<CatalogItem>> RetrieveCatalogItemsByBasket(Basket basket)
    {
        var catalogItemsSpecification = new CatalogItemsSpecification(basket.Items.Select(item => item.CatalogItemId).ToArray());
        return await _itemRepository.ListAsync(catalogItemsSpecification);
    }
}
