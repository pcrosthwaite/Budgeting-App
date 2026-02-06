using BudgetingApp.Data;
using BudgetingApp.Web.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MudBlazor;

namespace BudgetingApp.Web.Features.Tools.ExportToMariaDb
{
    //public class IndexQuery : IRequest<IndexModel>
    //{
    //    public bool CreateTables { get; set; } = true;
    //    public bool ClearExistingData { get; set; } = false;
    //}

    //public class IndexModel
    //{
    //    public bool IsSuccess { get; set; }
    //    public List<AlertModel> Alerts { get; set; } = new();
    //    public ExportStats Stats { get; set; } = new();
    //}

    //public class ExportStats
    //{
    //    public int BanksExported { get; set; }
    //    public int BankAccountsExported { get; set; }
    //    public int CategoriesExported { get; set; }
    //    public int PersonsExported { get; set; }
    //    public int IncomesExported { get; set; }
    //    public int ExpensesExported { get; set; }
    //    public int PersonExpensesExported { get; set; }
    //    public int ExpenseHistoriesExported { get; set; }

    //    public int TotalRecords => BanksExported + BankAccountsExported + CategoriesExported + 
    //        PersonsExported + IncomesExported + ExpensesExported + PersonExpensesExported + ExpenseHistoriesExported;
    //}

    //public class IndexHandler : IRequestHandler<IndexQuery, IndexModel>
    //{
        //private readonly BudgetingDbContext _sqliteContext;
        //private readonly MariaDbContext _mariaDbContext;

        //public IndexHandler(BudgetingDbContext sqliteContext, MariaDbContext mariaDbContext)
        //{
        //    _sqliteContext = sqliteContext;
        //    _mariaDbContext = mariaDbContext;
        //}

        //public async Task<IndexModel> Handle(IndexQuery request, CancellationToken cancellationToken)
        //{
        //    var model = new IndexModel();

        //    try
        //    {
        //        // Test connection
        //        model.Alerts.Add(new AlertModel 
        //        { 
        //            Severity = Severity.Info, 
        //            Message = "Testing connection to MariaDB..." 
        //        });

        //        if (!await _mariaDbContext.Database.CanConnectAsync(cancellationToken))
        //        {
        //            model.Alerts.Add(new AlertModel 
        //            { 
        //                Severity = Severity.Error, 
        //                Message = "Cannot connect to MariaDB. Please check your connection string in appsettings.json." 
        //            });
        //            return model;
        //        }

        //        model.Alerts.Add(new AlertModel 
        //        { 
        //            Severity = Severity.Success, 
        //            Message = "Connected to MariaDB successfully." 
        //        });

        //        // Create tables if requested
        //        if (request.CreateTables)
        //        {
        //            model.Alerts.Add(new AlertModel 
        //            { 
        //                Severity = Severity.Info, 
        //                Message = "Creating/migrating database schema..." 
        //            });

        //            await _mariaDbContext.Database.EnsureCreatedAsync(cancellationToken);

        //            model.Alerts.Add(new AlertModel 
        //            { 
        //                Severity = Severity.Success, 
        //                Message = "Database schema created/verified." 
        //            });
        //        }

        //        // Clear existing data if requested
        //        if (request.ClearExistingData)
        //        {
        //            model.Alerts.Add(new AlertModel 
        //            { 
        //                Severity = Severity.Warning, 
        //                Message = "Clearing existing data in MariaDB..." 
        //            });

        //            await ClearExistingDataAsync(cancellationToken);

        //            model.Alerts.Add(new AlertModel 
        //            { 
        //                Severity = Severity.Success, 
        //                Message = "Existing data cleared." 
        //            });
        //        }

        //        // Export data
        //        model.Alerts.Add(new AlertModel 
        //        { 
        //            Severity = Severity.Info, 
        //            Message = "Starting data export..." 
        //        });

        //        model.Stats = await ExportDataAsync(cancellationToken);

        //        model.Alerts.Add(new AlertModel 
        //        { 
        //            Severity = Severity.Success, 
        //            Message = $"Export completed successfully! {model.Stats.TotalRecords} records exported." 
        //        });

        //        model.IsSuccess = true;
        //    }
        //    catch (Exception ex)
        //    {
        //        model.Alerts.Add(new AlertModel 
        //        { 
        //            Severity = Severity.Error, 
        //            Message = $"Export failed: {ex.Message}" 
        //        });

        //        if (ex.InnerException != null)
        //        {
        //            model.Alerts.Add(new AlertModel 
        //            { 
        //                Severity = Severity.Error, 
        //                Message = $"Inner exception: {ex.InnerException.Message}" 
        //            });
        //        }
        //    }

        //    return model;
        //}

