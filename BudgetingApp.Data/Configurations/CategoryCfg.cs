using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetingApp.Data.Configurations
{
    public class CategoryCfg : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.Property(ec => ec.Name).IsRequired().HasMaxLength(100);
            builder.Property(ec => ec.HexColourCode).HasMaxLength(9);
        }
    }
}