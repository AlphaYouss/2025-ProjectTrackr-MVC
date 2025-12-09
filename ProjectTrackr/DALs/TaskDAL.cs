using Microsoft.Data.SqlClient;
using ProjectTrackr.Models;
using System.Data;
using ITaskItem = ProjectTrackr.Interfaces.ITaskItem;

namespace ProjectTrackr.DALs
{
    public class TaskItemDAL : ITaskItem
    {
        private DatabaseHandler databasehandler { get; set; }

        private SqlCommand command { get; set; }
        private DataTable table { get; set; }

        public TaskItemDAL(IConfiguration configuration)
        {

            databasehandler = new DatabaseHandler(configuration);
            databasehandler.TestConnection();

            command = new SqlCommand();
            table = new DataTable();
        }

        public Guid CreateTaskItem(TaskItem item)
        {
            command = new SqlCommand("INSERT INTO [Tasks] (Id, Title, Status, Description, CreatedAt, ProjectId) VALUES (@Id, @Title, @Status, @Description, @CreatedAt, @ProjectId)", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Id", item.id);
            command.Parameters.AddWithValue("Title", item.title);
            command.Parameters.AddWithValue("Status", (int)item.status);
            command.Parameters.AddWithValue("Description", item.description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("CreatedAt", item.createdAt);
            command.Parameters.AddWithValue("ProjectId", item.projectId);

            databasehandler.OpenConnection();
            command.ExecuteNonQuery();
            databasehandler.CloseConnection();

            return item.id;
        }

        public void EditTask(TaskItem item)
        {
            command = new SqlCommand("UPDATE [Tasks] SET Title = @Title, Description = @Description, Status = @Status WHERE Id = @Id", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Title", item.title);
            command.Parameters.AddWithValue("Description", item.description ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("Status", (int)item.status);
            command.Parameters.AddWithValue("Id", item.id);

            databasehandler.OpenConnection();
            command.ExecuteNonQuery();
            databasehandler.CloseConnection();
        }

        public DataTable GetTaskDetails(Guid itemId)
        {
            command = new SqlCommand("SELECT * FROM [Tasks] WHERE Id = @Id", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Id", itemId);

            databasehandler.OpenConnection();

            SqlDataAdapter adapt = new(command);
            adapt.Fill(table);

            databasehandler.CloseConnection();

            return table;
        }

        public DataTable GetTasks(Guid projectId)
        {
            command = new SqlCommand("SELECT * FROM [Tasks] WHERE ProjectId = @ProjectId ORDER BY CreatedAt DESC", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ProjectId", projectId);

            databasehandler.OpenConnection();

            SqlDataAdapter adapt = new(command);
            adapt.Fill(table);

            databasehandler.CloseConnection();

            return table;
        }

        public bool TaskExists(string taskName)
        {
            command = new SqlCommand("SELECT COUNT(*) FROM [Tasks] WHERE Title = @Title", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Title", taskName);

            databasehandler.OpenConnection();
            int count = Convert.ToInt32(command.ExecuteScalar());
            databasehandler.CloseConnection();

            if (count > 0)
                return true;
            else
                return false;
        }
    }
}