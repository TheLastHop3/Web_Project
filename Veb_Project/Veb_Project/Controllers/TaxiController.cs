using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Veb_Project.Models;
using Veb_Project.Models.DBContext;
using Veb_Project.Models.Enums;

namespace Veb_Project.Controllers
{
    public class TaxiController : ApiController
    {
        //ProbError: comment deo vrv ne radi
        //Done:Template for SignIn,Options,SignOff,SignUp,Details User/Driver,Edit info User/Driver,Edit pw,SignUp Driver,printMainCustomer,Add ride disp,custom
        //Done: cancel ride,printMainDriver,AcceptRideDriver,Change status for ride,edit printMainCustomer,For successful add button addComment,printDispecher
        //TO DO: ,Filter,Search,SignOff,Sort,SignIn(Cookie)
        private User signedIn = new User();
        private Driver signedInD = new Driver();
        private List<Driver> slobodniVozaci = new List<Driver>();
        private Ride dispRide = null;
        bool userLogged = false;
        bool driverLogged = false;
        bool dispecherLogged = false;
        bool dispNapravioVoznju = false;
        private List<Ride> ridesSl = new List<Ride>();
        private Ride selectedRideToAssign = new Ride();
        List<Ride> GetridesMain = new List<Ride>();
        public IHttpActionResult SignIn(string username, string password, bool checkBox)
        {
            if (!TaxiRepository.Instance.UserLogin(username, password) || !TaxiRepository.Instance.DriverLogin(username,password))
            {
                
                if (checkBox)
                {
                   
                    //remember
                }
                else
                {
                    //dont remember
                }
                signedIn = TaxiRepository.Instance.getUser(username);
                return Ok(signedIn);
            }else if(!TaxiRepository.Instance.DriverLogin(username, password))
            {

                if (checkBox)
                {
                    //remember
                }
                else
                {
                    //dont remember
                }

                signedInD = TaxiRepository.Instance.getDriver(username);

                return Ok(signedInD);
            }

            return NotFound();

        }
        public IHttpActionResult SignUp(User user)
        {
            User tmp = new User();
            tmp = user;
            tmp.Role = UserRole.Customer;
            if (!TaxiRepository.Instance.UserExists(user.Username))
            {
                TaxiRepository.Instance.TaxiServiceRepository.Users.Add(tmp);
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }




        }
        public void SignOff()
        {
            if(signedIn.Role == UserRole.Customer)
            {
                userLogged = false;
            }
            else if(signedIn.Role == UserRole.Dispatcher)
            {
                dispecherLogged = false;
            }
            else
            {
                driverLogged = false;
            }
        }
        public IHttpActionResult SignUpDriver(Driver driver)
        {
            Driver tmp = new Driver();
            tmp = driver;
            tmp.Car.Driver = driver;
            if (!TaxiRepository.Instance.DriverExists(driver.Username) && !TaxiRepository.Instance.UserExists(tmp.Username))
            {
                if (!TaxiRepository.Instance.CarExists(driver.Car.CarNumber))
                {
                    TaxiRepository.Instance.TaxiServiceRepository.Cars.Add(tmp.Car);
                    TaxiRepository.Instance.TaxiServiceRepository.Drivers.Add(tmp);
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    return NotFound();
                }
                else
                {

                    return Ok("Car");
                }
            }
            else
            {
                return Ok("Driver");
            }

        }

        public IHttpActionResult EditDriver(Driver driver)
        {
            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
            {
                if (item.Username == signedInD.Username)
                {
                    item.Name = driver.Name;
                    item.LastName = driver.LastName;
                    item.JMBG = driver.JMBG;
                    item.Email = driver.Email;
                    item.PhoneNumber = driver.PhoneNumber;
                    item.Sex = driver.Sex;
                    item.Location.Latitude = driver.Location.Latitude;
                    item.Location.Longitude = driver.Location.Longitude;
                    item.Location.Address.Street = driver.Location.Address.Street;
                    item.Location.Address.City = driver.Location.Address.City;
                    item.Location.Address.Number = driver.Location.Address.Number;
                    item.Location.Address.PostalCode = driver.Location.Address.PostalCode;
                    item.Car.Year = driver.Car.Year;
                    item.Car.Registration = driver.Car.Registration;
                    item.Car.Type = driver.Car.Type;
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    return Ok(item);
                }
            }
            return NotFound();

        }

        public IHttpActionResult EditUser(User user)
        {
            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
            {
                if (item.Username == signedIn.Username)
                {
                    item.Name = user.Name;
                    item.LastName = user.LastName;
                    item.JMBG = user.JMBG;
                    item.Email = user.Email;
                    item.PhoneNumber = user.PhoneNumber;
                    item.Sex = user.Sex;
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    return Ok(item);
                }
            }
            return NotFound();

        }
        public IHttpActionResult EditPassword(string password)
        {
            if (userLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {
                    if (item.Username == signedIn.Username)
                    {
                        item.Password = password;
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                        return Ok();
                    }

                }
            }
            else if (driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (item.Username == signedInD.Username)
                    {
                        item.Password = password;
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                        return Ok();
                    }
                }
            }

