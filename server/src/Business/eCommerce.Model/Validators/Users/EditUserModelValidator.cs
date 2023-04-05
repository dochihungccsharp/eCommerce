using System.Text.RegularExpressions;
using eCommerce.Model.Users;
using eCommerce.Model.Validators.Services;
using FluentValidation;

namespace eCommerce.Model.Validators.Users;

public class EditUserModelValidator : AbstractValidator<EditUserModel>
{
    public EditUserModelValidator()
    {
        RuleFor(x => x.Username)
            .NotNull()
            .NotEmpty().WithMessage("Tên đăng nhập không được để trống.");

        RuleFor(x => x.Fullname)
            .NotEmpty()
            .WithMessage("Họ tên không được để trống.");

        RuleFor(x => x.Email)
            .NotNull()
            .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
            .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");

        RuleFor(x => x.Password)
            .MinimumLength(6)
            .When(x => !string.IsNullOrEmpty(x.Password))
            .WithMessage("Mật khẩu phải chứa ít nhất 6 ký tự.");
            
        RuleFor(x => x.PhoneNumber)
            .Must(phone => phone == null || Regex.IsMatch(phone, @"^(03|05|07|08|09)+([0-9]{8})$"))
            .WithMessage("Số điện thoại không hợp lệ hoặc không được cung cấp.");
        
        RuleFor(x => x.Avatar)
            .Must(path => string.IsNullOrEmpty(path) || File.Exists(path))
            .WithMessage("Logo không tồn tại trong hệ thống.");
    }
    
}