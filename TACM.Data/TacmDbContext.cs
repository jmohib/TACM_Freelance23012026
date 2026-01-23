using Microsoft.EntityFrameworkCore;
using TACM.Data.EntitiesConfigurations;
using TACM.Entities;

namespace TACM.Data;

public class TacmDbContext : DbContext
{
    public TacmDbContext(DbContextOptions<TacmDbContext> options) : base(options) { }

    public DbSet<Settings> Settings { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<TestResultItem> TestResultItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .ApplyConfiguration(new SettingsEntityConfiguration())
            .ApplyConfiguration(new SessionEntityConfiguration())
            .ApplyConfiguration(new TestResultEntityConfiguration())
            .ApplyConfiguration(new TestResultItemEntityConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}
