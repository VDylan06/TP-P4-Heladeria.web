using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaDeClases.Modelos
{
    public class Rol
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRol { get; set; }

        [Required]
        [StringLength(100)]
        public string Descripcion { get; set; } = string.Empty;

        public ICollection<RolUsuario> RolesUsuarios { get; set; } = new List<RolUsuario>();
    }
}

