using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data.Entities;

namespace Soccer.Web.Data
{
    public class DataContext : DbContext
    {

        #region Constructor
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }
        #endregion

        public DbSet<LeagueEntity> Leagues { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }
    }
}