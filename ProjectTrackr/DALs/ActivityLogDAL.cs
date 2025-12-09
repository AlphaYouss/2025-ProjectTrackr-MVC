using Microsoft.Data.SqlClient;
using ProjectTrackr.Interfaces;
using ProjectTrackr.Models;
using System.Data;

namespace ProjectTrackr.DALs
{
    public class ActivityLogDAL : IActivityLog
    {
        private DatabaseHandler databasehandler { get; set; }

        private SqlCommand command { get; set; }
        private DataTable table { get; set; }

        public ActivityLogDAL(IConfiguration configuration)
        {

            databasehandler = new DatabaseHandler(configuration);
            databasehandler.TestConnection();

            command = new SqlCommand();
            table = new DataTable();
        }

        public void CreateActivityLog(ActivityLog activityLog)
        {
            command = new SqlCommand("INSERT INTO [ActivityLogs] VALUES(@Id, @Action, @CreatedAt, @UserId, @ProjectId, @TaskId)", databasehandler.GetConnection());
            command.Parameters.AddWithValue("Id", activityLog.id);
            command.Parameters.AddWithValue("Action", activityLog.action);
            command.Parameters.AddWithValue("CreatedAt", activityLog.createdAt);
            command.Parameters.AddWithValue("UserId", activityLog.userId);
            command.Parameters.AddWithValue("ProjectId", activityLog.projectId ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("TaskId", activityLog.taskId ?? (object)DBNull.Value);


            databasehandler.OpenConnection();
            command.ExecuteNonQuery();
            databasehandler.CloseConnection();
        }

        public DataTable GetActivityLogsByProjectId(Guid projectId)
        {
            command = new SqlCommand("SELECT TOP 5 * FROM [ActivityLogs] WHERE ProjectId = @ID ORDER BY CreatedAt DESC", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", projectId);

            databasehandler.OpenConnection();

            SqlDataAdapter adapt = new SqlDataAdapter(command);
            adapt.Fill(table);

            databasehandler.CloseConnection();

            return table;
        }

        public void DeleteActivityLogByProjectId(Guid projectId)
        {
            command = new SqlCommand("DELETE FROM [ActivityLogs] WHERE ProjectId = @ID", databasehandler.GetConnection());
            command.Parameters.AddWithValue("ID", projectId);

            databasehandler.OpenConnection();
            command.ExecuteNonQuery();
            databasehandler.CloseConnection();
        }
    }
}