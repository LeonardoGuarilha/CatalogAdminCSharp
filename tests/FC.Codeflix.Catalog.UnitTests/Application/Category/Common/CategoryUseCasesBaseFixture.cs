using System;
using FC.Codeflix.Catalog.Application.Interfaces;
using FC.Codeflix.Catalog.Domain.Repository;
using FC.Codeflix.Catalog.UnitTests.Common;
using Moq;

namespace FC.Codeflix.Catalog.UnitTests.Application.Category.Common;

public abstract class CategoryUseCasesBaseFixture : BaseFixture
{
    public string GetValidCategoryName()
    {
        var categoryName = "";
        
        while (categoryName.Length < 3)
            categoryName = Faker.Commerce.Categories(1)[0];

        if (categoryName.Length > 255)
            categoryName = categoryName[.. 255]; // Pega os primeiros 255 caracteres

        return categoryName;
    }

    public string GetValidCategoryDescription()
    {
        var categoryDescription = Faker.Commerce.ProductDescription();
        
        if (categoryDescription.Length > 10000)
            categoryDescription = categoryDescription[.. 10000]; // Pega os primeiros 10000 caracteres

        return categoryDescription;
    }
    
    public bool GetRandomBoolean() => (new Random()).NextDouble() < 0.5;
    public Catalog.Domain.Entity.Category GetExampleCategory() => new(
        GetValidCategoryName(), 
        GetValidCategoryDescription(),
        GetRandomBoolean()
    );
    public Mock<ICategoryRepository> GetRepositoryMock() => new Mock<ICategoryRepository>();
    // Dessa forma também retorna uma instancia do IUnitOfWork
    public Mock<IUnitOfWork> GetUnitOfWorkMock() => new();
}