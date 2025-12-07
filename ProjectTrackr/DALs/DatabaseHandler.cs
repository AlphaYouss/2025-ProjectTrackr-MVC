using Microsoft.Data.SqlClient;

namespace ProjectTrackr.DALs
{
    public class DatabaseHandler
    {
        private readonly IConfiguration configuration;
        private SqlConnection connection;

        public DatabaseHandler(IConfiguration configuration)
        {
            this.configuration = configuration;
            connection = new SqlConnection(configuration.GetConnectionString("DBConnectionString"));
        }

        public bool TestConnection()
        {
            try
            {
                connection.Open();
                connection.Close();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
        }

        public void OpenConnection()
        {
            if (connection.State != System.Data.ConnectionState.Open)
            {
                connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (connection.State != System.Data.ConnectionState.Closed)
            {
                connection.Close();
            }
        }

        public SqlConnection GetConnection()
        {
            return connection;
        }
    }
}