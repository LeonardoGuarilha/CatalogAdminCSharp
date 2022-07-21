using FC.Codeflix.Catalog.Application.UseCases.Category.CreateCategory;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.CreateCategory;

public class CreateCategoryApiTestFixture : CategoryBaseFixture
{
    public CreateCategoryInput GetExampleInput()
        => new (GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());
}

[CollectionDefinition(nameof(CreateCategoryApiTestFixture))]
public class CreateCategoryApiTestFixtureCollection : ICollectionFixture<CreateCategoryApiTestFixture>
{
    
}