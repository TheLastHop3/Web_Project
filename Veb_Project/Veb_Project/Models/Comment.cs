using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Veb_Project.Models
{
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public int? CustomerId { get; set; }
        public User Customer { get; set; }
        public int? RideId { get; set; }
        public Ride Ride { get; set; }
        public uint Rate { get; set; }
    }
}