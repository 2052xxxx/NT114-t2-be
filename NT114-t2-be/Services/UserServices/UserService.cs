using NT114_t2_be.Models;
using System.Security.Claims;

namespace NT114_t2_be.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserAuth GetUserName()
        {
            var username = string.Empty;
            var email = string.Empty;
            if (_httpContextAccessor.HttpContext != null)
            {
                username = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name);
                email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            }
            return new UserAuth {
                Username = username,
                Email = email
            };
        }
    }
}
