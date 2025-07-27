using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RailConnect.Models
{
    public class StanicaPolazak
    {
        [Key]
        public int IdStanicaPolazak { get; set; }
        [DisplayName("Naziv:")]
        public string Naziv { get; set; }
        [ForeignKey("Grad")]
        public int IdGrad { get; set; }
        public StanicaPolazak() { }
    }
}
