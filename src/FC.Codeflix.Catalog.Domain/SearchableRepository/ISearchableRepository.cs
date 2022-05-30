using FC.Codeflix.Catalog.Domain.SeedWork;

namespace FC.Codeflix.Catalog.Domain.SearchableRepository;

public interface ISearchableRepository<TAggregate> where TAggregate : AggregateRoot
{
    Task<SearchOutput<TAggregate>> Search(SearchInput input, CancellationToken cancellationToken);
}