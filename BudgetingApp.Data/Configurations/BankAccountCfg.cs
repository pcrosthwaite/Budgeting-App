using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetingApp.Data.Configurations
{
    public class BankAccountCfg : IEntityTypeConfiguration<BankAccount>
    {
        public void Configure(EntityTypeBuilder<BankAccount> builder)
        {
            builder.Property(ec => ec.Name).IsRequired().HasMaxLength(100);
            builder.Property(ec => ec.AccountNumber).IsRequired().HasMaxLength(100);

            builder
                .HasOne(x => x.Bank)
                .WithMany()
                .HasForeignKey(x => x.BankId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}