namespace RailConnect.Models
{
    public class PutovanjeStatistikaViewModel
    {
        public double AverageRating { get; set; }
        public int KapacitetVoz { get; set; }
        public int BrojMjesta { get; set; }
        public double popunjenost { get; set; }

        public List<Recenzija> TopRecenzije { get; set; }
    }

}
