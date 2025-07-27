using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public enum VrstaVoza
    {
        [Display(Name = "Brzi voz")]
        Brzi=0, 
        [Display(Name = "Lokalni voz")]
        Lokalni=1
    }
}
