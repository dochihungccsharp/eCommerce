namespace eCommerce.Model.Paginations;

public class PaginationModel<T>
{
    public int PageIndex { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public IList<T> Items { get; set; }
}