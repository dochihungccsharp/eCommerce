using eCommerce.Model.Products;
using eCommerce.Model.Validators.Services;
using FluentValidation;

namespace eCommerce.Model.Validators.Products;

public class EditProductModelValidator : AbstractValidator<EditProductModel>
{
    public EditProductModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên sản phẩm không được để trống.")
            .Length(3, 50)
            .WithMessage("Tên có độ dài từ 3 đến 255 ký tự.");

        RuleFor(x => x.OriginalPrice)
            .NotEmpty()
            .WithMessage("Giá gốc không được để trống.");
            
        RuleFor(x => x.Price)
            .NotEmpty()
            .WithMessage("Giá bán không được để trống.");

        RuleFor(x => x.ImageUrl)
            .Must(path => string.IsNullOrEmpty(path) || File.Exists(path))
            .WithMessage("Image không tồn tại trong hệ thống.");
    }
}