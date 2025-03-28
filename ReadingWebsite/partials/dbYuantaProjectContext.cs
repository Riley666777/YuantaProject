using Microsoft.EntityFrameworkCore;

namespace ReadingWebsite.Models
{
    public partial class dbYuantaProjectContext : DbContext
    {
        public dbYuantaProjectContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfiguration Config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
                optionsBuilder.UseSqlServer(Config.GetConnectionString("dbYuantaProject"));
            }
        }
    }
}
