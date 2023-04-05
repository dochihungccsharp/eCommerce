using eCommerce.Model.Users;
using FluentValidation;

namespace eCommerce.Model.Validators.Users;

public class UserRegistrationModelValidator : AbstractValidator<UserRegistrationModel>
{
    public UserRegistrationModelValidator()
    {
        RuleFor(x => x.Fullname)
            .NotNull().WithMessage("Vui lòng nhập họ tên của bạn.")
            .NotEmpty().WithMessage("Họ tên không được để trống.");
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được để trống.")
            .Matches(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")
            .WithMessage("Email có định dạng không hơp lệ.");
        
        RuleFor(x => x.Password)
            .NotNull().WithMessage("Vui lòng nhập mật khẩu của bạn.")
            .NotEmpty().WithMessage("Mật khẩu không được để trống.")
            .MinimumLength(6).WithMessage("Mật khẩu phải chứa ít nhất 6 ký tự.");
        
        RuleFor(x => x.PhoneNumber)
            .NotNull().WithMessage("Số điện thoại không được để trống.")
            .Length(10).WithMessage("Số điện thoại phải có 10 chữ số.")
            .Matches(@"^(03|05|07|08|09)+([0-9]{8})$")
            .WithMessage("Số điện thoại không hợp lệ.");
    }
}