using eCommerce.Model.Brands;
using eCommerce.Model.Validators.Services;
using FluentValidation;

namespace eCommerce.Model.Validators.Brands;

public class EditBrandModelValidator : AbstractValidator<EditBrandModel>
{
    public EditBrandModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên nhãn hiệu không được để trống.")
            .Length(3, 255)
            .WithMessage("Tên nhãn hiệu phải có độ dài từ 3 đến 255 ký tự.");

        RuleFor(x => x.Description)
            .NotNull()
            .WithMessage("Mô tả nhãn hiệu không được để trống.");

        When(x => x.LogoUpload != null, () =>
        {
            RuleFor(x => x.LogoUpload)
                .SetValidator(new FileValidator());
        });
    }
}