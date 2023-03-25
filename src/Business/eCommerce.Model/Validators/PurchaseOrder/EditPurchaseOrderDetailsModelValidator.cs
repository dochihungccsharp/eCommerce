using eCommerce.Model.PurchaseOrderDetails;
using FluentValidation;

namespace eCommerce.Model.Validators.PurchaseOrder;

public class EditPurchaseOrderDetailsModelValidator : AbstractValidator<EditPurchaseOrderDetailsModel>
{
    public EditPurchaseOrderDetailsModelValidator()
    {
        RuleFor(x => x.Quantity)
            .GreaterThan(0)
            .WithMessage("Số lượng sản phẩm cần nhập hàng phải lớn hơn 0");
        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Số lượng sản phẩm cần nhập hàng phải lớn hơn 0");
    }
}