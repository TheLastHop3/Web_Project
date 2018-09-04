using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Veb_Project.Models
{
    public class TaxiServiceContext:DbContext
    {
        public TaxiServiceContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Driver> Drivers { get; set; }
        public DbSet<Ride> Rides { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Driver>().HasOptional(x => x.Car).WithMany().HasForeignKey(x => x.CarId);
            modelBuilder.Entity<Car>().HasOptional(x => x.Driver).WithMany().HasForeignKey(x => x.DriverId);
            modelBuilder.Entity<Ride>().HasOptional(x => x.Comment).WithMany().HasForeignKey(x => x.CommentId);
            modelBuilder.Entity<Ride>().HasOptional(x => x.CustomerLocation).WithMany().HasForeignKey(x => x.CustomerLocationId);
            modelBuilder.Entity<Comment>().HasOptional(x => x.Ride).WithMany().HasForeignKey(x => x.RideId);
            base.OnModelCreating(modelBuilder);
        }
    }
}