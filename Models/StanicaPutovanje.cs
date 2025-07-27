using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailConnect.Models
{
    public class StanicaPutovanje
    {
        [Key]
        public int IdStanicaPutovanje { get; set; }
        [ForeignKey("StanicaDolazak")]
        public int IdStanicaDolazak { get; set; }
        [ForeignKey("Putovanje")]
        public int IdPutovanje { get; set; }
        public StanicaPutovanje() { }
    }
}
