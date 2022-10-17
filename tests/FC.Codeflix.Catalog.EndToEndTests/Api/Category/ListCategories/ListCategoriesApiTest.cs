using System.Net;
using FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;
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
    public async void ListCategoriesAndTotalDefault()
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
    public async void ItemsEmptyWhenWhenPersistenceIsEmpty()
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
    public async void ListCategoriesAndTotal()
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
    
    public void Dispose() => _fixture.CleanPersistence();
}