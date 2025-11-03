using LogicaHeladeria.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

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

    // Mapear el email del usuario
    options.ClaimActions.MapJsonKey(System.Security.Claims.ClaimTypes.Email, "email", "string");

    options.SaveTokens = true;
});


// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<HeladeriaDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar el servicio de roles
builder.Services.AddScoped<Heladeria.Servicios.RolService>();

// Configurar localización por defecto (forzar cultura es-AR)
var defaultCulture = new CultureInfo("es-AR");
var supportedCultures = new List<CultureInfo> { defaultCulture, new CultureInfo("es-ES") };
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture(defaultCulture);
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
    options.RequestCultureProviders.Clear();
});

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

// Aplicar localización (antes del routing)
var localizationOptions = app.Services.GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(localizationOptions);

app.UseRouting();

// Autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

// Middleware para registrar usuarios automáticamente
app.UseMiddleware<Heladeria.Middleware.RegistroUsuarioMiddleware>();

// Inicializar roles automáticamente
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<HeladeriaDbContext>();
    InicializarRoles(context);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

// Método para inicializar roles automáticamente
static void InicializarRoles(HeladeriaDbContext context)
{
    // Verificar si ya existen roles
    if (!context.Roles.Any())
    {
        // Crear rol Usuario
        var rolUsuario = new LibreriaDeClases.Modelos.Rol
        {
            Descripcion = "Usuario"
        };
        context.Roles.Add(rolUsuario);

        // Crear rol Administrador
        var rolAdministrador = new LibreriaDeClases.Modelos.Rol
        {
            Descripcion = "Administrador"
        };
        context.Roles.Add(rolAdministrador);

        context.SaveChanges();
    }
}
