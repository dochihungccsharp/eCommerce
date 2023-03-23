using AutoMapper;
using eCommerce.Domain.Abstractions.Paginations;
using eCommerce.Domain.Domains;
using eCommerce.Model.Paginations;
using eCommerce.Model.Roles;
using eCommerce.Model.Users;
using eCommerce.Shared.Extensions;

namespace eCommerce.Service.Mapping;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        #region CREATE MAPPER USER
        CreateMap<User, UserModel>().ReverseMap();
        CreateMap<IPagedList<User>, PaginationModel<UserModel>>().ReverseMap();
        CreateMap<UserRegistrationModel, User>()
            .AfterMap((src, dest) =>
            {
                dest.Username = src.Email;
                dest.PasswordHash = src.Password.HashMD5();
                dest.EmailConfirmed = false;
            });

        CreateMap<EditUserModel, User>()
            .AfterMap((src, dest) =>
            {
                dest.EmailConfirmed = true;
                dest.PasswordHash = src.Password.HashMD5();
                dest.Avatar = default!;
                dest.TotalAmountOwed = 0;
                dest.Status = false;
            });
        
        CreateMap<EditProfileModel, User>()
            .AfterMap((src, dest) =>
            {
                dest.Avatar = default!;
            });
        #endregion

        #region CREATE MAPPER ROLE
        CreateMap<Role, EditRoleModel>().ReverseMap();
        CreateMap<Role, AddRoleModel>().ReverseMap();
        // CreateMap<List<Role>, List<AddRoleModel>>().ReverseMap();
        CreateMap<Role, RoleModel>().ReverseMap();
        #endregion

        #region CREATE MAPPPER USERROLE

        

        #endregion
        
    }
}