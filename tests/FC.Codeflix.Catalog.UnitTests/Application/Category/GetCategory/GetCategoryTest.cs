using System;
using System.Threading;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Application.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using UseCase = FC.Codeflix.Catalog.Application.UseCases.Category.GetCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.GetCategory;

[Collection(nameof(GetCategoryTestFixture))]
public class GetCategoryTest
{
    private readonly GetCategoryTestFixture _fixture;

    public GetCategoryTest(GetCategoryTestFixture fixture)
        => _fixture = fixture;

    [Fact(DisplayName = nameof(GetCategory))]
    [Trait("Application ", "GetCategory - Use Cases")]
    public async Task GetCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var exapleCategory = _fixture.GetExampleCategory();
        // Quando o repositório receber um Get com um Guid e um CancellationToken ele deve retornar a exampleCategory
        repositoryMock.Setup(x => 
            x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(exapleCategory);
        var input = new UseCase.GetCategoryInput(exapleCategory.Id);
        var useCase = new UseCase.GetCategory(repositoryMock.Object);

        var output = await useCase.Handle(input, CancellationToken.None);
        
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);

        output.Should().NotBeNull();
        output.Name.Should().Be(exapleCategory.Name);
        output.Description.Should().Be(exapleCategory.Description);
        output.IsActive.Should().Be(exapleCategory.IsActive);
        output.Id.Should().Be(exapleCategory.Id);
        output.CreatedAt.Should().Be(exapleCategory.CreatedAt);
    } 
    
    [Fact(DisplayName = nameof(NotFoundExceptionWhenCategoryDoesNotExist))]
    [Trait("Application ", "GetCategory - Use Cases")]
    public async Task NotFoundExceptionWhenCategoryDoesNotExist()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var exapleGuid = Guid.NewGuid();
        // Quando o repositório receber um Get com um Guid e um CancellationToken ele deve retornar a exampleCategory
        repositoryMock.Setup(x => 
            x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ThrowsAsync(
                new NotFoundException($"Category '{exapleGuid}' not found")
            );
        
        var input = new UseCase.GetCategoryInput(exapleGuid);
        var useCase = new UseCase.GetCategory(repositoryMock.Object);

        var task= async () => await useCase.Handle(input, CancellationToken.None);
        
        await task.Should().ThrowAsync<NotFoundException>();
        
        repositoryMock.Verify(x => x.Get(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
    } 
}