            return NotFound();
        }
        public IHttpActionResult AddRide(Location location, CarType type)
        {
            Ride ride = new Ride();
            ride.CarType = type;
            ride.CustomerLocation = location;
            ride.OrderTime = DateTime.Now;


            if (userLogged)
            {
                dispNapravioVoznju = false;
                ride.Status = RideStatus.Ordered;
                ride.Customer = signedIn;
                signedIn.Rides.Add(ride);
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {
                    if (item.Username == signedIn.Username)
                    {
                        item.Rides.Add(ride);
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.Rides.Add(ride);
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                return Ok("Successfuly ordered ride,the available driver will pick you up shorlty");
            }
            else if (dispecherLogged)
            {
                dispNapravioVoznju = true;
                dispRide.CarType = type;
                dispRide.CustomerLocation = location;
                dispRide.OrderTime = DateTime.Now;
                dispRide.Status = RideStatus.Processed;
                dispRide.Dispatcher = TaxiRepository.Instance.getUser(signedIn.Username);
                TaxiRepository.Instance.TaxiServiceRepository.Rides.Add(dispRide);
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                return Ok("");

            }
            return NotFound();
        }

        public List<Driver> getDrivers()
        {
            bool slobodan;
            slobodniVozaci.Clear();
            if (TaxiRepository.Instance.TaxiServiceRepository.Drivers.Count() > 0)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    slobodan = true;
                    foreach (var rides in item.Rides)
                    {
                        if (rides.Status == RideStatus.Accepted) slobodan = false;
                    }
                    if (slobodan)
                    {
                        slobodniVozaci.Add(item);
                        // ride.Status = RideStatus.Accepted;
                        //item.Rides.Add(ride);
                        //TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                    }
                }

                return slobodniVozaci;
            }

            return new List<Driver>();
        }

        public List<Ride> getRides()
        {

            //slobodniVozaci.Clear();
            ridesSl.Clear();
            if (dispecherLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {

                    if (item.Status == RideStatus.Ordered || item.Status == RideStatus.Processed)
                    {
                        ridesSl.Add(item);
                    }


                }
            }
            else if (driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {

                    if (item.Status == RideStatus.Ordered)
                    {
                        ridesSl.Add(item);
                    }


                }
            }

            return ridesSl;

        }
        //select driver thats gonna accept the ride
        public IHttpActionResult SelectDriver(int i)
        {
            if (dispNapravioVoznju)
            {
                slobodniVozaci[i].Rides.Add(dispRide);
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (slobodniVozaci[i].Username == item.Username)
                    {
                        dispRide.Status = RideStatus.Accepted;
                        item.Rides.Add(dispRide);
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                        break;
                    }
                }
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {
                    if (item.Username == signedIn.Username)
                    {
                        foreach (var rides in item.Rides)
                        {
                            if (rides.Dispatcher.Username == signedIn.Username)
                            {
                                rides.Status = RideStatus.Accepted;
                                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                                return Ok();
                            }
                        }

                    }
                }
            }
            else
            {

                selectedRideToAssign.Status = RideStatus.Accepted;
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Customer.Username == selectedRideToAssign.Customer.Username)
                    {
                        item.Status = RideStatus.Accepted;
                        break;
                    }
                }

