using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace RailConnect.Models
{
    public class UserViewModel
    {
        [DisplayName("Slika:")]
        public string Slika { get; set; }
        [DisplayName("ID:")]

        public string Id { get; set; }
        [DisplayName("Ime:")]

        public string Ime { get; set; }
        [DisplayName("Prezime:")]

        public string Prezime { get; set; }
        [DisplayName("Email:")]

        public string Email { get; set; }
        [DisplayName("Uloga:")]

        public string Role { get; set; }
    }


}
