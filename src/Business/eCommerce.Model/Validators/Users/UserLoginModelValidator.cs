using eCommerce.Service.Users;
using FluentValidation;

namespace eCommerce.Model.Validators.Users;

public class UserLoginModelValidator : AbstractValidator<UserLoginModel>
{
    public UserLoginModelValidator()
    {
        RuleFor(x => x.Email)
            .NotNull().WithMessage("Email không được để trống.")
            .NotEmpty().WithMessage("Email không được để trống.")
            .MinimumLength(5).WithMessage("Email có độ dài quá ngắn, độ dài tối thiểu phải có 5 ký tự.")
            .EmailAddress().WithMessage("Email có định dạng không hợp lệ.");
        
        RuleFor(x => x.Password)
            .NotNull().NotEmpty()
            .MinimumLength(6)
            .WithMessage("Mật khẩu không được để trống và phải có ít nhất 6 ký tự.");
    }
}