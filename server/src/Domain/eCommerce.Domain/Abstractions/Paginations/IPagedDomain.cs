namespace eCommerce.Domain.Abstractions.Paginations;

public interface IPagedDomain
{
    int TotalRows { get; set; }
}