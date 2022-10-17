using System.Collections.Generic;
using System.Linq;
using FC.Codeflix.Catalog.Domain.SearchableRepository;
using FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

namespace FC.Codeflix.Catalog.IntegrationTests.Application.UseCases.Category.ListCategories;

public class ListCategoriesTestFixture : CategoryUseCasesBaseFixture
{
    // Para cada nome eu retorno uma categoria
    public List<DomainEntity.Category> GetExampleCategoriesListWithNames(List<string> names)
        => names.Select(name =>
        {
            var category = GetExampleCategory();
            category.Update(name);
            return category;
        }).ToList();
    
    public List<DomainEntity.Category> CloneCategoryListOrdered(List<DomainEntity.Category> categoriesList, string orderBy, SearchOrder order)
    {
        // Passando a lista no construtor dessa lista ele já faz o clone da lista que veio no
        // parametro para a minha listClone
        var listClone = new List<DomainEntity.Category>(categoriesList);
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

[CollectionDefinition(nameof(ListCategoriesTestFixture))]
public class ListCategoriesTestFixtureCollection : ICollectionFixture<ListCategoriesTestFixture> {}