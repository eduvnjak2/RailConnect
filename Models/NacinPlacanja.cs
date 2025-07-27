using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public enum NacinPlacanja
    {
        [Display(Name = "Karticno placanje")]
        Kartica,
        [Display(Name = "Gotovinsko placanje")]
        Gotovina
    }
}
