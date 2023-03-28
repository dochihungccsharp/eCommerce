using eCommerce.Model.Users;
using eCommerce.Model.Validators.Services;
using FluentValidation;

namespace eCommerce.Model.Validators.Users;

public class EditProfileModelValidator : AbstractValidator<EditProfileModel>
{
    public EditProfileModelValidator()
    {
        RuleFor(x => x.Fullname)
            .NotEmpty()
            .WithMessage("Họ tên không được để trống.");

        RuleFor(x => x.Email)
            .NotNull().WithMessage("Vui lòng nhập địa chỉ email của bạn.")
            .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
            .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");
        
            
        // Cần xem lại nhóa
        RuleFor(x => x.PhoneNumber)
            .NotNull().WithMessage("Số điện thoại không được để trống.")
            .Length(10).WithMessage("Số điện thoại phải có 10 chữ số.")
            .Matches(@"^(03|05|07|08|09)+([0-9]{8})$")
            .WithMessage("Số điện thoại không hợp lệ.");
        
        
        RuleFor(x => x.Avatar)
            .Must(path => string.IsNullOrEmpty(path) || File.Exists(path))
            .WithMessage("Logo không tồn tại trong hệ thống.");
    }
}