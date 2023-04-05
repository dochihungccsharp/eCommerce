namespace eCommerce.Domain.Abstractions.Paginations;

public class PagedList<TEntity> : IPagedList<TEntity> where TEntity : class, new()
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IList<TEntity> Items { get; set; }

    public PagedList(
        int pageIndex,
        int pageSize,
        int totalCount,
        int totalPages,
        IList<TEntity> items
    )
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalCount = totalCount;
        TotalPages = totalPages;
        Items = items;
    }
    internal PagedList() => Items = Array.Empty<TEntity>();
}