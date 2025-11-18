namespace Infrastructure.Persistence
{
    public static class DbContextHelper
    {
        public static string GetConnectionString()
        {
            string? connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "CONNECTION_STRING environment variable not found. Ensure .env file is loaded with DotNetEnv.Env.Load()");
            }

            return connectionString;
        }
    }
}
