using LiftAndAesthetics.Models;
using Microsoft.EntityFrameworkCore;
using LiftAndAesthetics.Repositories.Interfaces;
using LiftAndAesthetics.Repositories;
using LiftAndAesthetics.Services.Interfaces;
using LiftAndAesthetics.Services;
using Microsoft.AspNetCore.Identity;
using System; 

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddDbContext<LiftContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); 
    options.Lockout.MaxFailedAccessAttempts = 6;                       
    options.Lockout.AllowedForNewUsers = true;

})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<LiftContext>();

// DEPENDENCY INJECTION: REPOSITORY WRAPPER 
builder.Services.AddScoped<IRepositoryWrapper, RepositoryWrapper>();

// DEPENDENCY INJECTION: SERVICES
builder.Services.AddScoped<ITrainerService, TrainerService>();
builder.Services.AddScoped<IFitnessClassService, FitnessClassService>();
builder.Services.AddScoped<IMembershipService, MembershipService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IContactMessageService, ContactMessageService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMemberService, MemberService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IClassReservationService, ClassReservationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

var app = builder.Build();

app.UseStatusCodePagesWithReExecute("/Error/{0}");
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<LiftContext>();
        await context.Database.MigrateAsync();
        await LiftAndAesthetics.Data.DbSeeder.SeedAsync(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error on migrating or seeding the database : {ex.Message}");
    }
}

app.Run();