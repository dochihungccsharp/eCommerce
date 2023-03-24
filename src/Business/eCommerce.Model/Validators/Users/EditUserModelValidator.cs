using eCommerce.Model.Users;
using eCommerce.Model.Validators.Services;
using FluentValidation;

namespace eCommerce.Model.Validators.Users;

public class EditUserModelValidator : AbstractValidator<EditUserModel>
{
    public EditUserModelValidator()
    {
        RuleFor(x => x.Username)
            .NotNull().WithMessage("Vui lòng nhập tên đăng nhập của bạn.")
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.");

        RuleFor(x => x.Fullname)
            .NotEmpty()
            .WithMessage("Họ tên không được để trống.");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Vui lòng nhập địa chỉ email của bạn.")
            .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
            .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");

        RuleFor(x => x.Password)
            .MinimumLength(6).When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage("Mật khẩu phải chứa ít nhất 6 ký tự.");
            
        // Cần xem lại nhóa
        RuleFor(x => x.PhoneNumber)
            .NotNull().WithMessage("Số điện thoại không được để trống.")
            .Length(10).WithMessage("Số điện thoại phải có 10 chữ số.")
            .Matches(@"^(03|05|07|08|09)+([0-9]{8})$")
            .WithMessage("Số điện thoại không hợp lệ.");
        
        
        When(x => x.Avatar != null, () =>
        {
            RuleFor(x => x.Avatar)
                .SetValidator(new FileValidator());
        });
    }
    
}