using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_Project.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public uint Number { get; set; }
        public string City { get; set; }
        public uint PostalCode { get; set; }
        public override string ToString()
        {
            return $"{Street} {Number}, {City} {PostalCode}";
        }
    }
}