using Basics.CustomPolicyProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        //    private readonly IAuthorizationService _authorizationService;
        //    public HomeController(IAuthorizationService authorizationService)
        //    {
        //        _authorizationService = authorizationService;
        //    }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }

        [Authorize(Policy = "Claim.DoB")]
        [Authorize(Policy = "SecurityLevel.5")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }

        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }


        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }


        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var granyClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"Bob@gmail.com"),
                new Claim(ClaimTypes.DateOfBirth,"12/23/1989"),
                new Claim(ClaimTypes.Role,"Admin"),
                new Claim(ClaimTypes.Role,"AdminTwo"),
                new Claim(DynamicPolicies.SecurityLevel,"7"),
                new Claim("Grany.Says","Very nice Bob"),
            };

            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob K Foo"),
                new Claim("DrivingLicense","A+"),
            };

            var granyIdentity = new ClaimsIdentity(granyClaims, "Grany Identity");

            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");


            var userPrincipal = new ClaimsPrincipal(new[] { granyIdentity, licenseIdentity });
            //------------------------------------------------------------
            HttpContext.SignInAsync(userPrincipal);


            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DoStuff([FromServices] IAuthorizationService authorizationService)

        {
            //we are doing stuff here
            var builder = new AuthorizationPolicyBuilder("Schema");
            var CustomPolicy = builder.RequireClaim("Hello").Build();
            var authResult = await authorizationService.AuthorizeAsync(HttpContext.User, CustomPolicy);

            if (authResult.Succeeded)
            {
                return View("Index");
            }

            return View("Index");
        }
    }
}
