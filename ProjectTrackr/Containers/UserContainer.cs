using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.Containers
{
    public class UserContainer
    {
        IUser userDAL;

        public UserContainer(IUser userDAL)
        {
            this.userDAL = userDAL;
        }

        public bool UserNameExists(string username)
        {
            return userDAL.UserNameExists(username);
        }

        public bool EmailExists(string email)
        {
            return userDAL.EmailExists(email);
        }

        public Guid CreateUser(User user)
        {
           return userDAL.CreateUser(user);
        }

        public bool UserNameEmailExists(string usernameEmail)
        {
            return userDAL.UserNameEmailExists(usernameEmail);
        }

        public string FetchPassword(string usernameEmail)
        {
            return userDAL.FetchPassword(usernameEmail);
        }

        public bool EditPassword(User user, string newPassword)
        {
            return userDAL.EditPassword(user, newPassword);
        }

        public bool EditEmail(User user, string newEmail)
        {
            return userDAL.EditEmail(user, newEmail);
        }

        public DataTable GetUserDetails(User user)
        {
            return userDAL.GetUserDetails(user);
        }   
    }
}
