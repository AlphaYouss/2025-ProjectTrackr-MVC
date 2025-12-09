using System.Text.RegularExpressions;

namespace ProjectTrackr.Tools
{
    public class Validator
    {
        public bool ValidateUsername(string username)
        {
            // Validate username (mostly letters, 4 min)

            Regex regex = new Regex(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&*]{4,}$");
            bool isValid = regex.IsMatch(username.Trim());

            if (!isValid)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ValidateEmail(string email)
        {
            // Validate email

            Regex regex = new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            bool isValid = regex.IsMatch(email.Trim());

            if (!isValid)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ValidatePassword(string password)
        {
            // Validate password (letters & numbers & special characters, 8 min)

            Regex regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_\-+=?]).{8,}$");
            bool isValid = regex.IsMatch(password.Trim());

            if (!isValid)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool ValidateString(string stringContent)
        {
            // Validate string (mostly letters, 3 min)

            Regex regex = new Regex(@"^(?=.*[a-zA-Z\d].*)[a-zA-Z\d!@#$%&* ]{3,}$");
            bool isValid = regex.IsMatch(stringContent.Trim());

            if (!isValid)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}