namespace Infrastructure.Persistence
{
    public static class DbContextHelper
    {
        public static string GetConnectionString()
        {
            DotNetEnv.Env.Load(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

            return Environment.GetEnvironmentVariable("CONNECTION_STRING");
        }
    }
}
