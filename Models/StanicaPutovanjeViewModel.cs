using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class StanicaPutovanjeViewModel
    {
        [Key]
        public int IdStanicaPutovanje { get; set; }

        [Display(Name = "Stanica Dolazak")]
        public int IdStanicaDolazak { get; set; }

        [Display(Name = "Putovanje")]
        public int IdPutovanje { get; set; }

        [Display(Name = "Naziv Stanice Dolaska")]
        public string? StanicaDolazakNaziv { get; set; }

        public StanicaPutovanjeViewModel() { }
    }
}
