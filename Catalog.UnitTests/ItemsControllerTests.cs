using System;
using Catalog.Api.Repositories;
using Catalog.Api.Controllers;
using Catalog.Api.Entities;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Catalog.Api.Dtos;

namespace Catalog.UnitTests;

public class ItemsControllerTests
{
    private readonly Mock<IItemsRepository> repositoryStub = new();
    private readonly Mock<ILogger<ItemsController>> loggerStub = new();
    private readonly Random rand = new();


    [Fact]
    public async Task GetItemsAsync_WithUnexistingItem_ReturnsNotFound()
    {
        //  Arrange
        repositoryStub.Setup(repo => repo.GetItemAsync(It.IsAny<Guid>())).ReturnsAsync((Item)null);

        var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.GetItemAsync(Guid.NewGuid());
        // Assert
        // заменяем на версию от FluentAssertions
        //Assert.IsType<NotFoundResult>(result.Result);
        result.Result.Should().BeOfType<NotFoundResult>();

    }

    [Fact]
    public async Task GetItemsAsync_WithExistingItem_ReturnsExpectedItem()
    {
        // Arange
        var expectedItem = CreateRandomItem();
        repositoryStub.Setup(repositoryStub => repositoryStub.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(expectedItem);

        var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.GetItemAsync(Guid.NewGuid());
        // Assert
        /* вместо стого чтобы писать 100500 ассертов (что нехорошо)
          делаем dotnet add package FluentAssertions
        Assert.IsType<IHttpMessageHandlerFactory>(result.Value);
        var dto = (result as ActionResult<ItemDto>).Value;
        Assert.Equal(expectedItem.Id, dto.Id);
        Assert.Equal(expectedItem.Name, dto.Name);
        */
        result.Value.Should().BeEquivalentTo(expectedItem,
            options => options.ComparingByMembers<Item>());
    }

    [Fact]
    public async Task GetItemsAsync_WithExistingItem_ReturnsAllItems()
    {
        // Arrange
        var expectedItems = new[] { CreateRandomItem(), CreateRandomItem(), CreateRandomItem() };
        repositoryStub.Setup(repo => repo.GetItemsAsync())
            .ReturnsAsync(expectedItems);
        var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);
        // Act
        var actualItems = await controller.GetItemsAsync();
        // Assert
        actualItems.Should().BeEquivalentTo(
            expectedItems,
            options => options.ComparingByMembers<Item>()
        );
    }

    [Fact]
    public async Task CreateItemsAsync_WithItemToCreate_ReturnsCreatedItem()
    {
        // Arrange
        var itemToCreate = new CreateItemDto()
        {
            Name = Guid.NewGuid().ToString(),
            Price = rand.Next(1000)
        };

        var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.CreateItemAsync(itemToCreate);

        // Assert
        var createdItem = (result.Result as CreatedAtActionResult).Value as ItemDto;
        itemToCreate.Should().BeEquivalentTo(
            createdItem,
            options => options.ComparingByMembers<Item>().ExcludingMissingMembers() // У двух типов разный состав атрибутов. Сравниваем только те что общие
        );
        createdItem.Id.Should().NotBeEmpty();
        createdItem.CreatedDate.Should().BeCloseTo(DateTimeOffset.UtcNow, TimeSpan.FromMilliseconds(1000));
    }

    [Fact]
    public async Task UpdateItemsAsync_WithExistingItem_ReturnsNoContent()
    {
        // Arange
        Item existingItem = CreateRandomItem();
        repositoryStub.Setup(repositoryStub => repositoryStub.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingItem);

        var itemId = existingItem.Id;
        var itemToUpdate = new UpdateItemDto()
        {
            Name = Guid.NewGuid().ToString(),
            Price = existingItem.Price + 3
        };

        var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.UpdateItemAsync(itemId, itemToUpdate);

        // Assert
        result.Should().BeOfType<NoContentResult>();

    }

    [Fact]
    public async Task DeleteItemsAsync_WithExistingItem_ReturnsNoContent()
    {
        // Arange
        Item existingItem = CreateRandomItem();
        repositoryStub.Setup(repositoryStub => repositoryStub.GetItemAsync(It.IsAny<Guid>()))
            .ReturnsAsync(existingItem);


        var controller = new ItemsController(repositoryStub.Object, loggerStub.Object);

        // Act
        var result = await controller.DeleteItemAsync(existingItem.Id);

        // Assert
        result.Should().BeOfType<NoContentResult>();

    }



    private Item CreateRandomItem()
    {
        return new()
        {
            Id = Guid.NewGuid(),
            Name = Guid.NewGuid().ToString(),
            Price = rand.Next(1000),
            CreatedDate = DateTimeOffset.UtcNow
        };
    }

}