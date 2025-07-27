using RailConnect.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RailConnect.Models
{
    public class Karta
    {
        [Key]
        public int IdKarta { get; set; }

        [ForeignKey("StanicaPolazak")]
        public int IdStanicaPolazak { get; set; }

        [ForeignKey("StanicaDolazak")]
        public int IdStanicaDolazak { get; set; }

        [ForeignKey("ApplicationUser")]
        public string PutnikId { get; set; }

        [ForeignKey("Putovanje")]
        public int PutovanjeId { get; set; }

        [Required]
        [DisplayName("Nacin placanja:")]
        [EnumDataType(typeof(NacinPlacanja))]
        public NacinPlacanja NacinPlacanja { get; set; }

        // Navigation properties
        //public virtual ApplicationUser Putnik { get; set; }
        //public virtual StanicaPolazak StanicaPolazak { get; set; }
        //public virtual StanicaDolazak StanicaDolazak { get; set; }

        public Karta() { }
    }
}
