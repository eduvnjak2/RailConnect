using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class UserEditViewModel
    {
        public string Id { get; set; }

        [Required]
        [Display(Name = "Ime")]
        public string Ime { get; set; }

        [Required]
        [Display(Name = "Prezime")]
        public string Prezime { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Slika")]
        public string Slika { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; }
    }
}

