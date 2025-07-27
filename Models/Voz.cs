using RailConnect.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class Voz
    {
        [Key]
        public int IdVoz { get; set; }

        [EnumDataType(typeof(VrstaVoza))] 
        public VrstaVoza Vrsta { get; set; }
        [Required]
        [Range(20, 400, ErrorMessage = "Kapacitet sjedecih mjesta mora biti u opsegu 20 do 400")]
        [DisplayName("Broj sjedecih mjesta:")]
        public int Kapacitet { get; set; }
        public Voz() { }
    }
}
