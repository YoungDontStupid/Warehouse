using warehouse_management_core;
using warehouse_management_core.DTO_s;
using warehouse_management_core.Entities;

namespace warehouse_management_application.Items;

public class ItemService(IRepository<Item> repository) : IServise
{
    private IRepository<Item> Repository { get; init; } = repository;

    public async Task<IEnumerable<ItemDTO>> GetItemsAsync(CancellationToken cancellationToken = default) =>
        (await Repository.Get(cancellationToken)).Select(x => (ItemDTO)x);

    public async Task RegisterOrUpdateItemsAsync(IEnumerable<ItemDTO> items, CancellationToken cancellationToken = default)
    {
        var newItems = from item in items
                         where item.Id is null
                         select new Item()
                         {
                             Description = item.Description,
                             Name = item.Name,
                             ExpirationDate = item.ExpirationDate,
                             Temperature = item.Temperature,
                             Price = item.Price
                         };

        var repo = await Repository.Get(cancellationToken);
        var newOldItemCouples = from item in (from item in items where item.Id is not null select item)
                                  join repDevice in repo on item.Id equals repDevice.Id
                                  select (item, repDevice);

        foreach (var (updatedItem, oldItem) in newOldItemCouples)
        {
            oldItem.Name = updatedItem.Name;
            oldItem.ExpirationDate = updatedItem.ExpirationDate;
            oldItem.Temperature = updatedItem.Temperature;
            oldItem.Price = updatedItem.Price;
            
        }

        // todo: validation object
        await Repository.AddRange(newItems, cancellationToken);
        await Repository.UpdateRange(repo, cancellationToken);
    }
}
