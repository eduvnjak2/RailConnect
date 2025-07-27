using RailConnect.Data;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RailConnect.Models
{
    public class PutovanjeViewModel
    {
        [DisplayName("ID putovanja:")]
        public int IdPutovanje { get; set; }
        [DisplayName("ID voz:")]
        public int Voz { get; set; }
        [DisplayName("Polazna stanica:")]
        public string StanicaPolazakNaziv { get; set; }
        [DisplayName("ID polazne stanice:")]
        public int StanicaPolazakId { get; set; }
        [DisplayName("Dolazna stanica:")]

        public string StanicaDolazakNaziv { get; set; }
        [DisplayName("Cijena:")]
        public double Cijena { get; set; }
        [DisplayName("Vrijeme polaska:")]
        [DataType(DataType.Time)]
        public DateTime VrijemePolaska { get; set; }
        [DisplayName("Vrijeme dolaska:")]
        [DataType(DataType.Time)]

        public DateTime VrijemeDolaska { get; set; }
        [DisplayName("Datum polaska:")]
        [DataType(DataType.Date)]
        [ValidateDate]

        public DateTime DatumPolaska { get; set; }
        [DisplayName("Broj mjesta:")]
        public int BrojMjesta { get; set; }
        public double? AverageOcjena { get; set; }

        public PutovanjeViewModel() { }

    }
}
