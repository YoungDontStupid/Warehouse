﻿using warehouse_management_core;
using warehouse_management_core.DTO_s;
using warehouse_management_core.Entities;

namespace warehouse_management_application.Items;

public class ItemService(IRepository<Item> repository) : IServise
{
    private IRepository<Item> Repository { get; init; } = repository;

    public async Task<IEnumerable<ItemDTO>> GetItemsAsync(CancellationToken cancellationToken = default) =>
        (await Repository.Get(cancellationToken)).Select(x => (ItemDTO)x);

    public async Task AddItemAsync(string newItem, CancellationToken cancellationToken = default)
    {
        var potentialCopies = await Repository.GetWithoutTracking(x => x.Name.Equals(newItem, StringComparison.CurrentCultureIgnoreCase), cancellationToken);
        if (potentialCopies.Any())
            throw new SimilarItemTitleException(newItem);
    }

    public async Task CreateOrUpdateItemAsync(ItemDTO item, CancellationToken cancellationToken = default)
    {
        Item localitem;
        if (item.Id is not null)
        {
            localitem = (await Repository.Get(x => x.Id.Value == item.Id.Value, cancellationToken)).FirstOrDefault() ??
                throw new ItemNotFoundException(item.Id.Value);
            localitem.Name = item.Name;
            localitem.Description = item.Description;
            localitem.Price = item.Price;
            localitem.Temperature = item.Temperature;
            localitem.ExpirationDate = item.ExpirationDate;
        }
        else
            localitem = new()
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                Temperature = item.Temperature,
                ExpirationDate = item.ExpirationDate
            };

        if (localitem.Id is null)
            await Repository.Add(localitem, cancellationToken);
        else
            await Repository.Update(localitem, cancellationToken);
    }
}
