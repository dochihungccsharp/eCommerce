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
        
        RuleFor(x => x.ImageUrl)
            .Must(path => string.IsNullOrEmpty(path) || File.Exists(path))
            .WithMessage("Logo không tồn tại trong hệ thống.");

        RuleFor(x => x.ParentId)
            .Must(x => x == null || Guid.TryParse(x.ToString(), out _))
            .WithMessage("Danh mục cha không hợp lệ.");
    }
}