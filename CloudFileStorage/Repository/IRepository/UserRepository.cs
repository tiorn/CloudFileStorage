using CloudFileStorage.Models;
using Microsoft.AspNetCore.Identity;
using CloudFileStorage.Models.Enum;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using CloudFileStorage.Models.Response;
using Microsoft.EntityFrameworkCore;

namespace CloudFileStorage.Repository.IRepository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext db, ILogger<UserRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public bool IsUniqueUser(RegisterViewModel model)
        {
            var user = _db.Users.FirstOrDefault(x =>
                x.UserName.ToLower() == model.UserName.ToLower() || x.Email == model.Email);
            return user == null;
        }

        public async Task<BaseResponse<ClaimsIdentity>> Login(LoginViewModel model, IPasswordHasher<User> hasher)
        {
            try
            {
                var user = await _db.Users.FirstOrDefaultAsync(x => x.UserName.ToLower() == model.UserName.ToLower());

                if (user != null)
                {
                    var result = hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);

                    if (result == PasswordVerificationResult.Success)
                    {
                        return new BaseResponse<ClaimsIdentity>()
                        {
                            Description = "Logged in successfully",
                            StatusCode = StatusCode.OK,
                            IsSuccess = true,
                            Data = Authenticate(user)
                        };
                    }
                }

                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = "Bad credentials",
                    StatusCode = StatusCode.BadRequest,
                    IsSuccess = false
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserRepository.Login.Error:{ExMessage}", ex.Message);
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = $"Error:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError,
                    IsSuccess = false
                };
            }
        }

        public async Task<BaseResponse<ClaimsIdentity>> Register(RegisterViewModel model, IPasswordHasher<User> hasher)
        {
            try
            {
                if (!IsUniqueUser(model))
                {
                    return new BaseResponse<ClaimsIdentity>()
                    {
                        Description = "Account already exists",
                        StatusCode = StatusCode.BadRequest,
                        IsSuccess = false
                    };
                }
                var user = new User
                {
                    CreatedDate = DateTime.UtcNow,
                    ModifiedDate = DateTime.UtcNow,
                    UserName = model.UserName,
                    PasswordHash = hasher.HashPassword(new User(), model.Password),
                    Email = model.Email
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync();

                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = "Registration completed successfully",
                    StatusCode = StatusCode.OK,
                    IsSuccess = true,
                    Data = Authenticate(user)
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserRepository.Register.Error:{ExMessage}", ex.Message);
                return new BaseResponse<ClaimsIdentity>()
                {
                    Description = $"Error:{ex.Message}",
                    StatusCode = StatusCode.InternalServerError,
                    IsSuccess = false
                };
            }
        }

        public ClaimsIdentity Authenticate(User user) 
        { 
            var claims = new List<Claim>()
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.UserName)
            };
            return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}
