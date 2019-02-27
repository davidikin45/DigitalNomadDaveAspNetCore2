namespace AspNetCore.Base.Helpers
{
    public class ConnectionStringHelper
    {
        public static bool IsSQLite(string connectionString)
        {
            return connectionString.ToLower().Contains(".sqlite")
                || connectionString.ToLower().Contains(".db")
                || connectionString.ToLower().Contains(":memory:");
        }
    }
}
