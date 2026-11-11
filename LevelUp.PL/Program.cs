using FluentValidation.AspNetCore;
using LevelUp.BLL.Extentions.DependencyEnjection;
using LevelUp.DAL.Applications;
using LevelUp.DAL.Contexts;
using LevelUp.DAL.Extentions.DependencyEnjection;
using LevelUp.PL.Extentions;
using LevelUp.PL.Extentions.Logging;
using LevelUp.PL.Filters;
using Microsoft.AspNetCore.Identity;

// Add services to the container.
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// Secrets that are not in Git (admin seed password)
builder.Configuration.AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: false);

// Logging
builder.AddSerilogLogging();

// MVC
builder.Services.AddControllersWithViews(options =>
{
    // Validation messages come from FluentValidation (Arabic), not the implicit English [Required]
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    options.Filters.Add<RunningCountFilter>();
});

// Fluent Validation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

// Layers
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddBusinessLogicLayer();

// Identity (login by UserName)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(config =>
{
    config.User.RequireUniqueEmail = false;
    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(2);
    config.Lockout.MaxFailedAccessAttempts = 5;
}).AddEntityFrameworkStores<LevelUpDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/Login";
});

var app = builder.Build();

await app.SeedDataAndApplyPendingMigrations();

// Configure the HTTP request pipeline (HTTP only: no HTTPS redirection, no HSTS).
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// "/" opens the login page
app.MapControllerRoute(
    name: "root",
    pattern: "",
    defaults: new { controller = "Account", action = "Login" });

// "/Room" = Room/Index, "/Drink" = Drink/Index, ...
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.OpenBrowserOnStart();

app.Run();
