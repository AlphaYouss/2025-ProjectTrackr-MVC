using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.Interfaces
{
    public interface IUser
    {
        bool UserNameExists(string username);
        bool EmailExists(string email);
        Guid CreateUser(User user);
        bool UserNameEmailExists(string usernameEmail);
        string FetchPassword(string usernameEmail);
        bool EditPassword(User user, string newPassword);
        bool EditEmail(User user, string newEmail);
        DataTable GetUserDetails(User user);
    }
}