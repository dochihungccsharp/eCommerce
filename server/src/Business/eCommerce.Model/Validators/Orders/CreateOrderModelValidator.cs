using eCommerce.Model.Orders;
using FluentValidation;

namespace eCommerce.Model.Validators.Orders;

public class CreateOrderModelValidator : AbstractValidator<CreateOrderModel>
{
    public CreateOrderModelValidator()
    {
        RuleFor(o => o.Total)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Tổng thanh toán không hợp lệ.");
        RuleFor(o => o.PaymentStatus)
            .Must(ps => ps == "PAID" || ps == "UNPAID")
            .WithMessage("Trạng thái thanh toán không hợp lệ.");
        RuleFor(o => o.PaymentMethod)
            .Must(pm => pm == "COD" || pm == "VNPAY")
            .WithMessage("Phương thức thanh toán không hợp lệ.");
    }
}