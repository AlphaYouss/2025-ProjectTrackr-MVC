using Microsoft.Data.SqlClient;
using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.DALs
{
    public class UserDAL : IUser
    {
        private DatabaseHandler databasehandler { get; set; }

        private SqlCommand command { get; set; }
        private DataTable table { get; set; }

        public UserDAL(IConfiguration configuration) {

            databasehandler = new DatabaseHandler(configuration);
            databasehandler.TestConnection();

            command = new SqlCommand();
            table = new DataTable();
        }

        public bool EmailExists(string email)
        {
            command = new SqlCommand("SELECT COUNT(*) FROM [Users] WHERE Email = @Email", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Email", email);

            databasehandler.OpenConnection();
            int count = Convert.ToInt32(command.ExecuteScalar());
            databasehandler.CloseConnection();

            if (count > 0)
                return true;
            else
                return false;
        }

        public bool UserNameExists(string username)
        {
            command = new SqlCommand("SELECT COUNT(*) FROM [Users] WHERE Username = @Username", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Username", username);

            databasehandler.OpenConnection();
            int count = Convert.ToInt32(command.ExecuteScalar());
            databasehandler.CloseConnection();

            if (count > 0)
                return true;
            else
                return false;
        }

        public bool UserNameEmailExists(string usernameEmail)
        {
            command = new SqlCommand("SELECT COUNT(*) FROM [Users] WHERE Username = @Username OR Email = @Email", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Username", usernameEmail);
            command.Parameters.AddWithValue("Email", usernameEmail);

            databasehandler.OpenConnection();
            int count = Convert.ToInt32(command.ExecuteScalar());
            databasehandler.CloseConnection();

            if (count > 0)
                return true;
            else
                return false;
        }

        public string FetchPassword(string usernameEmail)
        {
            command = new SqlCommand("SELECT PasswordHash FROM [Users] WHERE Username = @Username OR Email = @Email", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Username", usernameEmail);
            command.Parameters.AddWithValue("Email", usernameEmail);

            databasehandler.OpenConnection();
            string response = (string)command.ExecuteScalar();
            databasehandler.CloseConnection();

            return response;
        }

        public Guid CreateUser(User user)
        {
            command = new SqlCommand("INSERT INTO [Users] VALUES(@ID, @Username, @Email, @PasswordHash, @CreatedAt)", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", user.id);
            command.Parameters.AddWithValue("Username", user.username);
            command.Parameters.AddWithValue("Email", user.email);
            command.Parameters.AddWithValue("PasswordHash", user.passwordHash);
            command.Parameters.AddWithValue("CreatedAt", user.createdAt);

            databasehandler.OpenConnection();
            command.ExecuteNonQuery();
            databasehandler.CloseConnection();

            return user.id;
        }

        public DataTable GetUserDetails(User user)
        {
            table = new DataTable();

            command = new SqlCommand("SELECT ID, Username, Email, CreatedAt FROM [Users] WHERE ID = @ID OR Username = @Username OR Email = @Email", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", user.id);
            command.Parameters.AddWithValue("Username", user.username);
            command.Parameters.AddWithValue("Email", user.email);

            databasehandler.OpenConnection();

            SqlDataAdapter adapt = new SqlDataAdapter(command);
            adapt.Fill(table);

            databasehandler.CloseConnection();

            return table;
        }

        public bool EditEmail(User user, string newEmail)
        {
            command = new SqlCommand("SELECT Email FROM [Users] WHERE ID = @ID", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", user.id);

            databasehandler.OpenConnection();
            string email = (string)command.ExecuteScalar();
            databasehandler.CloseConnection();

            if (email != newEmail)
            {
                command = new SqlCommand("UPDATE [Users] SET Email = @Email WHERE ID = @userID", databasehandler.GetConnection());
                command.Parameters.AddWithValue("ID", user.id);
                command.Parameters.AddWithValue("Email", newEmail);

                databasehandler.OpenConnection();
                command.ExecuteNonQuery();
                databasehandler.CloseConnection();

                return true;
            }
            return false;
        }

        public bool EditPassword(User user, string newPassword)
        {
            command = new SqlCommand("SELECT PasswordHash FROM [Users] WHERE ID = @ID", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", user.id);

            databasehandler.OpenConnection();
            string passwordHash = (string)command.ExecuteScalar();
            databasehandler.CloseConnection();

            if (passwordHash != newPassword)
            {
                command = new SqlCommand("UPDATE [Users] SET PasswordHash = @PasswordHash WHERE ID = @ID", databasehandler.GetConnection());
                command.Parameters.AddWithValue("ID", user.id);
                command.Parameters.AddWithValue("PasswordHash", newPassword);

                databasehandler.OpenConnection();
                command.ExecuteNonQuery();
                databasehandler.CloseConnection();

                return true;
            } 
            return false;
        }
    }
}