using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTrackr.Containers;
using ProjectTrackr.DALs;
using ProjectTrackr.Models;
using ProjectTrackr.Models.ViewModels;
using ProjectTrackr.Tools;
using System.Data;
using System.Security.Claims;

namespace ProjectTrackr.Controllers
{
    public class LoginController : Controller
    {
        private Validator validator = new();

        private UserContainer userContainer { get; set; }

        private string usernameEmail { get; set; } = "";

        public LoginController(IConfiguration configuration)
        {
            userContainer = new UserContainer(new UserDAL(configuration));
        }

        // GET: LoginController
        public ActionResult Index()
        {
            return View();
        }

        // POST: LoginController/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!validator.ValidatePassword(model.password))
            {
                ModelState.AddModelError("InvalidPassword", "Password format is invalid.");
                return RegisterFailed();
            }
            else if (userContainer.UserNameEmailExists(model.usernameOrEmail) == false)
            {
                ModelState.AddModelError("UsernameEmailNotKnown", "Username or email does not exist.");
                return RegisterFailed();
            }
            else if (IsValidLogin(model.usernameOrEmail, model.password) == false)
            {
                ModelState.AddModelError("InvalidLogin", "Combination of email/username and given password is incorrect.");
                return RegisterFailed();
            }
            else
            {
                return await SetSignedIn();
            }
        }

        private ActionResult RegisterFailed()
        {
            TempData["Errors"] = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            return RedirectToAction("Index");
        }

        private bool IsValidLogin(string usernameEmail, string password)
        {
            PasswordHasher<string> hasher = new();
            string hashedPassword = userContainer.FetchPassword(usernameEmail);

            PasswordVerificationResult result = hasher.VerifyHashedPassword(null, hashedPassword, password);

            if (result == PasswordVerificationResult.Success)
            {
                this.usernameEmail = usernameEmail;
                return true;
            }

            else
                return false;
        }

        public async Task<IActionResult> SetSignedIn()
        {
            User user = new()
            {
                email = usernameEmail,
                username = usernameEmail
            };

            DataTable table = userContainer.GetUserDetails(user);

            user.id = Guid.Parse(table.Rows[0]["ID"].ToString() ?? String.Empty);
            user.email = table.Rows[0]["Email"].ToString() ?? String.Empty;
            user.username = table.Rows[0]["Username"].ToString() ?? String.Empty;
            user.createdAt = (DateTime)table.Rows[0]["CreatedAt"];

            List<Claim> claims =
            [
                    new(ClaimTypes.NameIdentifier, user.id.ToString()),
                    //new(ClaimTypes.Name, user.username),
                    //new(ClaimTypes.Email, user.email),
            ];

            ClaimsIdentity identity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new()
            {
                AllowRefresh = true,
                ExpiresUtc = DateTime.Now.AddDays(1),
                IsPersistent = true,
                RedirectUri = "/Login"
            };

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), properties);

            return RedirectToAction("All", "Project");
        }
    }
}