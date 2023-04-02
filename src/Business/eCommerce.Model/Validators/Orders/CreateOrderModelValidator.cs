using eCommerce.Model.Orders;
using FluentValidation;

namespace eCommerce.Model.Validators.Orders;

public class CreateOrderModelValidator : AbstractValidator<CreateOrderModel>
{
    public CreateOrderModelValidator()
    {
        
    }
}