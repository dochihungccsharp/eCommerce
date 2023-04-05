namespace eCommerce.Domain.Abstractions.Paginations;

public interface IPagedList<TEntity> where TEntity : class, new()
{
    int PageIndex { get; set; }
    int PageSize { get; set; }
    int TotalCount { get; set; }
    int TotalPages { get; set; }
    IList<TEntity> Items { get; set; }
}