using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class KartaViewModel
    {
        public int IdKarta { get; set; }
        [DisplayName("ID Polazne stanice:")]
        public int IdStanicaPolazak { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Naziv smije imati između 3 i 50 karaktera!")]
        [DisplayName("Polazna stanica:")]
        public string StanicaPolazakNaziv { get; set; }
        [Required]
        [DisplayName("ID Dolazne stanice:")]
        public int IdStanicaDolazak { get; set; }
        [DisplayName("Dolazna stanica:")]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Naziv smije imati između 3 i 50 karaktera!")]

        public string StanicaDolazakNaziv { get; set; }
        [Required]
        [DisplayName("Putnik:")]
        public string PutnikId { get; set; }
        [DisplayName("ID putovanja:")]
        public int PutovanjeId { get; set; }
        [DataType(DataType.Date)]

        public DateTime DatumPutovanja { get; set; }
        public NacinPlacanja NacinPlacanja { get; set; }
    }

}
