using System.Linq;
using schools_web_api.Model;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using schools_web_api.TokenManager.Services.Model;
using schools_web_api.TokenManager.TransmitModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace schools_web_api.TokenManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> logger;
        private readonly IUserService userService;
        private readonly ITokenManager tokenManager;

        public UsersController(ILogger<UsersController> logger, IUserService context, ITokenManager tokenManager)
        {
            this.tokenManager = tokenManager;
            this.userService = context;
            this.logger = logger;
        }

        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateRequest ar)
        {
            var user = await userService.AutenticateUser(ar);

            if (user == null)
            {
                return BadRequest("Username or password is incorrect");
            }

            var response = tokenManager.GenerateAccessTokens((int)user.Id, user.Role, ReadRequestIp());

            return response == null ? BadRequest() : Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest rtq)
        {
            string ip = ReadRequestIp();

            var result = await Task.Run(() => tokenManager.RefreshToken(rtq.RefreshToken, ip));

            if (result == null)
            {
                return BadRequest("Incorrect/expired refresh token");
            }

            return Ok(result);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest rtq)
        {
            var result = await Task.Run(() => tokenManager.RevokeRefreshToken(rtq.RefreshToken));

            if (!result)
            {
                return BadRequest("Logout was not possible");
            }

            return Ok("Logout was successful");
        }

        [HttpPost("password-change")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest cpr)
        {
            string authToken = this.ReadAuthToken();

            if (!tokenManager.VerifyCanChangePassword(authToken, cpr.IdUser))
            {
                return Forbid();
            }

            bool passwordChanged = await this.userService.ChangeUserPasswordAsync(cpr);

            return passwordChanged ? Ok("Password has been changed") : BadRequest("Password has not been changed");
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUserAsync(int id)
        {
            var user = await this.userService.GetUserByIdAsync(id);

            return user == null ? NotFound() : Ok(user);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetUsersAsync()
        {
            var users = await this.userService.GetUsersAsync(null);

            if (users.Count == 0)
            {
                return NotFound();
            }

            return Ok(users);
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> DeleteUserAsync(int id)
        {
            string authToken = this.ReadAuthToken();

            if (!tokenManager.VerifyCanDeleteUser(id, authToken)) 
            {
                return Forbid();
            }

            var result = await this.userService.DeleteUserAsync(id);
            
            return result ? Ok() : BadRequest();
        }

        [HttpPost("Update")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateUserAsync([FromBody] User newUserData)
        {
            string authToken = this.ReadAuthToken();

            if (!tokenManager.VerifyHasHighestPrivilleges(authToken))
            {
                return Unauthorized();
            }

            var oldUserData = await this.userService.GetUserByIdAsync((int)newUserData.Id);

            if (oldUserData == null) 
            {
                return NotFound();
            }

            var result = await this.userService.UpdateUserAsync(oldUserData, newUserData);

            return result ? Ok() : BadRequest();
        }

        [HttpPost("Add")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> AddUserAsync([FromBody] User u)
        {
            string authToken = this.ReadAuthToken();

            if (!tokenManager.VerifyHasHighestPrivilleges(authToken)) 
            {
                return Unauthorized();
            }

            var result = await this.userService.AddUserAsync(u);

            return result ? Ok(result) : BadRequest();
        }

        private string ReadRequestIp()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                return Request.Headers["X-Forwarded-For"];
            }
            else
            {
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            }
        }

        private string ReadAuthToken() 
        {
            return this.Request.Headers["Authorization"].ToString().Split(' ').Last();
        }
    }
}