                slobodniVozaci[i].Rides.Add(selectedRideToAssign);
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (slobodniVozaci[i].Username == item.Username)
                    {
                        dispRide.Status = RideStatus.Accepted;
                        item.Rides.Add(dispRide);
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

            }
            return Ok();
        }

        public IHttpActionResult SelectRide(int i)
        {

            selectedRideToAssign = ridesSl[i];
            return Ok();

        }

        public List<Ride> getRidesMain()
        {
            GetridesMain.Clear();
            if (userLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Customer.Username == signedIn.Username) GetridesMain.Add(item);
                }

            }
            else if (dispecherLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Dispatcher.Username == signedIn.Username) GetridesMain.Add(item);
                }

            }
            else if (driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Driver.Username == signedInD.Username) GetridesMain.Add(item);
                }

            }

            return GetridesMain;
        }

        public IHttpActionResult CancelRide(int i)
        {
            if (GetridesMain[i].Status != RideStatus.Accepted)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {
                    if (item.Username == signedIn.Username)
                    {
                        foreach (var ride in item.Rides)
                        {
                            if (ride.Customer.Username == signedIn.Username)
                            {
                                ride.Status = RideStatus.Canceled;
                                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                            }
                        }
                    }
                }

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Customer.Username == signedIn.Username)
                    {
                        item.Status = RideStatus.Canceled;
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                    }
                }
                return Ok();
            }
            return NotFound();
        }

        public IHttpActionResult Comment(Comment Comment)
        {
            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
            {
                if (item.Username == signedIn.Username)
                {
                    foreach (var ride in item.Rides)
                    {
                        
                        if (ride.Customer.Username == signedIn.Username)
                        {
                            ride.Comment.PublishDate = DateTime.Now;

                            ride.Comment.Description = Comment.Description;
                            ride.Comment.Rate = Comment.Rate;
                            ride.Comment.Ride = ride;
                            TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                            ride.Comment.Customer = TaxiRepository.Instance.getUser(signedIn.Username);
                            TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                        }
                    }
                }
            }

            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
            {
                if (item.Customer.Username == signedIn.Username)
                {
                    item.Comment.PublishDate = DateTime.Now;
                    item.Comment.Description = Comment.Description;
                    item.Comment.Rate = Comment.Rate;
                    item.Comment.Ride = item;
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    item.Comment.Customer = TaxiRepository.Instance.getUser(signedIn.Username);
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                }
            }

            return Ok();
        }

        public IHttpActionResult AssignDriver(int i)
        {
            if (ridesSl[i].Status == RideStatus.Ordered)
            {
                ridesSl[i].Driver = TaxiRepository.Instance.getDriver(signedInD.Username);
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (item.Username == signedInD.Username)
                    {
                        item.Rides.Add(ridesSl[i]);

                    }
                }
                foreach (var ride in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (ride.Customer.Username == ridesSl[i].Customer.Username)
                    {
                        ride.Status = RideStatus.Accepted;
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                return Ok();
            }
            return NotFound();
        }

        public IHttpActionResult CurrentRide()
        {
            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
            {
                if(signedInD.Username == item.Username)
                {
                    foreach (var ride in item.Rides)
                    {
                        if(ride.Status == RideStatus.Accepted)
                        {
                            return Ok(ride);
                        }
                    }
                }
            }
            return NotFound();
        }

        public void FinishedRide(RideStatus status,Comment comment,Location location,int fare)
        {

            if(status == RideStatus.Successful)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (signedInD.Username == item.Username)
                    {
                        foreach (var ride in item.Rides)
                        {
                            ride.Status = RideStatus.Successful;
                            ride.Fare = fare;
                            ride.Destionation.Latitude = location.Latitude;
                            ride.Destionation.Longitude = location.Longitude;
                            ride.Destionation.Address.Street = location.Address.Street;
                            ride.Destionation.Address.Number = location.Address.Number;
                            ride.Destionation.Address.City = location.Address.City;
                            ride.Destionation.Address.PostalCode = location.Address.PostalCode;
                        }
                    }
                }

                foreach (var user in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {
                    foreach (var ride in user.Rides)
                    {
                        if(user.Username == ride.Customer.Username)
                        {
                            ride.Driver = TaxiRepository.Instance.getDriver(signedInD.Username);
                            ride.Status = RideStatus.Successful;
                            ride.Fare = fare;
                            ride.Destionation.Latitude = location.Latitude;
                            ride.Destionation.Longitude = location.Longitude;
                            ride.Destionation.Address.Street = location.Address.Street;
                            ride.Destionation.Address.Number = location.Address.Number;
                            ride.Destionation.Address.City = location.Address.City;
                            ride.Destionation.Address.PostalCode = location.Address.PostalCode;
                        }
                    }
                }

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if(item.Driver.Username == signedInD.Username)
                    {
                        item.Status = RideStatus.Successful;
                        item.Fare = fare;
                        item.Destionation.Latitude = location.Latitude;
                        item.Destionation.Longitude = location.Longitude;
                        item.Destionation.Address.Street = location.Address.Street;
                        item.Destionation.Address.Number = location.Address.Number;
                        item.Destionation.Address.City = location.Address.City;
                        item.Destionation.Address.PostalCode = location.Address.PostalCode;
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
            }
            else
            {

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (signedInD.Username == item.Username)
                    {
                        foreach (var ride in item.Rides)
                        {
                            ride.Status = RideStatus.Unsuccessful;
                            ride.Comment.Description = comment.Description;

                        }
                    }
                }

                foreach (var user in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {
                    foreach (var ride in user.Rides)
                    {
                        if (user.Username == ride.Customer.Username)
                        {
                            ride.Status = RideStatus.Unsuccessful;
                            ride.Comment.Description = comment.Description;
                        }
                    }
                }

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Driver.Username == signedInD.Username)
                    {
                        item.Status = RideStatus.Unsuccessful;
                        item.Comment.Description = comment.Description;
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
            }
        }
        public List<Ride> AllRides()
        {

            return TaxiRepository.Instance.TaxiServiceRepository.Rides.ToList();
        }
    }
}