using eCommerce.Model.PurchaseOrders;
using FluentValidation;

namespace eCommerce.Model.Validators.PurchaseOrder;

public class EditPurchaseOrderModelValidator : AbstractValidator<EditPurchaseOrderModel>
{
    public EditPurchaseOrderModelValidator()
    {
        RuleFor(x => x.SupplierId)
            .NotNull()
            .NotEmpty()
            .WithMessage("Mã nhà cung cấp bắt buộc phải có");
        
        RuleFor(x => x.UserId)
            .NotNull()
            .NotEmpty()
            .WithMessage("Mã nhân viên cấp bắt buộc phải có");

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
            .WithMessage("Vui lòng thêm sản phẩm trước khi tạo đơn hàng.")
            .ForEach(detailValidator => detailValidator
                .SetValidator(new EditPurchaseOrderDetailsModelValidator()));

    }
}