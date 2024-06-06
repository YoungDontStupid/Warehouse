using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using warehouse_management_application.Items;
using warehouse_management_core;
using warehouse_management_core.DTO_s;
using warehouse_management_core.Entities;
using warehouse_management_core.Exceptions;

namespace warehouse_management_applicationtests.Items;

public class ItemServiceTest
{
    [TestCase("name1", "Desc", "2020-01-01",10,10.99)]
    [TestCase("name2", "Desc", "2021-01-01", 9, 0.99)]
    [TestCase("name3", "Desc", "2022-01-01", 11, 1.99)]
    [Test]
    public async Task CreateOrUpdateItem_WhenItemForUpdateDoesntExists_ThrowsException()
    {
        // Arrange
        var repo = new FakeRepository<Item>();
        var item = new Item
        {
            Id = new Id(Guid.NewGuid()),
            Name = "Test"
        };
        await repo.AddRange([item]);

        var service = new ItemService(repo);
        var toChange = new Item { Id = new Id(Guid.NewGuid()), Name = "NewName" };

        // Act
        AsyncTestDelegate act = async delegate { await service.CreateOrUpdateItemAsync(toChange); };

        // Assert
        Assert.ThrowsAsync<ItemNotFoundException>(act);
    }
    [TestCase("name1", "Desc", "2020-01-01", 10, 10.99)]
    [TestCase("name2", "Desc", "2021-01-01", 9, 0.99)]
    [TestCase("name3", "Desc", "2022-01-01", 11, 1.99)]
    public void AddItem_WhenSameName_ShouldThrowException(string name1, string name2)
    {
        // Arrange
        var itemRepo = new FakeRepository<Item>();
        itemRepo.AddRange([new Item() { Id = new Id(Guid.NewGuid()), Name = name1 }]);
        var itemService = new ItemService(itemRepo);

        // Act
        AsyncTestDelegate act = async delegate { await itemService.AddItemAsync(name2); };

        // Assert
        Assert.ThrowsAsync<SimilarItemTitleException>(act);
    }
    public async Task CreateOrUpdateItem_WhenItemForUpdateExists_ItemIsUpdated(
       string oldName, double oldBudget, DateTime oldDeadline,
       string newName, double newBudget, DateTime newDeadline)
    {
        // Arrange
        var repo = new FakeRepository<Item>();
        var item = new Item
        {
            Id = new Id(Guid.NewGuid()),
            Name = oldName,
            
        };
        await repo.AddRange([item]);

        var service = new ItemService(repo);
        var toChange = new Item { Id = item.Id, Name = newName,  };

        // Act
        await service.CreateOrUpdateItemAsync(toChange);

        // Assert
        var data = await repo.Get();
        var newItem = data.First();

        Assert.That(data.Count() == 1 &&
                    newItem.Id == item.Id &&
                    newItem.Name == newName &&
                   Is.True);
    }

}
