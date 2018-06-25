using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_Project.Models
{
    public class Dodatno
    {

        public string DatumOd { get; set; }
        public string DatumDo { get; set; }
        public int OcenaOd { get; set; }
        public int OcenaDo { get; set; }
        public float CenaOd { get; set; }
        public float CenaDo { get; set; }
        public string imeVozaca { get; set; }
        public string prezimeVozaca { get; set; }
        public string imeMusterije { get; set; }
        public string prezimeMusterije { get; set; }
        public bool SearchOcena { get; set; }
        public bool SearchCena { get; set; }
        public bool SearchD { get; set; }
        public bool SearchC { get; set; }
        public bool SearchDatum { get; set; }
    }
}