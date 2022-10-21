using FC.Codeflix.Catalog.Api.ApiModels.Category;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.UpdateCategory;

public class UpdateCategoryApiTestFixture : CategoryBaseFixture
{
    public UpdateCategoryApiInput GetExampleInput()
        => new UpdateCategoryApiInput(
            GetValidCategoryName(), 
            GetValidCategoryDescription(),
            GetRandomBoolean()
        );
}

[CollectionDefinition(nameof(UpdateCategoryApiTestFixture))]
public class UpdateCategoryApiTestFixtureCollection : ICollectionFixture<UpdateCategoryApiTestFixture>
{
    
}