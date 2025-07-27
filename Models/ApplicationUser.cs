using Microsoft.AspNetCore.Identity;
namespace RailConnect.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Slika { get; set; }
        public string BrojKartice { get; set; }
    }
}
