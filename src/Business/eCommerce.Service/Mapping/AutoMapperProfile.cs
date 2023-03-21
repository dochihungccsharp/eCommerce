using AutoMapper;
using eCommerce.Domain.Domains;
using eCommerce.Model.Users;
using eCommerce.Shared.Extensions;

namespace eCommerce.Service.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<UserRegistrationModel, User>()
            .AfterMap((src, dest) =>
            {
                dest.Username = src.Email;
                dest.PasswordHash = src.Password.HashMD5();
            });

    }
}