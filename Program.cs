using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ExcelManipulation.Data;
using ExcelManipulation.Services;
using ExcelManipulation.Enums;
using System.Net;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions{
    Args = args, 

});
// var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection") ?? throw new InvalidOperationException("Connection string 'AppDbContextConnection' not found.");

// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(connectionString));;

// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//     .AddEntityFrameworkStores<AppDbContext>();;

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("HerokuServer");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IEmployeeDBService, EmployeeDBService>();
builder.Services.AddScoped<IDataManipulationService, DataManipulationService>();
var port = Environment.GetEnvironmentVariable("PORT");
// builder.WebHost.UseUrls("http://*:" + port);
builder.WebHost.UseKestrel(serverOptions => {
    serverOptions.Listen(IPAddress.Any, Convert.ToInt32(port));
});
// builder.Host.ConfigureWebHostDefaults(webBuilder => {
//     webBuilder.UseUrls("http://*:" + port);
// });


var app = builder.Build();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
await SeedDatabase();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Employee}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

async Task SeedDatabase() 
{
    using (var scope = app.Services.CreateScope())
    {
        var _roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        if (!await _roleManager.RoleExistsAsync(Roles.Admin.ToString()))
        {
            await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
            await _roleManager.CreateAsync(new IdentityRole(Roles.Default.ToString()));
        }
    }
}
