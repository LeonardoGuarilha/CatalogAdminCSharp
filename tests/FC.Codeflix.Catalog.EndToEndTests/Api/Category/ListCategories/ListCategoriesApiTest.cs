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
        output!.Items.Should().HaveCount(defaultPerPage);
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