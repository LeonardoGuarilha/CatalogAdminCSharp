using System;
using System.Threading;
using System.Threading.Tasks;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FluentAssertions;
using Moq;
using Xunit;
using UseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture createCategoryTestFixture) 
        => _fixture = createCategoryTestFixture;
    

    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object); // O Object é o mock
        var input = _fixture.GetInput();

        var output = await useCase.Handle(input, CancellationToken.None);
        
        // Verificar se o método Insert do repositório foi chamado passando um Category como parâmetro
        // e um cancelation token e se ele foi chamado somente uma vez.
        repositoryMock.Verify(
            repository => repository.Insert(It.IsAny<Catalog.Domain.Entity.Category>(), It.IsAny<CancellationToken>()), Times.Once);
        
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }
    
    [Fact(DisplayName = nameof(CreateCategoryWithOnlyName))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithOnlyName()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
        var input = new UseCases.CreateCategoryInput(_fixture.GetValidCategoryName());

        var output = await useCase.Handle(input, CancellationToken.None);
        
        repositoryMock.Verify(
            repository => repository.Insert(It.IsAny<Catalog.Domain.Entity.Category>(), It.IsAny<CancellationToken>()), Times.Once);
        
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }
    
    [Fact(DisplayName = nameof(CreateCategoryWithOnlyNameAndDescription))]
    [Trait("Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryWithOnlyNameAndDescription()
    {
        var repositoryMock = _fixture.GetRepositoryMock();
        var unitOfWorkMock = _fixture.GetUnitOfWorkMock();
        var useCase = new UseCases.CreateCategory(repositoryMock.Object, unitOfWorkMock.Object);
        var input = new UseCases.CreateCategoryInput(_fixture.GetValidCategoryName(), _fixture.GetValidCategoryDescription());

        var output = await useCase.Handle(input, CancellationToken.None);
        
        repositoryMock.Verify(
            repository => repository.Insert(It.IsAny<Catalog.Domain.Entity.Category>(), It.IsAny<CancellationToken>()), Times.Once);
        
        unitOfWorkMock.Verify(uow => uow.Commit(It.IsAny<CancellationToken>()), Times.Once);
        
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().BeTrue();
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }

    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [Trait("Application", "CreateCategory - Use Cases")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 24,
        MemberType = typeof(CreateCategoryTestDataGenerator)
        )]
    public async void ThrowWhenCantInstantiateCategory(UseCases.CreateCategoryInput input, string exceptionMessage)
    {
        var useCase = new UseCases.CreateCategory(_fixture.GetRepositoryMock().Object, _fixture.GetUnitOfWorkMock().Object);
        
        Func<Task> task = async() => await useCase.Handle(input, CancellationToken.None);
        
        await task
            .Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(exceptionMessage);
    }
}