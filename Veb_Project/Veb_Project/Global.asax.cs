using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Veb_Project.Models;
using Veb_Project.Models.DBContext;

namespace Veb_Project
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            TaxiRepository.Instance.TaxiServiceRepository.Cars.DefaultIfEmpty(null);
            TaxiRepository.Instance.TaxiServiceRepository.Users.DefaultIfEmpty(null);
            TaxiRepository.Instance.TaxiServiceRepository.Drivers.DefaultIfEmpty(null);
            TaxiRepository.Instance.TaxiServiceRepository.Rides.DefaultIfEmpty(null);


            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            /*  TaxiServiceContext db = new TaxiServiceContext("connstring");
              db.users.Add(new Models.User() { Username = "dajiodja" });
              db.SaveChanges();
              */
            

            Location loc = new Location()
            {
                Latitude = 22, Longitude = 30, Address = new Address() { City = "saa", Number = 123, PostalCode = 22, Street = "sa" }
            };
            Location loc1 = new Location()
            {
                Latitude = 25,
                Longitude = 35,
                Address = new Address() { City = "saa", Number = 123, PostalCode = 22, Street = "sa" }
            };
            Location loc2 = new Location()
            {
                Latitude = 40,
                Longitude = 30,
                Address = new Address() { City = "saa", Number = 123, PostalCode = 22, Street = "sa" }
            };

            Car car = new Car()
            {
                CarNumber = 1,
                Registration = "2141",
                Type = Models.Enums.CarType.Sedan,
                Year = 22
            };
            Car car1 = new Car()
            {
                CarNumber = 2,
                Registration = "2141",
                Type = Models.Enums.CarType.Sedan,
                Year = 22
            };

            Car car2 = new Car()
            {
                CarNumber = 3,
                Registration = "2141",
                Type = Models.Enums.CarType.Sedan,
                Year = 22
            };
            Driver testDriver = new Driver()
            {
                Username = "test1",
                Password = "123",
                Name = "test",
                LastName = "test",
                Email = "mail",
                PhoneNumber = "1241",
                JMBG = "2141",
                Location = loc,
                Car = car,
                Sex = Sex.Male,
                Role = UserRole.Driver,
                Blocked = false
                
            };
            Comment com = new Comment()
            {
                Description = "Nice ride",
                PublishDate = DateTime.Now.Date,
                Rate = 5

            };
            Ride ride = new Ride()
            {
                CustomerLocation = new Location() { Address = new Address() { City = "ggg", Number = 23, PostalCode = 2141, Street = "qwrq" }, Latitude = 2141, Longitude = 2141 },
                CarType = Models.Enums.CarType.Sedan,
                Status = RideStatus.Formed,
                OrderTime = DateTime.Now,
                Dispatcher = new User() { Username = "Andrej1",Role= UserRole.Dispatcher, },
                Driver = testDriver,
                Comment = com,
                Fare = 2000

            };
            Ride ride1 = new Ride()
            {
                CustomerLocation = new Location() { Address = new Address() { City = "ggg", Number = 23, PostalCode = 2141, Street = "qwrq" }, Latitude = 2141, Longitude = 2141 },
                CarType = Models.Enums.CarType.Sedan,
                Status = RideStatus.Formed,
                OrderTime = DateTime.Now,
                Dispatcher = new User() { Username = "Andrej1", Role = UserRole.Dispatcher, },
                Driver = testDriver


            };
            Ride ride2 = new Ride()
            {
                CustomerLocation = new Location() { Address = new Address() { City = "ggg", Number = 23, PostalCode = 2141, Street = "qwrq" }, Latitude = 2141, Longitude = 2141 },
                CarType = Models.Enums.CarType.Sedan,
                Status = RideStatus.Formed,
                OrderTime = DateTime.Now,
                Dispatcher = new User() { Username = "Andrej1", Role = UserRole.Dispatcher, },
                Driver = testDriver


            };
            /*  foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
              {
                  TaxiRepository.Instance.AllRides.Add(item);
              }
              */
           /* foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
            {
                if(!TaxiRepository.Instance.SignedUp.ContainsKey(item.Username))
                TaxiRepository.Instance.SignedUp.Add(item.Username,item);
            }
            if(!TaxiRepository.Instance.SignedUpD.ContainsKey(testDriver.Username))
            TaxiRepository.Instance.SignedUpD.Add(testDriver.Username, testDriver);

            TaxiRepository.Instance.AllRides.Add(ride);
            TaxiRepository.Instance.AllRides.Add(ride2);
            TaxiRepository.Instance.AllRides.Add(ride1);
            */
            //TaxiRepository.Instance.TaxiServiceRepository.Rides.Add(ride);
            TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
           // LoadDispechers(@"F:\Web_Project\Veb_Project\Veb_Project\App_Data\Dispechers.txt");
        }

        private void LoadDispechers(string path)
        {
            string readLine = "";
            using (StreamReader streamReader = new StreamReader(path))
            {
                while(!streamReader.EndOfStream)
                {
                    
                    readLine = streamReader.ReadLine();
                    string[] dispData = readLine.Split(':');
                    User disp = new User();
                    if (!TaxiRepository.Instance.UserExists(dispData[0]))
                    {
                        disp.Username = dispData[0];
                        disp.Password = dispData[1];
                        disp.Name = dispData[2];
                        disp.LastName = dispData[3];
                        disp.Sex = (dispData[4] == "Female") ? Sex.Female : Sex.Male;
                        disp.JMBG = dispData[5];
                        disp.Email = dispData[6];
                        disp.PhoneNumber = dispData[7];
                        disp.Blocked =  false;
                        disp.Role = UserRole.Dispatcher;
                        TaxiRepository.Instance.TaxiServiceRepository.Users.Add(disp);
                        TaxiRepository.Instance.SignedUp[dispData[0]] = disp;
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    }
                }

            }


        }
    }
}
