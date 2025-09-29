using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibreriaDeClases.Modelos
{

    [Table("Helados")]
    public class Helado
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdHelado { get; set; }
        
        [Required(ErrorMessage = "El sabor es obligatorio")]
        [StringLength(50, ErrorMessage = "El sabor no puede exceder los 50 caracteres")]
        [Display(Name = "Nombre del Helado")]
        public string Sabor { get; set; } = string.Empty;

        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        [Display(Name = "Descripción")]
        public string Descripcion { get; set; } = string.Empty;

        [Required(ErrorMessage = "El precio es obligatorio")]
        [Range(0.01, 100.00, ErrorMessage = "El precio debe estar entre $0.01 y $100.00")]
        [Display(Name = "Precio")]
        [DataType(DataType.Currency)]
        public decimal Precio { get; set; }

        [Required(ErrorMessage = "La cantidad en stock es obligatoria")]
        [Range(0, 1000, ErrorMessage = "La cantidad debe estar entre 0 y 1000")]
        [Display(Name = "Cantidad en Stock")]
        public int CantidadEnStock { get; set; }

        public int IdCategoria { get; set; }


        [Display(Name = "Fecha de Creación")]
        [DataType(DataType.DateTime)]
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        
        public Categoria? Categoria { get; set; }

    }
}
