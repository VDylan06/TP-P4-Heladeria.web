using LogicaHeladeria.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);



// Habilita servicios de autenticación
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
.AddCookie()
.AddGoogle(options =>
{
    options.ClientId = "868984785169-j3hmr3l019mg5fhf9ghvu1t2rpdjult1.apps.googleusercontent.com";
    options.ClientSecret = "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";

    options.ClaimActions.MapJsonKey("urn:google:picture", "picture", "url");
    options.ClaimActions.MapJsonKey("urn:google:locale", "locale", "string");

    options.SaveTokens = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HeladeriaDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();


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

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
