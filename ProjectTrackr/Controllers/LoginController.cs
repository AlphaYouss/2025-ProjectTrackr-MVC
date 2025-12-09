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
        private Validator validator = new Validator();

        private UserContainer userContainer { get; set; }

        private string usernameEmail;

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
            else if (isValidLogin(model.usernameOrEmail, model.password) == false)
            {
                ModelState.AddModelError("InvalidLogin", "Combination of email/username and given password is incorrect.");
                return RegisterFailed();
            }
            else
            {
                return await setSignedIn();
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

        private bool isValidLogin(string usernameEmail, string password)
        {
            PasswordHasher<string> hasher = new PasswordHasher<string>();
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

        public async Task<IActionResult> setSignedIn()
        {
            User user = new User();
            user.email = this.usernameEmail;
            user.username = this.usernameEmail;

            DataTable table = userContainer.GetUserDetails(user);

            user.id = Guid.Parse(table.Rows[0]["ID"].ToString());
            user.email = table.Rows[0]["Email"].ToString();
            user.username = table.Rows[0]["Username"].ToString();
            user.createdAt = (DateTime)table.Rows[0]["CreatedAt"];

            List<Claim> claims = new List<Claim>
            {
                    new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                    new Claim(ClaimTypes.Name, user.username),
                    new Claim(ClaimTypes.Email, user.email),
                    new Claim("CreatedAt", user.createdAt.ToString())
            };

            ClaimsIdentity identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            AuthenticationProperties properties = new AuthenticationProperties
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