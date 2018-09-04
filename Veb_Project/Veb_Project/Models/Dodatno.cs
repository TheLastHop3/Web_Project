using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_Project.Models
{
    public class Dodatno
    {

        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public int OcenaOd { get; set; }
        public int OcenaDo { get; set; }
        public decimal CenaOd { get; set; }
        public decimal CenaDo { get; set; }
        public string imeVozaca { get; set; }
        public string prezimeVozaca { get; set; }
        public string imeMusterije { get; set; }
        public string prezimeMusterije { get; set; }
        public bool SearchOcena { get; set; }
        public bool SearchCena { get; set; }
        public bool SearchD { get; set; }
        public bool SearchC { get; set; }
        public bool SearchDatum { get; set; }
        public RideStatus Status { get; set; }
        public bool SortDatum { get; set; }
        public bool SortOcena { get; set; }
        public int Selected { get; set; }
        public string Description { get; set; }
        public string Rate { get; set; }
        public int brojac { get; set; }
    }
}