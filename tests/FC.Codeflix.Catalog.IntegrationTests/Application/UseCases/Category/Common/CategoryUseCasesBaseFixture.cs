using System;
using System.Collections.Generic;
using System.Linq;
using FC.Codeflix.Catalog.IntegrationTests.Base;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;

public class CategoryUseCasesBaseFixture : BaseFixture
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
    public Domain.Entity.Category GetExampleCategory() => new(
        GetValidCategoryName(), 
        GetValidCategoryDescription(),
        GetRandomBoolean()
    );

    // O Enumerable.Range vai gerar uma sequencia de números de 1 até 10 nesse caso, mas posso passar mais na chamada desse método,
    // depois eu faço a iteração com o Select, o "_" é para eu ignorar o número, seria tipo o "i" do for.
    public List<Domain.Entity.Category> GetExampleCategoriesList(int lenght = 10)
        => Enumerable.Range(1, lenght).Select(_ => GetExampleCategory()).ToList();
}