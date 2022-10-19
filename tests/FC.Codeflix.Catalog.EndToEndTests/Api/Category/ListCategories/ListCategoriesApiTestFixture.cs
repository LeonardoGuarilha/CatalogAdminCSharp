using FC.Codeflix.Catalog.Domain.SearchableRepository;
using FC.Codeflix.Catalog.EndToEndTests.Api.Category.Common;
using DomainEntity = FC.Codeflix.Catalog.Domain.Entity;
using Xunit;

namespace FC.Codeflix.Catalog.EndToEndTests.Api.Category.ListCategories;

public class ListCategoriesApiTestFixture : CategoryBaseFixture
{
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
            // Vai ordenar por nome e por id, faz uma ordenação com um segundo nível
            ("name", SearchOrder.Asc) => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id),
            // Vai ordenar por nome e por id decrescente, faz uma ordenação com um segundo nível
            ("name", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Name).ThenByDescending(x => x.Id),
            ("id", SearchOrder.Asc) => listClone.OrderBy(x => x.Id),
            ("id", SearchOrder.Desc) => listClone.OrderByDescending(x => x.Id),
            ("createdat", SearchOrder.Asc) => listClone.OrderBy(x => x.CreatedAt),
            ("createdat", SearchOrder.Desc) => listClone.OrderByDescending(x => x.CreatedAt),
            // Por default ordena por nome
            _ => listClone.OrderBy(x => x.Name).ThenBy(x => x.Id)
        };

        // Vai fazer a ordenação de cima e depois ainda ordena por CreatedAt
        return orderedEnumerable.ThenBy(x => x.CreatedAt).ToList();
    }
}

[CollectionDefinition(nameof(ListCategoriesApiTestFixture))]
public class ListCategoriesApiTestFixtureCollection : ICollectionFixture<ListCategoriesApiTestFixture>
{
    
}