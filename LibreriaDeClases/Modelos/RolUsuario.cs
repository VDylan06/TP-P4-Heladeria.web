using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaDeClases.Modelos
{
    public class RolUsuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdRolUsuario { get; set; }

        public int IdUsuario { get; set; }

        public int IdRol { get; set; }

        [Column(TypeName = "date")]
        public DateTime? FechaBaja { get; set; }

        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; } = null!;

        [ForeignKey("IdRol")]
        public Rol Rol { get; set; } = null!;
    }
}

