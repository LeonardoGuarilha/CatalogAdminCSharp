using FC.Codeflix.Catalog.Application.Common;
using FC.Codeflix.Catalog.Domain.SearchableRepository;
using MediatR;

namespace FC.Codeflix.Catalog.Application.UseCases.Category.ListCategories;

public class ListCategoriesInput : PaginatedListInput, IRequest<ListCategoriesOutput>
{
    public ListCategoriesInput(
        int page = 1, 
        int perPage = 15, 
        string search = "", 
        string sort = "", 
        SearchOrder dir = SearchOrder.Asc) 
        : base(page, perPage, search, sort, dir)
    { }
    
    // Para não dar problema no binding na controller, no [FromRoute] no método List
    public ListCategoriesInput() : base(1, 15, "", "", SearchOrder.Asc)
    { }
}