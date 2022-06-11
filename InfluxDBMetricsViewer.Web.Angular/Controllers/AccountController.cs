using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using MetaMetricsViewer.Web.Angular.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace InfluxDB_MetricsViewer.Web.Angular.Controllers
{
    [Route("Account")]
    public class AccountController : ControllerBase
    {
        private readonly IOptions<AccountOptions> _options;

        public AccountController(IOptions<AccountOptions> options)
        {
            _options = options;
        }

        private IEnumerable<Claim> GetUserClaims(string login)
        {
            List<Claim> claims = new List<Claim>();

            claims.Add(new Claim(ClaimTypes.NameIdentifier, login));
            claims.Add(new Claim(ClaimTypes.Name, login));
            // claims.Add(new Claim(ClaimTypes.Email, user.UserEmail));
            // claims.AddRange(this.GetUserRoleClaims(user));
            return claims;
        }


        // private IActionResult RedirectToReferrer()
        // {
        //     var referrer = Request.Headers["Referer"].ToString();
        //     return Redirect(referrer);
        // }

        private JsonResult GetUserProfile(IIdentity identity)
        {
            return new JsonResult(new UserProfileModel()
            {
                Name = identity.Name,
                IsAuthenticated = identity.IsAuthenticated,
            });
        }


        [Authorize]
        [HttpPost]
        [Route(nameof(Profile))]
        public async Task<IActionResult> Profile()
        {
            return GetUserProfile(this.User.Identity);
        }


        [HttpPost]
        [Route(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] AccountLoginModel model)
        {
            // _logger.LogInformation($"{GetType().Name}.{nameof(Login)}\tLogin:{model.Login} Pass: {model.Password?.Length} ContextType: {_configuration.Account.ContextType} Domain: {_configuration.Account.Domain}");


            using (PrincipalContext context =
                new PrincipalContext(_options.Value.ContextType, _options.Value.Domain))
            {
                // _logger.LogInformation($"Login context. ConnectedServer: {context.ConnectedServer}");


                var valid = context.ValidateCredentials(model.Login, model.Password);


                // _logger.LogInformation($"Login context. valid: {valid}");

                if (valid)
                {
                    var identity = new ClaimsIdentity(this.GetUserClaims(model.Login),
                        CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    return GetUserProfile(identity);
                }
                else
                {
                    throw new Exception("Authentication failed.");
                }
            }


            return new JsonResult(new { Success = true });
        }


        [HttpPost]
        [Route(nameof(Logout))]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return new JsonResult(new { Success = true });

            // return RedirectToReferrer();
        }
    }
}