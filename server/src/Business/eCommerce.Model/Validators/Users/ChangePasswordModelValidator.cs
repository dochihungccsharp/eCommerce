using eCommerce.Model.Users;
using FluentValidation;

namespace eCommerce.Model.Validators.Users;

public class ChangePasswordModelValidator : AbstractValidator<ChangePasswordModel>
{
    public ChangePasswordModelValidator()
    {
        RuleFor(x => x.OldPassword)
            .NotNull().WithMessage("Vui lòng nhập mật khẩu cũ của bạn.")
            .NotEmpty().WithMessage("Mật khẩu cũ không được để trống.")
            .MinimumLength(6).WithMessage("Mật khẩu cũ phải chứa ít nhất 6 ký tự.");
        
        RuleFor(x => x.NewPassword)
            .NotNull().WithMessage("Vui lòng nhập mật khẩu mới của bạn.")
            .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
            .MinimumLength(6).WithMessage("Mật khẩu mới phải chứa ít nhất 6 ký tự.");
    }
}