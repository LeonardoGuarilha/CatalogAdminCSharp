using System;
using System.Linq;
using System.Threading;
using FC.Codeflix.Catalog.Domain.Exceptions;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.Infra.Data.EF.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using ApplicationUseCases = FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.CreateCategory;

[Collection(nameof(CreateCategoryTestFixture))]
public class CreateCategoryTest
{
    private readonly CreateCategoryTestFixture _fixture;

    public CreateCategoryTest(CreateCategoryTestFixture fixture) 
        => _fixture = fixture;
    
    [Fact(DisplayName = nameof(CreateCategory))]
    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    public async void CreateCategory()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);
        var input = _fixture.GetInput();
        
        var output = await useCase.Handle(input, CancellationToken.None);
        
        // Testo o que está no banco
        // Pego um novo contexto para garantir que não tem nada em cache e é de fato o que está no banco
        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(input.IsActive);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        
        // Testo o que está no output
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(input.IsActive);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }
    
    [Fact(DisplayName = nameof(CreateCategoryOnlyWithName))]
    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryOnlyWithName()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);
        var input = new ApplicationUseCases.CreateCategoryInput(_fixture.GetInput().Name);
        
        var output = await useCase.Handle(input, CancellationToken.None);
        
        // Testo o que está no banco
        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be("");
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        
        // Testo o que está no output
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be("");
        output.IsActive.Should().Be(true);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }
    
    [Fact(DisplayName = nameof(CreateCategoryOnlyWithNameAndDescription))]
    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    public async void CreateCategoryOnlyWithNameAndDescription()
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);
        var exampleInput = _fixture.GetInput();
        var input = new ApplicationUseCases.CreateCategoryInput(exampleInput.Name, exampleInput.Description);
        
        var output = await useCase.Handle(input, CancellationToken.None);
        
        // Testo o que está no banco
        var dbCategory = await (_fixture.CreateDbContext(true)).Categories.FindAsync(output.Id);
        dbCategory.Should().NotBeNull();
        dbCategory!.Name.Should().Be(input.Name);
        dbCategory.Description.Should().Be(input.Description);
        dbCategory.IsActive.Should().Be(true);
        dbCategory.CreatedAt.Should().Be(output.CreatedAt);
        
        // Testo o que está no output
        output.Should().NotBeNull();
        output.Name.Should().Be(input.Name);
        output.Description.Should().Be(input.Description);
        output.IsActive.Should().Be(true);
        output.Id.Should().NotBeEmpty();
        output.CreatedAt.Should().NotBeSameDateAs(default(DateTime));
    }
    
    [Theory(DisplayName = nameof(ThrowWhenCantInstantiateCategory))]
    [Trait("Integration/Application", "CreateCategory - Use Cases")]
    [MemberData(
        nameof(CreateCategoryTestDataGenerator.GetInvalidInputs),
        parameters: 4,
        MemberType = typeof(CreateCategoryTestDataGenerator)
    )]
    public async void ThrowWhenCantInstantiateCategory(ApplicationUseCases.CreateCategoryInput input, 
        string expectedExceptionMessage)
    {
        var dbContext = _fixture.CreateDbContext();
        var repository = new CategoryRepository(dbContext);
        var unitOfWork = new UnitOfWork(dbContext);
        var useCase = new ApplicationUseCases.CreateCategory(repository, unitOfWork);

        var task = async() => await useCase.Handle(input, CancellationToken.None);

        await task
            .Should()
            .ThrowAsync<EntityValidationException>()
            .WithMessage(expectedExceptionMessage);

        var dbCategoriesList = _fixture.CreateDbContext(true).Categories.AsNoTracking().ToList();
        dbCategoriesList.Should().HaveCount(0);
    }
}