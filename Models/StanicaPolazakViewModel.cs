using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class StanicaPolazakViewModel
    {
        public int IdStanicaPolazak { get; set; }

        [Display(Name = "Naziv")]
        public string Naziv { get; set; }

        public int? IdGrad { get; set; }

        public string? GradNaziv { get; set; }
    }
}
