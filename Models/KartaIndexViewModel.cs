namespace RailConnect.Models
{
    public class KartaIndexViewModel
    {
        public List<KartaViewModel> ValidKartas { get; set; } = new List<KartaViewModel>();
        public List<KartaViewModel> PreviousKartas { get; set; } = new List<KartaViewModel>();
    }

}
