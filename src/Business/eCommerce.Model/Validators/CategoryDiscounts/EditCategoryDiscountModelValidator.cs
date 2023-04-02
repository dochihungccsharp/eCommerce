using eCommerce.Model.CategoryDiscounts;
using eCommerce.Shared.Consts;
using FluentValidation;

namespace eCommerce.Model.Validators.CategoryDiscounts;

public class EditCategoryDiscountModelValidator : AbstractValidator<EditCategoryDiscountModel>
{
    public EditCategoryDiscountModelValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Mã code giảm giá bắt buộc phải có.")
            .MinimumLength(3)
            .WithMessage("Mã code phải có tối thiểu 3 ký tự.");

        RuleFor(x => x.DiscountType)
            .NotEmpty()
            .WithMessage("Loại giảm giá bắt buộc phải có.")
            .Must(status => status.Trim().ToUpper() == Discount.PERCENT
                            || status.Trim().ToUpper() == Discount.FIXED)
            .WithMessage("Loại giảm giá không hợp lệ.");

        RuleFor(x => x.StartDate)
            .NotEmpty()
            .WithMessage("Ngày bắt đầu bắt buộc phải có.")
            .GreaterThanOrEqualTo(DateTime.Now)
            .WithMessage("Ngày bắt đầu phải lớn hơn hoặc bằng thời điểm hiện tại.");

        RuleFor(x => x.EndDate)
            .NotEmpty()
            .WithMessage("Ngày kết thúc bắt buộc phải có.")
            .GreaterThan(x => x.StartDate)
            .WithMessage("Ngày kết thúc phải sau ngày bắt đầu.");
            

        RuleFor(x => x.DiscountValue)
            .Must((model, value) =>
            {
                if (model.DiscountType.ToUpper() == Discount.PERCENT)
                {
                    return value >= 0 && value <= 100;
                }

                return true; 
            })
            .WithMessage("Giá trị giảm giá phải nằm trong khoảng từ 0 đến 100 nếu loại giảm giá là Percent.");
    }
}
