using LogicaHeladeria.Data;
using LibreriaDeClases.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Heladeria.Servicios
{
    public class RolService
    {
        private readonly HeladeriaDbContext _context;

        public RolService(HeladeriaDbContext context)
        {
            _context = context;
        }

        // Verifica si un usuario es Administrador activo
        public async Task<bool> EsAdministrador(string email)
        {
            if (string.IsNullOrEmpty(email))
                return false;

            var usuario = await _context.Usuarios
                .Include(u => u.RolesUsuarios)
                    .ThenInclude(ru => ru.Rol)
                .FirstOrDefaultAsync(u => u.Email == email);

            if (usuario == null)
                return false;

            // Busca es admin activo
            return usuario.RolesUsuarios.Any(ru => 
                ru.Rol.Descripcion == "Administrador" && 
                ru.FechaBaja == null);
        }

        // recibo el email del usuario y busco en la base si tiene rol Administrador activo (revisa fechabaja), returna booleano
    }
}

