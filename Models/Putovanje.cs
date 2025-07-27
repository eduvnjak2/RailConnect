using RailConnect.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailConnect.Models
{
    public class Putovanje
    {
        [Key]
        public int IdPutovanje { get; set; }
        [ForeignKey("Voz")]
        [Required]

        public int IdVoz { get; set; }
        [Required]
        [ForeignKey("StanicaPolazak")]
        [DisplayName("Mjesto polaska:")]
        public int MjestoPolaska { get; set; }
        [Required]
        [ForeignKey("StanicaDolazak")]
        [DisplayName("Mjesto dolaska:")]
        public int MjestoDolaska { get; set; }

        [Required]
        [Range(1.0, 30.0, ErrorMessage = "Cijena mora biti u opsegu 1 do 30")] 
        [DisplayName("Cijena:")]
        public double Cijena { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [DisplayName("Vrijeme polaska:")]
        public DateTime VrijemePolaska { get; set; }
        [Required]
        [DataType(DataType.Time)]
        [DisplayName("Vrijeme dolaska:")]
        public DateTime VrijemeDolaska { get; set; }
        [ValidateDate]
        [Required]
        [DataType(DataType.Date)]
        [DisplayName("Datum polaska:")]
        public DateTime DatumPolaska { get; set; }
        [Required]
        [Range(20, 400, ErrorMessage = "Broj sjedecih mjesta mora biti u opsegu 20 do 400")]
        [DisplayName("Broj sjedecih mjesta:")]
        public int BrojMjesta { get; set; }
        public Putovanje() { }
    }
}
