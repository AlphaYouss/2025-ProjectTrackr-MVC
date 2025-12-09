using Microsoft.Data.SqlClient;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.DALs
{
    public class ProjectDAL : IProject
    {
        private DatabaseHandler databasehandler { get; set; }

        private SqlCommand command { get; set; }
        private DataTable table { get; set; }

        public ProjectDAL(IConfiguration configuration)
        {

            databasehandler = new DatabaseHandler(configuration);
            databasehandler.TestConnection();

            command = new SqlCommand();
            table = new DataTable();
        }

        public Guid CreateProject(Project project)
        {
            command = new SqlCommand("INSERT INTO [Projects] VALUES(@ID, @Name, @Description, @CreatedAt, @OwnerId)", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", project.id);
            command.Parameters.AddWithValue("Name", project.name);
            command.Parameters.AddWithValue("Description", project.description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("CreatedAt", project.createdAt);
            command.Parameters.AddWithValue("OwnerId", project.ownerId);

            databasehandler.OpenConnection();
            command.ExecuteNonQuery();
            databasehandler.CloseConnection();

            return project.id;
        }

        public void EditProject(Project project)
        {
            command = new SqlCommand("UPDATE [Projects] SET Name = @Name, Description = @Description WHERE ID = @ID", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Name", project.name);
            command.Parameters.AddWithValue("Description", project.description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("ID", project.id);

            databasehandler.OpenConnection();
            command.ExecuteNonQuery();
            databasehandler.CloseConnection();
        }

        public DataTable GetProjects(User user)
        {
            command = new SqlCommand("SELECT * FROM [Projects] WHERE OwnerID = @OwnerId", databasehandler.GetConnection());
            command.Parameters.AddWithValue("OwnerId", user.id);

            databasehandler.OpenConnection();

            SqlDataAdapter adapt = new SqlDataAdapter(command);
            adapt.Fill(table);

            databasehandler.CloseConnection();

            return table;
        }

        public bool ProjectExists(string projectName, Guid projectId, bool editProject)
        {
            command = new SqlCommand("SELECT COUNT(*) FROM [Projects] WHERE Name = @Name AND OwnerId = @OwnerId", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Name", projectName);
            command.Parameters.AddWithValue("OwnerId", projectId);

            databasehandler.OpenConnection();
            int count = Convert.ToInt32(command.ExecuteScalar());
            databasehandler.CloseConnection();

            if (count >= 1 && editProject == false)
                return true;
            else
                return false;
        }

        public void DeleteProject(Guid projectId)
        {
            command = new SqlCommand("DELETE FROM [Projects] WHERE Id = @ID", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", projectId);

            databasehandler.OpenConnection();
            command.ExecuteNonQuery();
            databasehandler.CloseConnection();
        }

        public DataTable GetProjectDetails(Guid projectId)
        {
            command = new SqlCommand("SELECT * FROM [Projects] WHERE Id = @ID", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", projectId);

            databasehandler.OpenConnection();

            SqlDataAdapter adapt = new SqlDataAdapter(command);
            adapt.Fill(table);

            databasehandler.CloseConnection();

            return table;
        }
    }
}