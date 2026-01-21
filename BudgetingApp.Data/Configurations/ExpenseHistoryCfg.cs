using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetingApp.Data.Configurations
{
    public class ExpenseHistoryCfg : IEntityTypeConfiguration<ExpenseHistory>
    {
        public void Configure(EntityTypeBuilder<ExpenseHistory> builder)
        {
            builder.HasKey(x => x.ExpenseHistoryId);

            builder.Property(x => x.Cost).IsRequired();

            builder.Property(x => x.ChangedDate).IsRequired();

            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasOne(x => x.Expense)
                .WithMany(x => x.ExpenseHistory)
                .HasForeignKey(x => x.ExpenseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ExpenseId, x.ChangedDate });
        }
    }
}