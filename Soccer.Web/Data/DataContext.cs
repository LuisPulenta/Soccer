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

        public DbSet<DateNameEntity> DateNames { get; set; }
        public DbSet<GroupDetailEntity> GroupDetails { get; set; }
        public DbSet<GroupEntity> Groups { get; set; }
        public DbSet<LeagueEntity> Leagues { get; set; }
        public DbSet<MatchEntity> Matches { get; set; }
        public DbSet<TeamEntity> Teams { get; set; }
        public DbSet<TournamentEntity> Tournaments { get; set; }
    }
}