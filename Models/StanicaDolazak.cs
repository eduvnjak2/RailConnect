using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RailConnect.Models
{
    public class StanicaDolazak
    {
        [Key]
        public int IdStanicaDolazak { get; set; }
        [Required]
        [DisplayName("Naziv:")]
        public string Naziv { get; set; }

        [ForeignKey("Grad")]
        [DisplayName("Grad:")]
        public int IdGrad { get; set; }
        public Grad Grad { get; set; }

        public StanicaDolazak() { }
    }
}
