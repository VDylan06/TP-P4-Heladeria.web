using LogicaHeladeria.Data;
using LibreriaDeClases.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Heladeria.Middleware
{
    public class RegistroUsuarioMiddleware
    {
        private readonly RequestDelegate _next;

        public RegistroUsuarioMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, HeladeriaDbContext dbContext)
        {
            // verifico si el usuario está autenticado con google
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var email = context.User.FindFirst(ClaimTypes.Email)?.Value
                    ?? context.User.FindFirst("email")?.Value;

                if (!string.IsNullOrEmpty(email))
                {
                    await RegistrarUsuarioSiNoExiste(email, dbContext);
                }
            }

            await _next(context);
        }


        //acá realizo lógica para registrar usuario en la bdd.
        private static async Task RegistrarUsuarioSiNoExiste(string email, HeladeriaDbContext context)
        {
            var usuario = await context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

                //Si no existe creo uno nuevo y le asigno el rol Usuario para que pueda leer todo mas no interactuar. primero lo agrego a tabla usuarios y luego a rolesusuarios
            if (usuario == null)
            {
                usuario = new Usuario
                {
                    Email = email
                };
                context.Usuarios.Add(usuario);
                await context.SaveChangesAsync();
                
                var rolUsuario = await context.Roles.FirstOrDefaultAsync(r => r.Descripcion == "Usuario");

                if (rolUsuario != null)
                {
                    var rolUsuarioAsignado = new RolUsuario
                    {
                        IdUsuario = usuario.IdUsuario,
                        IdRol = rolUsuario.IdRol,
                        FechaBaja = null
                    };
                    context.RolesUsuarios.Add(rolUsuarioAsignado);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}

