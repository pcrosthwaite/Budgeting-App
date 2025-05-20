using BudgetingApp.Data.Configurations;
using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace BudgetingApp.Data
{
    public class BudgetingDbContext : DbContext
    {
        public BudgetingDbContext(DbContextOptions<BudgetingDbContext> options) : base(options)
        {
            ChangeTracker.AutoDetectChangesEnabled = false;
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.OnBeforeSaving();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public Task<int> SaveChangesAsync()
        {
            return SaveChangesAsync(true);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            this.OnBeforeSaving();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public DbSet<Expense> Expenses { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<PersonExpense> PersonExpenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddIsDeleted();

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new ExpenseCfg());
            modelBuilder.ApplyConfiguration(new PersonCfg());
            modelBuilder.ApplyConfiguration(new PersonExpenseCfg());
        }
    }
}