using BTL_Web.Models;
using BTL_Web.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add DbContext
var defaultConnection = ResolveConnectionString(builder);

builder.Services.AddDbContext<TtanContext>(options =>
    options.UseSqlServer(defaultConnection));

// Temporary compatibility registration: some services/controllers still inject TtamContext.
builder.Services.AddDbContext<TtamContext>(options =>
    options.UseSqlServer(defaultConnection));

// Add custom services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserInfoService, UserInfoService>();

// Add Authentication
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireLogin", policy =>
        policy.RequireAuthenticatedUser());
    
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
    
    options.AddPolicy("GiaoVienOrAdmin", policy =>
        policy.RequireRole("GiaoVien", "Admin"));
    
    options.AddPolicy("HocVienOrAdmin", policy =>
        policy.RequireRole("HocVien", "Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();

static string ResolveConnectionString(WebApplicationBuilder builder)
{
    var configured = builder.Configuration.GetConnectionString("DefaultConnection");

    if (!builder.Environment.IsDevelopment())
    {
        return configured ?? throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");
    }

    var candidates = new[]
    {
        configured,
        "Server=.;Database=TTAN;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;",
        "Server=localhost;Database=TTAN;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;",
        "Server=DESKTOP-I8D5TII;Database=TTAN;Trusted_Connection=True;Encrypt=False;TrustServerCertificate=True;"
    }
    .Where(s => !string.IsNullOrWhiteSpace(s))
    .Distinct(StringComparer.OrdinalIgnoreCase);

    foreach (var candidate in candidates)
    {
        try
        {
            using var connection = new SqlConnection(candidate!);
            connection.Open();
            return candidate!;
        }
        catch
        {
            // Try next candidate.
        }
    }

    return configured ?? throw new InvalidOperationException("No usable SQL Server connection string was found for Development.");
}
