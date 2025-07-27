using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class Grad
    {
        [Key]
        public int idGrad { get; set; }
        [Required]
        [StringLength(maximumLength: 50, MinimumLength = 3, ErrorMessage ="Naziv grada smije imati između 3 i 50 karaktera!")]
        [RegularExpression(@"[a-z|A-Z]*", ErrorMessage = "Dozvoljeno je samo korištenje velikih i malih slova, brojeva i razmaka!")] 
        [DisplayName("Naziv grada:")]
        public string naziv { get; set; }
        public Grad() {}
    }
}
