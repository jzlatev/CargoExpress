using CargoExpress.Core.Constants;
using CargoExpress.Infrastructure.Data;
using CargoExpress.Infrastructure.Data.Identity;
using CargoExpress.ModelBinders;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Add contexts
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews()
    .AddMvcOptions(option =>
    {
        option.ModelBinderProviders.Insert(0, new DecimalModelBinderProvider());
        //option.ModelBinderProviders.Insert(1, new DateTimeModelBinderProvider(FormatingConstant.BGDateFormat));
        option.ModelBinderProviders.Insert(2, new DoubleModelBinderProvider());
    });

// Add services
builder.Services.AddApplicationService();

var app = builder.Build();

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
    name: "Area",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
