using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Veb_Project.Models.Enums;

namespace Veb_Project.Models
{
    public class Car
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? DriverId { get; set; }
        public Driver Driver { get; set; }
        public ushort Year { get; set; }
        public string Registration { get; set; }
        public int CarNumber { get; set; }
        public CarType Type { get; set; }
    }
}