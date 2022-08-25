using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.DeleteCategory;

[Collection(nameof(DeleteCategoryApiTestFixture))]
public class DeleteCategoryApiTest : IDisposable
{
    private readonly DeleteCategoryApiTestFixture _fixture;

    public DeleteCategoryApiTest(DeleteCategoryApiTestFixture fixture)
        => _fixture = fixture;
    

    [Fact(DisplayName = nameof(DeleteCategory))]
    [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
    public async void DeleteCategory()
    {
        // arrange
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var exampleCategory = exampleCategoriesList[10];

        // act
        var (response, output) = await _fixture
            .ApiClient
            .Delete<object>($"/categories/{exampleCategory.Id}");

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status204NoContent);
        output.Should().BeNull();

        var persistenceCategory = await _fixture.Persistence.GetById(exampleCategory.Id);
        persistenceCategory.Should().BeNull();
    }
    
    [Fact(DisplayName = nameof(ErrorWhenCategoryNotFound))]
    [Trait("EndToEnd/API", "Category/Delete - Endpoints")]
    public async void ErrorWhenCategoryNotFound()
    {
        // arrange
        var exampleCategoriesList = _fixture.GetExampleCategoriesList(20);
        await _fixture.Persistence.InsertList(exampleCategoriesList);

        var randomGuid = Guid.NewGuid();

        // act
        var (response, output) = await _fixture
            .ApiClient
            .Delete<ProblemDetails>($"/categories/{randomGuid}");

        // assert
        response.Should().NotBeNull();
        response!.StatusCode.Should().Be((HttpStatusCode)StatusCodes.Status404NotFound);
        output.Should().NotBeNull();
        output!.Title.Should().Be("Not Found");
        output.Type.Should().Be("NotFound");
        output.Status.Should().Be((int)StatusCodes.Status404NotFound);
        output.Detail.Should().Be($"Category '{randomGuid}' not found.");
    }
    
    public void Dispose() => _fixture.CleanPersistence();
}