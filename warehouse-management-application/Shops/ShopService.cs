using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using warehouse_management_core.DTO_s;

namespace warehouse_management_application.Shops;

public class ShopService(IRepository<Shop> repository) : IServise
{
    private IRepository<Shop> Repository { get; init; } = repository;

    public async Task<IEnumerable<ShopDTO>> GetShopAsync(CancellationToken cancellationToken = default) =>
        (await Repository.Get(cancellationToken)).Select(x => (ShopDTO)x);

    public async Task<IEnumerable<ShopDTO>> GetShopsItemsAsync(Guid shopId, CancellationToken cancellationToken = default)
    {
        var potentialShop = (await Repository.GetWithoutTracking(x => x.Id.Value == shopId, cancellationToken)).FirstOrDefault() ??
            throw new ShopNotFoundException(shopId);
        return (IEnumerable<ShopDTO>)potentialShop.Items.Cast<ItemDTO>();
    }

    public async Task CreateOrUpdateShopAsync(ShopDTO shop, CancellationToken cancellationToken = default)
    {
        Shop localShop;
        if (shop.Id is not null)
        {
            localShop = (await Repository.Get(x => x.Id.Value == shop.Id.Value, cancellationToken)).FirstOrDefault() ??
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
            await Repository.Add(localShop, cancellationToken);
        else
            await Repository.Update(localShop, cancellationToken);
    }
}
