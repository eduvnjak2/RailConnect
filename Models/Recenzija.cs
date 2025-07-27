using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailConnect.Models
{
    public class Recenzija
    {
        [Key]
        public int IdRecencija { get; set; }
        [ForeignKey("ApplicationUser")]
        public string PutnikId { get; set; }

        [ForeignKey("Putovanje")]
        [Required]
        [DisplayName("Putovanje :")]
        public int PutovanjeId { get; set; }
        [Required]
        [Range(1, 5, ErrorMessage = "Ocjena mora biti u opsegu 1 do 5")]
        [DisplayName("Cijena:")]
        public int Ocjena { get; set; }
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage = "Komentar smije imati između 0 i 100 karaktera!")]
        [RegularExpression(@"[a-z|A-Z]*", ErrorMessage = "Dozvoljeno je samo korištenje velikih i malih slova i razmaka!")]
        [DisplayName("Komentar:")]
        public string Komentar { get; set; }
        public Recenzija() { }
    }
}
