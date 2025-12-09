using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace Unit_tests.Stubs
{
    public class UserContainerStubs : IUser
    {
        public User user;
        public bool? existReturnValue = null;
        public bool? successReturnValue = null;
        public string? stringReturnValue = null;

        public Guid CreateUser(User user)
        {
            if (successReturnValue == null || user == null)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field successReturnValue and the user.");
            }
            user = new();

            return user.id;
        }

        public bool EditEmail(User user, string newEmail)
        {
            if (successReturnValue == null || newEmail == null)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field successReturnValue and the newEmail.");
            }
            return successReturnValue.Value;
        }

        public bool EditPassword(User user, string newPassword)
        {
            if (successReturnValue == null || newPassword == null)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field successReturnValue and the newPassword.");
            }
            return successReturnValue.Value;
        }

        public bool EmailExists(string email)
        {
            if (existReturnValue == null || email == "")
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and email.");
            }
            return existReturnValue.Value;
        }

        public string FetchPassword(string usernameEmail)
        {
            if (existReturnValue == null || usernameEmail == "")
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and usernameEmail.");
            }

            int length = 10;
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new();
            string randomString = new(Enumerable.Repeat(chars, length)
                                    .Select(s => s[random.Next(s.Length)])
                                    .ToArray());

            stringReturnValue = randomString;
            return stringReturnValue;
        }

        public DataTable GetUserDetails(User user)
        {
            user = new();

            if (existReturnValue == null || user.id == Guid.Empty)
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and user.");
            }
            return new DataTable();
        }

        public bool UserNameEmailExists(string usernameEmail)
        {
            if (existReturnValue == null || usernameEmail == "")
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and usernameEmail.");
            }
            return existReturnValue.Value;
        }

        public bool UserNameExists(string username)
        {
            if (existReturnValue == null || username == "")
            {
                throw new NullReferenceException("Invalid use of stub code. First set field existReturnValue and username.");
            }
            return existReturnValue.Value;
        }
    }
}