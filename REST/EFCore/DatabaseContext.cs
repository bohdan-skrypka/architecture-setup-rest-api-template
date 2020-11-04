using Database.Context.DataContracts.Entities;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace EFCoreProvider
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Owner> Owners { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
           // ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var property in modelBuilder.Model.GetEntityTypes().SelectMany(t => t.GetProperties()).Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
            {
                // property.SetColumnType("decimal(18, 6)");
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
