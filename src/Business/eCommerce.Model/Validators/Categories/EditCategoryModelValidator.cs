using eCommerce.Model.Categories;
using eCommerce.Model.Validators.Services;
using FluentValidation;

namespace eCommerce.Model.Validators.Categories;

public class EditCategoryModelValidator : AbstractValidator<EditCategoryModel>
{
    public EditCategoryModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên danh mục không được để trống.")
            .Length(3, 50)
            .WithMessage("Tên danh mục có độ dài từ 3 đến 50 ký tự.");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Mô tả danh mục không được để trống.")
            .Length(3, 50)
            .WithMessage("Mô tả danh mục tối thiểu phải có 3 ký tự.");

        When(x => x.ImageUpload != null, () =>
        {
            RuleFor(x => x.ImageUpload)
                .SetValidator(new FileValidator());
        });

        RuleFor(x => x.ParentId)
            .Must(x => x == null || Guid.TryParse(x.ToString(), out _))
            .WithMessage("Danh mục cha không hợp lệ.");
    }
}