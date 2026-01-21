using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetingApp.Data.Configurations
{
    public class BankCfg : IEntityTypeConfiguration<Bank>
    {
        public void Configure(EntityTypeBuilder<Bank> builder)
        {
            builder.Property(ec => ec.Name).IsRequired().HasMaxLength(100);
        }
    }
}