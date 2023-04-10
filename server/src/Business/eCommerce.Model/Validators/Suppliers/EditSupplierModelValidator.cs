using eCommerce.Model.Suppliers;
using FluentValidation;

namespace eCommerce.Model.Validators.Suppliers;

public class EditSupplierModelValidator : AbstractValidator<EditSupplierModel>
{
    public EditSupplierModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Tên không được để trống.")
            .Length(3, 255)
            .WithMessage("Tên có độ dài từ 3 đến 255 kí tự.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Địa chỉ nhà không được để trống.")
            .Length(3, 255)
            .WithMessage("Địa chỉ có độ dài từ 3 đến 255 kí tự.");
            
        RuleFor(x => x.Phone)
            .NotNull()
            .WithMessage("Số điện thoại không được để trống.")
            .Length(10)
            .WithMessage("Số điện thoại phải có 10 chữ số.")
            .Matches(@"^(03|05|07|08|09)+([0-9]{8})$")
            .WithMessage("Số điện thoại không hợp lệ.");
            
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email bắt buộc phải có.")
            .Matches(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$")
            .WithMessage("Email có định dạng không hơp lệ.");
            
            
    }
}