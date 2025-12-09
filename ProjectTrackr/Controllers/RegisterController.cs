using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTrackr.Containers;
using ProjectTrackr.DALs;
using ProjectTrackr.Tools;
using ProjectTrackr.Models;
using ProjectTrackr.Models.ViewModels;

namespace ProjectTrackr.Controllers
{
    public class RegisterController : Controller
    {
        private Validator validator = new();

        private UserContainer userContainer { get; set; }

        public RegisterController(IConfiguration configuration) { 
        
            userContainer = new UserContainer(new UserDAL(configuration));
        }

        // GET: RegisterController
        public ActionResult Index()
        {
            return View();
        }

        // POST: RegisterController/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!CheckInputs(model))
                return RegisterFailed();
            else if (userContainer.UserNameExists(model.username) == true)
            {
                ModelState.AddModelError("UsernameExists", "Username is already taken.");
                return RegisterFailed();
            }
            else if (userContainer.EmailExists(model.email) == true)
            {
                ModelState.AddModelError("EmailExists", "An account with this email already exists.");
                return RegisterFailed();
            }
            else
            {
                CreateUser(model.username, model.email, model.password);

                return RedirectToAction("Index", "Login");
            }
        }

        private bool CheckInputs(RegisterViewModel model)
        {
            if (model.password != model.passwordRepeat)
            {
                ModelState.AddModelError("PasswordMismatch", "Passwords do not match.");
                return false;
            }
            else if (!validator.ValidateUsername(model.username))
            {
                ModelState.AddModelError("InvalidUsername", "Username format is invalid.");
                return false;
            }
            else if(!validator.ValidateEmail(model.email))
            {
                ModelState.AddModelError("InvalidEmail", "Email format is invalid.");
                return false;
            }
            else if (!validator.ValidatePassword(model.password))
            {
                ModelState.AddModelError("InvalidPassword", "Password format is invalid.");
                return false;
            }
            else
                return true;
        }

        private RedirectToActionResult RegisterFailed()
        {
            TempData["Errors"] = ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

            return RedirectToAction("Index");
        }

        private void CreateUser(string username, string email, string password)
        {
            string hashedPassword = new PasswordHasher<string>().HashPassword(username, password);

            User user = new()
            {
                username = username,
                email = email,
                passwordHash = hashedPassword,
                createdAt = DateTime.Now
            };

            userContainer.CreateUser(user);
        }
    }
}