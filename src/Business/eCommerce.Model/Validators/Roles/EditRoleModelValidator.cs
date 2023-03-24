using eCommerce.Model.Roles;
using FluentValidation;

namespace eCommerce.Model.Validators.Roles;

public class EditRoleModelValidator : AbstractValidator<EditRoleModel>
{
    public EditRoleModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên quyền không được để trống.")
            .MinimumLength(3)
            .WithMessage("Tên quyền phải có tối thiểu 3 ký tự.");
    }
}