using FC.Codeflix.Catalog.EndToEndTests.Base;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;

public class CategoryBaseFixture : BaseFixture
{

    public CategoryPersistence Persistence;

    public CategoryBaseFixture() : base()
    {
        Persistence = new CategoryPersistence(CreateDbContext());
    }
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

    public string GetInvalidNameToShort()
        => Faker.Commerce.ProductName().Substring(0, 2);

    public string GetInvalidDescriptionTooLong()
    {
        var tooLongDescriptionForCategory = Faker.Commerce.ProductDescription();

        while (tooLongDescriptionForCategory.Length <= 10000)
            tooLongDescriptionForCategory = $"{tooLongDescriptionForCategory} {Faker.Commerce.ProductDescription()}";

        return tooLongDescriptionForCategory;
    }

    public string GetInvalidNameTooLong()
    {
        var tooLongNameForCategory = Faker.Commerce.ProductName();

        while (tooLongNameForCategory.Length <= 255)
            tooLongNameForCategory = $"{tooLongNameForCategory} {Faker.Commerce.ProductName()}";

        return tooLongNameForCategory;
    }

    public DomainEntity.Category GetExampleCategory()
        => new(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean());

    // Vai gerar uma lista de 15 elementos ou a quantidade de elementos que quem chamar esse método passar
    public List<DomainEntity.Category> GetExampleCategoriesList(int listLength = 15)
        => Enumerable.Range(1, listLength).Select(_ =>
            new DomainEntity.Category(GetValidCategoryName(), GetValidCategoryDescription(), GetRandomBoolean())).ToList();
}