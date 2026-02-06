using ApexCharts;
using BudgetingApp.Data;
using BudgetingApp.Data.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MudBlazor.Services;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
IConfiguration Configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddRazorPages().WithRazorPagesRoot("/Features");
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<BudgetService>();
builder.Services.AddScoped<ExpenseService>();
builder.Services.AddScoped<PersonService>();
builder.Services.AddMudServices();
builder.Services.AddMudBlazorDialog(); // Add MudBlazor dialog service

// Add DbContext with SQLite configuration
services.AddDbContexts<BudgetingDbContext>(Configuration, "DefaultConnection", Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient); //Transient for Blazor
                                                                                                                                                    //services.AddDbContexts<MariaDbContext>(Configuration, "MariaDbConnection", Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient);

// Add MariaDB DbContext

//var mariaDbConnectionString = Configuration.GetConnectionString("MariaDbConnection");
//services.AddDbContext<MariaDbContext>(options =>
//{
//    var serverVersion = ServerVersion.AutoDetect(mariaDbConnectionString);
//    options.UseMySql(mariaDbConnectionString, serverVersion, o => o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery))
//           .EnableDetailedErrors();
//#if DEBUG
//    options.EnableSensitiveDataLogging();
//#endif
//}, contextLifetime: ServiceLifetime.Transient, optionsLifetime: ServiceLifetime.Singleton);

var assemblies = new List<Assembly>();
//var hfAssemblies = AssemblyHelper.GetAssemblies("vDia.Hangfire").ToList();

// Add the main project's assembly
assemblies.Add(typeof(Program).Assembly);
//assemblies.AddRange(hfAssemblies);

services.AddMapperly(assemblies.ToArray());

services.AddApexCharts(e =>
{
    e.GlobalOptions = new ApexChartBaseOptions
    {
        Debug = true,
        Theme = new Theme { Palette = PaletteType.Palette6 }
    };
});

// Register MediatR
services.AddMediatR(typeof(Program).Assembly);

var app = builder.Build();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BudgetingDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();