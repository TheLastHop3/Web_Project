using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using Veb_Project.Models.Enums;

namespace Veb_Project.Models
{
    public class Ride
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime OrderTime { get; set; }
        public int? CustomerLocationId { get; set; }
        public Location CustomerLocation { get; set; }
        public CarType CarType { get; set; }
        public Location Destionation { get; set; }
        public int? CustomerId { get; set; }
        public User Customer { get; set; }
        public int? DispatcherId { get; set; }
        public User Dispatcher { get; set; }
        public int? DriverId { get; set; }
        public Driver Driver { get; set; }
        public decimal Fare { get; set; }
        public int? CommentId { get; set; }
        public Comment Comment { get; set; }
        public RideStatus Status { get; set; }

    }
}