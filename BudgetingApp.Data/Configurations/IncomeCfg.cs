using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetingApp.Data.Configurations
{
    public class IncomeCfg : IEntityTypeConfiguration<Income>
    {
        public void Configure(EntityTypeBuilder<Income> builder)
        {
            builder.HasOne(x => x.Person)
                .WithOne(x => x.Income)
                .HasForeignKey<Income>(x => x.PersonId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasOne(x => x.IncomeCategory)
                .WithMany(x => x.Income)
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}