namespace RailConnect.Models
{
    public class PutovanjeIndexViewModel
    {
            public List<PutovanjeViewModel> CurrentPutovanja { get; set; } = new List<PutovanjeViewModel>();
            public List<PutovanjeViewModel> PreviousPutovanja { get; set; } = new List<PutovanjeViewModel>();

    }


}
