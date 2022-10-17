﻿using System.Net;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
using FC.Codeflix.Catalog.Domain.SearchableRepository;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

[Collection(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTest : IDisposable
{
    private readonly ListCategoriesApiTestFixture _fixture;

    public ListCategoriesApiTest(ListCategoriesApiTestFixture fixture) 
        => _fixture = fixture;

    [Fact(DisplayName = nameof(ListCategoriesAndTotalDefault))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotalDefault()
    {
        // arrange
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        const int defaultPerPage = 15;
        
        // act
        var (response, output) = await _fixture
            .ApiClient.Get<ListCategoriesOutput>("/categories");

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(defaultPerPage);
        output.Page.Should().Be(1);
        output.PerPage.Should().Be(defaultPerPage);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
    
    [Fact(DisplayName = nameof(ItemsEmptyWhenWhenPersistenceIsEmpty))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ItemsEmptyWhenWhenPersistenceIsEmpty()
    {
        // act
        var (response, output) = await _fixture
            .ApiClient.Get<ListCategoriesOutput>("/categories");

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(0);
        output!.Items.Should().HaveCount(0);
    }
    
    [Fact(DisplayName = nameof(ListCategoriesAndTotal))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    public async Task ListCategoriesAndTotal()
    {
        // arrange
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page: 1, perPage: 5);
        
        // act
        var (response, output) = await _fixture
            .ApiClient.Get<ListCategoriesOutput>("/categories", input);

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(input.PerPage);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
    
    [Theory(DisplayName = nameof(ListPaginated))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData(10, 1, 5, 5)]
    [InlineData(10, 2, 5, 5)]
    [InlineData(7, 2, 5, 2)]
    [InlineData(7, 3, 5, 0)]
    public async Task ListPaginated(
        int quantityCategoriesToGenerate,
        int page,
        int perPage,
        int expectedQuantityItems
    )
    {
        // arrange
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(quantityCategoriesToGenerate);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page, perPage);
        
        // act
        var (response, output) = await _fixture
            .ApiClient.Get<ListCategoriesOutput>("/categories", input);

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(expectedQuantityItems);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
    
    [Theory(DisplayName = nameof(SearchByText))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData("Action", 1, 5, 1, 1)]
    [InlineData("Horror", 1, 5, 3, 3)]
    [InlineData("Horror", 2, 5, 0, 3)]
    [InlineData("Sci-fi", 1, 5, 4, 4)]
    [InlineData("Sci-fi", 1, 2, 2, 4)]
    [InlineData("Sci-fi", 2, 3, 1, 4)]
    [InlineData("Sci-fi Other", 1, 3, 0, 0)]
    [InlineData("Robots", 1, 5, 2, 2)]
    public async Task SearchByText(
        string search,
        int page,
        int perPage,
        int expectedQuantityItemsReturned,
        int expectedQuantityTotalItems
    )
    {
        // arrange
        var categoryNamesList = new List<string>()
        {
            "Action",
            "Horror",
            "Horror - Robots",
            "Horror - Based on Real Facts",
            "Drama",
            "Sci-fi - IA",
            "Sci-fi - Space",
            "Sci-fi - Robots",
            "Sci-fi - Future"
        };
        
        var exampleCategoriesList = _fixture.GetExampleCategoriesListWithNames(categoryNamesList);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var input = new ListCategoriesInput(page, perPage, search);
        
        // act
        var (response, output) = await _fixture
            .ApiClient.Get<ListCategoriesOutput>("/categories", input);

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(expectedQuantityTotalItems);
        output.Items.Should().HaveCount(expectedQuantityItemsReturned);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        foreach (var outputItem in output.Items)
        {
            var exampleItem = exampleCategoriesList.FirstOrDefault(x => x.Id == outputItem.Id);

            exampleItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);
        }
    }
    
    [Theory(DisplayName = nameof(ListOrdered))]
    [Trait("EndToEnd/API", "Category/List - Endpoints")]
    [InlineData("name", "asc")]
    [InlineData("name", "desc")]
    [InlineData("id", "asc")]
    [InlineData("id", "desc")]
    [InlineData("createdAt", "asc")]
    [InlineData("createdAt", "desc")]
    [InlineData("", "asc")]
    public async Task ListOrdered(
        string orderBy,
        string order
    )
    {
        // arrange
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(10);
        await _fixture.Persistence.InsertList(exampleCategoriesList);
        var inputOrder = order == "asc" ? SearchOrder.Asc : SearchOrder.Desc;
        var input = new ListCategoriesInput(page: 1, perPage: 20, sort: orderBy, dir: inputOrder);
        
        // act
        var (response, output) = await _fixture
            .ApiClient.Get<ListCategoriesOutput>("/categories", input);

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Total.Should().Be(exampleCategoriesList.Count);
        output.Items.Should().HaveCount(exampleCategoriesList.Count);
        output.Page.Should().Be(input.Page);
        output.PerPage.Should().Be(input.PerPage);
        
        var expectedOrderedList = _fixture.CloneCategoryListOrdered(exampleCategoriesList, input.Sort, input.Dir);
        
        for (var i = 0; i < expectedOrderedList.Count; i++)
        {
            var outputItem = output.Items[i];
            var exampleItem = expectedOrderedList[i];
            
            exampleItem.Should().NotBeNull();
            outputItem.Should().NotBeNull();
            outputItem.Name.Should().Be(exampleItem!.Name);
            outputItem.Id.Should().Be(exampleItem.Id);
            outputItem.Description.Should().Be(exampleItem.Description);
            outputItem.IsActive.Should().Be(exampleItem.IsActive);
            outputItem.CreatedAt.Should().Be(exampleItem.CreatedAt);   
        }
    }
    
    public void Dispose() => _fixture.CleanPersistence();
}