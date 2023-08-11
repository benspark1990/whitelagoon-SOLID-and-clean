using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Syncfusion.Licensing;
using WhiteLagoon.Application.Common.Interfaces;
using WhiteLagoon.Application.Services.Implementations;
using WhiteLagoon.Application.Services.Interfaces;
using WhiteLagoon.Domain.Entities;
using WhiteLagoon.Infrastructure.Data;
using WhiteLagoon.Infrastructure.Repository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<WhiteLagoon.Application.Services.Interfaces.IVillaService, WhiteLagoon.Application.Services.Implementations.VillaService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IAmenityService, AmenityService>();
builder.Services.AddScoped<WhiteLagoon.Application.Services.Interfaces.IVillaNumberService, WhiteLagoon.Application.Services.Implementations.VillaNumberService>();

#region SOLID

builder.Services.AddScoped<WhiteLagoon.Application.Services.SOLID.S.Interfaces.IVillaService, WhiteLagoon.Application.Services.SOLID.S.Implementations.VillaService>();
builder.Services.AddScoped<WhiteLagoon.Application.Services.SOLID.S.Interfaces.IImageService, WhiteLagoon.Application.Services.SOLID.S.Implementations.ImageService>();
builder.Services.AddScoped<WhiteLagoon.Application.Services.SOLID.O.Interfaces.IVillaNumberService, WhiteLagoon.Application.Services.SOLID.O.Implementations.VillaNumberService>();
builder.Services.AddScoped<WhiteLagoon.Application.Services.SOLID.O.Interfaces.IVillaNumberRepository, WhiteLagoon.Application.Services.SOLID.O.Implementations.VillaNumberRepository>();
builder.Services.AddScoped<WhiteLagoon.Application.Services.SOLID.L.Interfaces.IAmenityService, WhiteLagoon.Application.Services.SOLID.L.Implementations.AmenityService>();
builder.Services.AddScoped<WhiteLagoon.Application.Services.SOLID.I.Interfaces.IDashboardService, WhiteLagoon.Application.Services.SOLID.I.Implementations.DashboardService>();
builder.Services.AddScoped<WhiteLagoon.Application.Services.SOLID.D.Interfaces.IBookingService, WhiteLagoon.Application.Services.SOLID.D.Implementations.BookingService>();


#endregion

var app = builder.Build();

StripeConfiguration.ApiKey = builder.Configuration.GetSection("Stripe:SecretKey").Get<string>();
SyncfusionLicenseProvider.RegisterLicense(builder.Configuration.GetSection("Syncfusion:Licensekey").Get<string>());

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
