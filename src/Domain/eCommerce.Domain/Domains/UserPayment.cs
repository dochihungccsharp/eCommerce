namespace eCommerce.Domain.Domains;

public class UserPayment
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string PaymentType { get; set; }
    public string Provider { get; set; }
    public int AccountNo { get; set; }
    public DateTime Expiry { get; set; }
}