        //private async Task ClearExistingDataAsync(CancellationToken cancellationToken)
        //{
        //    // Delete in order to respect foreign key constraints
        //    _mariaDbContext.ChangeTracker.AutoDetectChangesEnabled = true;
            
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0", cancellationToken);
            
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE PersonExpenses", cancellationToken);
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE ExpenseHistories", cancellationToken);
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Expenses", cancellationToken);
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Income", cancellationToken);
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Persons", cancellationToken);
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE BankAccounts", cancellationToken);
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Banks", cancellationToken);
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Categories", cancellationToken);
            
        //    await _mariaDbContext.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1", cancellationToken);
        //}

        //private async Task<ExportStats> ExportDataAsync(CancellationToken cancellationToken)
        //{
        //    var stats = new ExportStats();
        //    _mariaDbContext.ChangeTracker.AutoDetectChangesEnabled = true;

        //    // Export Banks
        //    var banks = await _sqliteContext.Banks.IgnoreQueryFilters().ToListAsync(cancellationToken);
        //    if (banks.Any())
        //    {
        //        await _mariaDbContext.Banks.AddRangeAsync(banks, cancellationToken);
        //        await _mariaDbContext.SaveChangesAsync(cancellationToken);
        //        stats.BanksExported = banks.Count;
        //        _mariaDbContext.ChangeTracker.Clear();
        //    }

        //    // Export Categories  
        //    var categories = await _sqliteContext.Categories.IgnoreQueryFilters().ToListAsync(cancellationToken);
        //    if (categories.Any())
        //    {
        //        await _mariaDbContext.Categories.AddRangeAsync(categories, cancellationToken);
        //        await _mariaDbContext.SaveChangesAsync(cancellationToken);
        //        stats.CategoriesExported = categories.Count;
        //        _mariaDbContext.ChangeTracker.Clear();
        //    }

        //    // Export BankAccounts (depends on Banks)
        //    var bankAccounts = await _sqliteContext.BankAccounts.IgnoreQueryFilters().ToListAsync(cancellationToken);
        //    if (bankAccounts.Any())
        //    {
        //        await _mariaDbContext.BankAccounts.AddRangeAsync(bankAccounts, cancellationToken);
        //        await _mariaDbContext.SaveChangesAsync(cancellationToken);
        //        stats.BankAccountsExported = bankAccounts.Count;
        //        _mariaDbContext.ChangeTracker.Clear();
        //    }

        //    // Export Persons
        //    var persons = await _sqliteContext.Persons.IgnoreQueryFilters().ToListAsync(cancellationToken);
        //    if (persons.Any())
        //    {
        //        await _mariaDbContext.Persons.AddRangeAsync(persons, cancellationToken);
        //        await _mariaDbContext.SaveChangesAsync(cancellationToken);
        //        stats.PersonsExported = persons.Count;
        //        _mariaDbContext.ChangeTracker.Clear();
        //    }

        //    // Export Incomes (depends on Persons and Categories)
        //    var incomes = await _sqliteContext.Income.IgnoreQueryFilters().ToListAsync(cancellationToken);
        //    if (incomes.Any())
        //    {
        //        await _mariaDbContext.Income.AddRangeAsync(incomes, cancellationToken);
        //        await _mariaDbContext.SaveChangesAsync(cancellationToken);
        //        stats.IncomesExported = incomes.Count;
        //        _mariaDbContext.ChangeTracker.Clear();
        //    }

        //    // Export Expenses (depends on BankAccounts and Categories)
        //    var expenses = await _sqliteContext.Expenses.IgnoreQueryFilters().ToListAsync(cancellationToken);
        //    if (expenses.Any())
        //    {
        //        await _mariaDbContext.Expenses.AddRangeAsync(expenses, cancellationToken);
        //        await _mariaDbContext.SaveChangesAsync(cancellationToken);
        //        stats.ExpensesExported = expenses.Count;
        //        _mariaDbContext.ChangeTracker.Clear();
        //    }

        //    // Export PersonExpenses (depends on Persons and Expenses)
        //    var personExpenses = await _sqliteContext.PersonExpenses.IgnoreQueryFilters().ToListAsync(cancellationToken);
        //    if (personExpenses.Any())
        //    {
        //        await _mariaDbContext.PersonExpenses.AddRangeAsync(personExpenses, cancellationToken);
        //        await _mariaDbContext.SaveChangesAsync(cancellationToken);
        //        stats.PersonExpensesExported = personExpenses.Count;
        //        _mariaDbContext.ChangeTracker.Clear();
        //    }

        //    // Export ExpenseHistories (depends on Expenses)
        //    var expenseHistories = await _sqliteContext.ExpenseHistories.IgnoreQueryFilters().ToListAsync(cancellationToken);
        //    if (expenseHistories.Any())
        //    {
        //        await _mariaDbContext.ExpenseHistories.AddRangeAsync(expenseHistories, cancellationToken);
        //        await _mariaDbContext.SaveChangesAsync(cancellationToken);
        //        stats.ExpenseHistoriesExported = expenseHistories.Count;
        //        _mariaDbContext.ChangeTracker.Clear();
        //    }

        //    return stats;
        //}
    //}
}
