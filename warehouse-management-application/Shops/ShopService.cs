using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_management_core.DTO_s;
using warehouse_management_core.Exceptions;

namespace warehouse_management_application.Shops;

public class ShopService : IServise
{
    private readonly IRepository<Shop> _repository;

    public ShopService(IRepository<Shop> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<ShopDTO>> GetShopAsync(CancellationToken cancellationToken = default) =>
        (await _repository.Get(cancellationToken)).Select(x => (ShopDTO)x);

    public async Task<IEnumerable<ShopDTO>> GetShopsItemsAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        var potentialShop = (await _repository.GetWithoutTracking(x => x.Id.Value == shopId, cancellationToken)).FirstOrDefault()??
            throw new ShopNotFoundException(shopId);
        return (IEnumerable<ShopDTO>)potentialShop.Items.Cast<ItemDTO>();
    }

    public async Task CreateOrUpdateShopAsync(ShopDTO shop, CancellationToken cancellationToken = default)
    {
        Shop localShop;
        if (shop.Id is not null)
        {
            localShop = (await _repository.Get(x => x.Id.Value == shop.Id.Value, cancellationToken)).FirstOrDefault()??
                throw new ShopNotFoundException(shop.Id.Value);
            localShop.Name = shop.Name;
            localShop.Capacity = shop.Capacity;
        }
        else
            localShop = new()
            {
                Name = shop.Name,
                Capacity = shop.Capacity
            };

        if (localShop.Id is null)
            await _repository.Add(localShop, cancellationToken);
        else
            await _repository.Update(localShop, cancellationToken);
    }

    // Метод для получения всех товаров определенного магазина
    public async Task<IEnumerable<ItemDTO>> GetShopItemsAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        var potentialShop = (await _repository.GetWithoutTracking(x => x.Id.Value == shopId, cancellationToken)).FirstOrDefault()??
            throw new ShopNotFoundException(shopId);
        return potentialShop.Items.Cast<ItemDTO>();
    }

    // Метод для получения конкретного товара по его идентификатору
    public async Task<ItemDTO> GetItemByIdAsync(Guid itemId, CancellationToken cancellationToken = default)
    {
        var item = await _repository.GetById(itemId, cancellationToken);
        if (item is null)
            throw new ItemNotFoundException(itemId);
        return (ItemDTO)item;
    }
}