using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaDeClases.Modelos
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;

        public ICollection<RolUsuario> RolesUsuarios { get; set; } = new List<RolUsuario>();
    }
}

