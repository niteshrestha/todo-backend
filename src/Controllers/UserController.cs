using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Todo.API.Helpers;
using Todo.Data.Identity;
using Todo.Data.User;

namespace Todo.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        #region Fields
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        #endregion

        #region Constructors
        public UserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Register new user
        /// </summary>
        /// <param name="registerEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerEntity)
        {
            if (ModelState.IsValid)
            {
                var appUser = new ApplicationUser
                {
                    FirstName = registerEntity.FirstName,
                    LastName = registerEntity.LastName,
                    Email = registerEntity.Email,
                    UserName = registerEntity.Username
                };

                var user = await _userManager.CreateAsync(appUser, registerEntity.Password);

                if (user.Succeeded)
                {
                    await _signInManager.SignInAsync(appUser, false);
                    var token = AuthenticationHelper.GenerateJWTToken(appUser.Id.ToString());
                    return Ok(new
                    {
                        userId = appUser.Id.ToString(),
                        token
                    });
                }
                return Ok(string.Join(",", user.Errors?.Select(error => error.Description)));
            }
            string errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return BadRequest(errorMessage ?? "Bad request");
        }

        /// <summary>
        /// Login existing user
        /// </summary>
        /// <param name="loginEntity"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel loginEntity)
        {
            if (ModelState.IsValid)
            {
                var userSignIn = await _signInManager.PasswordSignInAsync(loginEntity.Username, loginEntity.Password, false, false);

                if (userSignIn.Succeeded)
                {
                    var appUser = _userManager.Users.SingleOrDefault(x => x.UserName == loginEntity.Username);
                    var token = AuthenticationHelper.GenerateJWTToken(appUser.Id.ToString());
                    return Ok(new
                    {
                        userId = appUser.Id.ToString(),
                        token
                    });
                }
                return StatusCode((int)HttpStatusCode.Unauthorized, "Bad Credentials");
            }
            string errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            return BadRequest(errorMessage ?? "Bad Request");
        }
        #endregion
    }
}
