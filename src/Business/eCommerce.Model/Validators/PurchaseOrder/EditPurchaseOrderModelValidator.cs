using eCommerce.Model.PurchaseOrders;
using FluentValidation;

namespace eCommerce.Model.Validators.PurchaseOrder;

public class EditPurchaseOrderModelValidator : AbstractValidator<EditPurchaseOrderModel>
{
    public EditPurchaseOrderModelValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotEmpty()
            .WithMessage("Mã nhà cung cấp bắt buộc phải có");

        RuleFor(x => x.TotalMoney)
            .GreaterThan(0)
            .WithMessage("Tổng thanh toán phải lớn 0");

        RuleFor(x => x.OrderStatus)
            .NotEmpty()
            .WithMessage("Trạng thái đơn hàng bắt buộc phải có.")
            .Must(status => status.Trim().ToUpper() == "PURCHASE_INVOICE"
                            || status.Trim().ToUpper() == "DRAFT_INVOICE")
            .WithMessage("Trạng thái đơn hàng không hợp lệ.");


        RuleFor(x => x.PaymentStatus)
            .NotEmpty()
            .WithMessage("Trạng thái thanh toán đơn hàng bắt buộc phải có.")
            .Must(status => status.Trim().ToUpper() == "UNPAID" 
                            || status.Trim().ToUpper() == "PAID")
            .WithMessage("Trạng thái thanh toán đơn hàng không hợp lệ.");

        RuleFor(x => x.EditPurchaseOrderDetailsModels)
            .Must(x => x != null && x.Count > 0)
            .WithMessage("Không thể tạo một đơn hàng mà không nhập sản phẩm nào.")
            .ForEach(detailValidator => detailValidator
                .SetValidator(new EditPurchaseOrderDetailsModelValidator()));

    }
}