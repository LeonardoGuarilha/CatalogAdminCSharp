using System.Net;
using FC.Codeflix.Catalog.Application.UseCases.Category.Common;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.GetCategory;

[Collection(nameof(GetCategoryApiTestFixture))]
public class GetCategoryApiaTest
{
    private readonly GetCategoryApiTestFixture _fixture;

    public GetCategoryApiaTest(GetCategoryApiTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("EndToEnd/API", "Category/Get - Endpoints")]
    public async Task GetCategory()
    {
        // arrange
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var examepleCategory = exampleCategoriesList[10];

        // act
        var (response, output) = await _fixture
            .ApiClient
            .Get<CategoryModelOutput>($"/categories/{examepleCategory.Id}");

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status200OK);
        output.Should().NotBeNull();
        output!.Id.Should().Be(examepleCategory.Id);
        output.Name.Should().Be(examepleCategory.Name);
        output.Description.Should().Be(examepleCategory.Description);
        output.IsActive.Should().Be(examepleCategory.IsActive);
        output.CreatedAt.Should().Be(examepleCategory.CreatedAt);
        

    }
}