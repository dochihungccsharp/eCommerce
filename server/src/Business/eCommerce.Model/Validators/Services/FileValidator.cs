using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace eCommerce.Model.Validators.Services;

public  class FileValidator : AbstractValidator<IFormFile>
{
    public FileValidator()
    {
        RuleFor(x => x.ContentType).NotNull().Must(x =>
                x.Equals("image/jpeg") ||
                x.Equals("image/jpg") ||
                x.Equals("image/png") ||
                x.Equals("image/gif"))
            .WithMessage("File không hợp lệ, file phải là jpeg, jpg, png, gif.");
    }
}