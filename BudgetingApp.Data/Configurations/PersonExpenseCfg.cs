using BudgetingApp.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BudgetingApp.Data.Configurations
{
    public class PersonExpenseCfg : IEntityTypeConfiguration<PersonExpense>
    {
        public void Configure(EntityTypeBuilder<PersonExpense> builder)
        {
            // Configure many-to-many relationship
            builder.HasKey(pe => new { pe.PersonId, pe.ExpenseId });

            builder
                .HasOne(pe => pe.Person)
                .WithMany(p => p.PersonExpenses)
                .HasForeignKey(pe => pe.PersonId);

            builder
                .HasOne(pe => pe.Expense)
                .WithMany(e => e.PersonExpenses)
                .HasForeignKey(pe => pe.ExpenseId);
        }
    }
}