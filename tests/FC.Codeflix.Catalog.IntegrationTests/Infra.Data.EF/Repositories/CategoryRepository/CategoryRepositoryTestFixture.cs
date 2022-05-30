using System;
using System.Collections.Generic;
using System.Linq;
using FC.Codeflix.Catalog.Domain.Entity;
using FC.Codeflix.Catalog.Domain.SearchableRepository;
using FC.Codeflix.Catalog.Infra.Data.EF;
using FC.Codeflix.Catalog.IntegrationTests.Base;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Infra.Data.EF.Repositories.CategoryRepository;

public class CategoryRepositoryTestFixture : BaseFixture
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
    public Category GetExampleCategory() => new(
        GetValidCategoryName(), 
        GetValidCategoryDescription(),
        GetRandomBoolean()
    );

    // O Enumerable.Range vai gerar uma sequencia de números de 1 até 10 nesse caso, mas posso passar mais na chamada desse método,
    // depois eu faço a iteração com o Select, o "_" é para eu ignorar o número, seria tipo o "i" do for.
    public List<Category> GetExampleCategoriesList(int lenght = 10)
        => Enumerable.Range(1, lenght).Select(_ => GetExampleCategory()).ToList();

    // Para cada nome eu retorno uma categoria
    public List<Category> GetExampleCategoriesListWithNames(List<string> names)
        => names.Select(name =>
        {
            var category = GetExampleCategory();
            category.Update(name);
            return category;
        }).ToList();

    public List<Category> CloneCategoryListOrdered(List<Category> categoriesList, string orderBy, SearchOrder order)
    {
        // Passando a lista no construtor dessa lista ele já faz o clone da lista qeue veio no
        // parametro para a minha listClone
        var listClone = new List<Category>(categoriesList);
        // syntax nova do switch
        var orderedEnumerable = (orderBy.ToLower(), order) switch
        {
            // Se o orderBy for name e o order sendo SearchOrder.Asc 
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name),
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            // Por default ordena por nome
            _ => listClone.OrderBy(x => x.Name)
        };

        return orderedEnumerable.ToList();
    }
}

[CollectionDefinition(nameof(CategoryRepositoryTestFixture))]
public class CategoryRepositoryTestFixtureCollection : ICollectionFixture<CategoryRepositoryTestFixture>
{ }