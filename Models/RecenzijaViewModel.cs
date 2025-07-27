using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class RecenzijaViewModel
    {
        public int IdRecencija { get; set; }

        [Display(Name = "Ime")]
        public string? Ime { get; set; }

        [Display(Name = "Prezime")]
        public string? Prezime { get; set; }

        public string? PutnikId { get; set; }

        [Required]
        [DisplayName("Putovanje :")]
        public int PutovanjeId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Ocjena mora biti u opsegu 1 do 5")]
        [DisplayName("Ocjena:")]
        public int Ocjena { get; set; }

        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Komentar smije imati između 0 i 50 karaktera!")]
        //[RegularExpression(@"[a-z|A-Z]*", ErrorMessage = "Dozvoljeno je samo korištenje velikih i malih slova i razmaka!")]
        [DisplayName("Komentar:")]
        public string Komentar { get; set; }
    }
}
