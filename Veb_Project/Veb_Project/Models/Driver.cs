using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_Project.Models
{
    public class Driver:User
    {
        public Driver()
        {
            Role = UserRole.Driver;
        }

        public int? LocationId { get; set; }
        public Location Location { get; set; }
        public int? CarId { get; set; }
        public Car Car { get; set; }
    }
}