using eCommerce.Shared.Extensions;

namespace eCommerce.Domain.Abstractions.Paginations;

public static class PagedListExtensions
{
    public static IPagedList<TEntity> ToPageList<TEntity>(
        this IEnumerable<TEntity> source,
        int pageIndex, 
        int pageSize
    )
        where TEntity : class, new()
    {
        if(!typeof(IPagedDomain).IsAssignableFrom(typeof(TEntity)))
            throw new NotSupportedException("Pagination isn't supported.");
        
        var totalCount = source.NotNullOrEmpty() ? (source.First() as IPagedDomain).TotalRows : 0;
        
        var items = source.ToList();


        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PagedList<TEntity>(pageIndex, pageSize, totalCount, totalPages, items);
    }
}