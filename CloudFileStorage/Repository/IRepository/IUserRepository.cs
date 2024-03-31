using CloudFileStorage.Models;
using CloudFileStorage.Models.Response;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace CloudFileStorage.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueUser(RegisterViewModel model);
        Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model, IPasswordHasher<User> hasher);
        Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model, IPasswordHasher<User> hasher);
        ClaimsIdentity Authenticate(User user);
    }
}
