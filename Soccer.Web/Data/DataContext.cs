using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Soccer.Web.Data.Entities;

namespace Soccer.Web.Data
{
    public class DataContext : IdentityDbContext<User>
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
        public DbSet<PredictionEntity> Predictions { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Manager> Managers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<LeagueEntity>()
                .HasIndex(t => t.Name)
                .IsUnique();
        }

    }
}