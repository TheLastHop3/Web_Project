using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Veb_Project.Models;
using Veb_Project.Models.DBContext;
using Veb_Project.Models.Enums;

namespace Veb_Project.Controllers
{
    public class TaxiController : ApiController
    {
       
        //lat = pow(driver.lat - custom.lat)
        //log = pow(driver.log - custom.log)
        //return sqrt(lat + log)
        
        [HttpGet]
        [Route("api/Taxi/GetUser")]
        public IHttpActionResult GetUser()
        {
            if (TaxiRepository.Instance.userLogged|| TaxiRepository.Instance.dispecherLogged)
                return Ok(TaxiRepository.Instance.getUser(TaxiRepository.Instance.signedIn.Username));
            else
            {
                return Ok(TaxiRepository.Instance.getDriver(TaxiRepository.Instance.signedInD.Username));
            }
        }
        [HttpPost]
        [Route("api/Taxi/SignIn")]
        public IHttpActionResult SignIn([FromBody]LoginParam login)
        {
            if (login.Logged)
            {
                if (TaxiRepository.Instance.UserExists(login.Username))
                {
                    TaxiRepository.Instance.signedIn = TaxiRepository.Instance.getUser(login.Username);
                    
                    if (TaxiRepository.Instance.signedIn.Blocked) return NotFound();
                    if (TaxiRepository.Instance.signedIn.Role == UserRole.Customer) TaxiRepository.Instance.userLogged = true;
                    else TaxiRepository.Instance.dispecherLogged = true;
                    return Ok(TaxiRepository.Instance.signedIn);
                }else if (TaxiRepository.Instance.DriverExists(login.Username))
                {
                    TaxiRepository.Instance.signedInD = TaxiRepository.Instance.getDriver(login.Username);
                    if (TaxiRepository.Instance.signedInD.Blocked) return NotFound();
                    TaxiRepository.Instance.driverLogged = true;
                    return Ok(TaxiRepository.Instance.signedInD);
                }

              
            }
            else
            {
                if (TaxiRepository.Instance.UserLogin(login.Username, login.Password) && !TaxiRepository.Instance.DriverLogin(login.Username, login.Password))
                {
                    TaxiRepository.Instance.signedIn = TaxiRepository.Instance.getUser(login.Username);
                    if (TaxiRepository.Instance.signedIn.Blocked) return NotFound();
                    if (TaxiRepository.Instance.signedIn.Role == UserRole.Customer) TaxiRepository.Instance.userLogged = true;
                    else TaxiRepository.Instance.dispecherLogged = true;
                    return Ok(TaxiRepository.Instance.signedIn);
                }
                else if (TaxiRepository.Instance.DriverLogin(login.Username, login.Password))
                {


                    TaxiRepository.Instance.signedInD = TaxiRepository.Instance.getDriver(login.Username);
                    if (TaxiRepository.Instance.signedInD.Blocked) return NotFound();
                    TaxiRepository.Instance.driverLogged = true;
                    return Ok(TaxiRepository.Instance.signedInD);
                }
            }
            return NotFound();
           
        }
        [HttpPost]
        [Route("api/Taxi/SignUp")]
        public IHttpActionResult SignUp([FromBody]User user)
        {
            User tmp = new User();
            tmp = user;
            tmp.Role = UserRole.Customer;
            if (!TaxiRepository.Instance.UserExists(user.Username))
            {
                TaxiRepository.Instance.SignedUp[user.Username] = user;
                TaxiRepository.Instance.TaxiServiceRepository.Users.Add(tmp);
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }




        }
        [HttpPost]
        [Route("api/Taxi/SignOff")]
        public void SignOff()
        {
            if(TaxiRepository.Instance.signedIn.Role == UserRole.Customer)
            {
                TaxiRepository.Instance.userLogged = false;
            }
            else if(TaxiRepository.Instance.signedIn.Role == UserRole.Dispatcher)
            {
                TaxiRepository.Instance.dispecherLogged = false;
            }
            else
            {
                TaxiRepository.Instance.driverLogged = false;
            }
        }
        [HttpPost]
        [Route("api/Taxi/SignUpDriver")]
        public IHttpActionResult SignUpDriver([FromBody]Driver driver)
        {
            Driver tmp = new Driver();
            tmp.Car = new Car();
            tmp = driver;
            tmp.Car = driver.Car;
            //tmp.Car.Driver = driver;
            if (!TaxiRepository.Instance.DriverExists(driver.Username) && !TaxiRepository.Instance.UserExists(tmp.Username))
            {
                if (!TaxiRepository.Instance.CarExists(driver.Car.CarNumber))
                {
                    TaxiRepository.Instance.allCars[driver.Car.CarNumber] = driver.Car;
                    TaxiRepository.Instance.SignedUpD[driver.Username] = tmp;

                    TaxiRepository.Instance.TaxiServiceRepository.Cars.Add(tmp.Car);
                    //TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
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
        [HttpPost]
        [Route("api/Taxi/EditDriver")]
        public IHttpActionResult EditDriver([FromBody]Driver driver)
        {
            bool found = false;
      
            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
            {
                if (item.Username != null)
                {
                    if (item.Username == TaxiRepository.Instance.signedInD.Username)
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

                        item.Car.Year = driver.Car.Year;
                        item.Car.Registration = driver.Car.Registration;
                        item.Car.Type = driver.Car.Type;

                        found = true;
                    }
                }
            }
            if (found)
            {
                    TaxiRepository.Instance.SignedUpD[TaxiRepository.Instance.signedInD.Username] = driver; 
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    return Ok();

            }
            if (TaxiRepository.Instance.SignedUpD.ContainsKey(TaxiRepository.Instance.signedInD.Username))
            {
                TaxiRepository.Instance.SignedUpD[TaxiRepository.Instance.signedInD.Username] = driver;
                return Ok();
            }

            return NotFound();

        }
        [HttpPost]
        [Route("api/Taxi/EditUser")]
        public IHttpActionResult EditUser([FromBody]User user)
        {
            bool found = false;
            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
            {
                if (item.Username != null)
                {
                    if (item.Username == TaxiRepository.Instance.signedIn.Username)
                    {
                        item.Name = user.Name;
                        item.LastName = user.LastName;
                        item.JMBG = user.JMBG;
                        item.Email = user.Email;
                        item.PhoneNumber = user.PhoneNumber;
                        item.Sex = user.Sex;
                        found = true;

                    }
                }
                if (TaxiRepository.Instance.SignedUp.ContainsKey(TaxiRepository.Instance.signedIn.Username)){
                    TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username].Name = user.Name;
                    TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username].LastName = user.LastName;
                    TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username].JMBG = user.JMBG;
                    TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username].Email = user.Email;
                    TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username].PhoneNumber = user.PhoneNumber;
                    TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username].Sex = user.Sex;
                }
                if (found)
                {
                 
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    return Ok();
                }

                if (TaxiRepository.Instance.SignedUp.ContainsKey(TaxiRepository.Instance.signedIn.Username))
                {
                    TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username] = user;
                    return Ok();
                }
            }

            return NotFound();

        }
        [HttpPost]
        [Route("api/Taxi/EditPassword")]
        public IHttpActionResult EditPassword([FromBody]LoginParam password)
        {
            bool found = false;
            if (TaxiRepository.Instance.userLogged || TaxiRepository.Instance.dispecherLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {if (item.Username != null)
                    {
                        if (item.Username == TaxiRepository.Instance.signedIn.Username)
                        {
                            item.Password = password.Password;
                            found = true;
                        }
                    }
                }
                if (found)
                {
                    TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username].Password = password.Password;
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                        return Ok();

                }
            if (TaxiRepository.Instance.SignedUp.ContainsKey(TaxiRepository.Instance.signedIn.Username))
            {
                TaxiRepository.Instance.SignedUp[TaxiRepository.Instance.signedIn.Username].Password = password.Password;
                return Ok();
            }
            }
            else if (TaxiRepository.Instance.driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {if (item.Username != null)
                    {
                        if (item.Username == TaxiRepository.Instance.signedInD.Username)
                        {
                            item.Password = password.Password;
                            found = true;
                        }
                    }
                }
                if (found)
                {
                    TaxiRepository.Instance.SignedUpD[TaxiRepository.Instance.signedInD.Username].Password = password.Password;

                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                        return Ok();

                }
            if (TaxiRepository.Instance.SignedUpD.ContainsKey(TaxiRepository.Instance.signedInD.Username))
            {
                TaxiRepository.Instance.SignedUpD[TaxiRepository.Instance.signedInD.Username].Password = password.Password;
                return Ok();
            }
            }

            

            return NotFound();
        }
        [HttpPost]
        [Route("api/Taxi/Addride")]
        public IHttpActionResult AddRide([FromBody]Ride loctype)
        {
            Ride ride = new Ride();
            ride.CarType = loctype.CarType;
            ride.OrderTime = DateTime.Now.Date;
            ride.CustomerLocation = new Location();
            ride.CustomerLocation.Address = new Address();
            ride.CarType = loctype.CarType;
            ride.CustomerLocation.Address.City = loctype.CustomerLocation.Address.City;
            ride.CustomerLocation.Address.Street = loctype.CustomerLocation.Address.Street;

            ride.CustomerLocation.Latitude = loctype.CustomerLocation.Latitude;
            ride.CustomerLocation.Longitude = loctype.CustomerLocation.Longitude;

            if (TaxiRepository.Instance.userLogged)
            {
                TaxiRepository.Instance.dispNapravioVoznju = false;
                ride.Status = RideStatus.Ordered;
                ride.Customer = TaxiRepository.Instance.signedIn;
                TaxiRepository.Instance.AllRides.Add(ride);
                TaxiRepository.Instance.TaxiServiceRepository.Rides.Add(ride);
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                return Ok("Successfuly ordered ride,the available driver will pick you up shorlty");
            }
            else if (TaxiRepository.Instance.dispecherLogged)
            {
                Ride newRide = new Ride();
                newRide.CustomerLocation = new Location();
                newRide.CustomerLocation.Address = new Address();
                newRide.CarType = loctype.CarType;
                newRide.CustomerLocation.Address.City = loctype.CustomerLocation.Address.City;
                newRide.CustomerLocation.Address.Street = loctype.CustomerLocation.Address.Street;
                newRide.CustomerLocation.Latitude = loctype.CustomerLocation.Latitude;
                newRide.CustomerLocation.Longitude = loctype.CustomerLocation.Longitude;
              
                newRide.OrderTime = DateTime.Now.Date;
                newRide.Status = RideStatus.Processed;
                newRide.Dispatcher = TaxiRepository.Instance.getUser(TaxiRepository.Instance.signedIn.Username);
                TaxiRepository.Instance.AllRides.Add(newRide);
                TaxiRepository.Instance.dispNapravioVoznju = true;
                TaxiRepository.Instance.dispRide.CarType = loctype.CarType;
                TaxiRepository.Instance.dispRide.CustomerLocation = loctype.CustomerLocation;
                TaxiRepository.Instance.dispRide.OrderTime = DateTime.Now.Date;
                TaxiRepository.Instance.dispRide.Status = RideStatus.Processed;
                TaxiRepository.Instance.dispRide.Dispatcher = TaxiRepository.Instance.getUser(TaxiRepository.Instance.signedIn.Username);
                TaxiRepository.Instance.TaxiServiceRepository.Rides.Add(newRide);
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                return Ok("");

            }
            return NotFound();
        }

        [HttpGet]
        [Route("api/Taxi/GetDrivers")]
        public List<Driver> getDrivers()
        {
            bool slobodan;
            TaxiRepository.Instance.slobodniVozaci.Clear();
            if (TaxiRepository.Instance.TaxiServiceRepository.Drivers.Count() > 0)
            {
                foreach (var driver in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {

                    slobodan = true;
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver").Include("Driver.Car"))
                {
                        if (item.Driver != null)
                        {
                            if (item.Status == RideStatus.Accepted && item.Driver.Username == driver.Username)
                            {
                                slobodan = false;
                            }
                        }
                }
                            if (slobodan)
                            {
                                TaxiRepository.Instance.slobodniVozaci.Add(driver);
                                // ride.Status = RideStatus.Accepted;
                                //item.Rides.Add(ride);
                                //TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                            }
                }

                return TaxiRepository.Instance.slobodniVozaci;
            }

            return new List<Driver>();
        }
        [HttpGet]
        [Route("api/Taxi/getRides")]
        public List<Ride> getRides()
        {

            //slobodniVozaci.Clear();
            TaxiRepository.Instance.ridesSl.Clear();
            if (TaxiRepository.Instance.dispecherLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("CustomerLocation").Include("CustomerLocation.Address"))
                {

                    if (item.Status == RideStatus.Ordered || item.Status == RideStatus.Processed)
                    {
                        TaxiRepository.Instance.ridesSl.Add(item);
                    }


                }
            }
            else if (TaxiRepository.Instance.driverLogged)
            {
                bool slobodan = true;
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver"))
                {
                    if(item.Driver != null && item.Driver.Username == TaxiRepository.Instance.signedInD.Username && item.Status == RideStatus.Accepted)
                    {
                        slobodan = false;
                    }
                }
                if (slobodan)
                {
                    foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("CustomerLocation").Include("CustomerLocation.Address"))
                    {

                        if (item.Status == RideStatus.Ordered || item.Status == RideStatus.Processed)
                        {
                            TaxiRepository.Instance.ridesSl.Add(item);
                        }


                    }
                }
            }

            return TaxiRepository.Instance.ridesSl;

        }
        //select driver thats gonna accept the ride
        [HttpGet]
        [Route("api/Taxi/SelectDriver")]
        public IHttpActionResult SelectDriver([FromUri]Dodatno dodatno)
        {
          

                TaxiRepository.Instance.selectedRideToAssign.Status = RideStatus.Accepted;
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver").Include("Customer"))
                {
                    

                        if(item.Id == TaxiRepository.Instance.selectedRideToAssign.Id)
                        {
                            if(TaxiRepository.Instance.selectedRideToAssign.Driver != null)
                            item.Driver = TaxiRepository.Instance.selectedRideToAssign.Driver;
                            else
                            item.Driver = TaxiRepository.Instance.slobodniVozaci[dodatno.brojac];
                            item.Status = RideStatus.Accepted;
                            break;
                        }
                    
                }
              
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

            return Ok();
        }
        [HttpGet]
        [Route("api/Taxi/SelectRide")]
        public IHttpActionResult SelectRide([FromUri]Dodatno selected)
        {

            TaxiRepository.Instance.selectedRideToAssign = TaxiRepository.Instance.ridesSl[selected.brojac];
            
            return Ok();

        }
        [HttpGet]
        [Route("api/Taxi/getRidesMain")]
        public List<Ride> getRidesMain()
        {
            TaxiRepository.Instance.GetridesMain.Clear();
            if (TaxiRepository.Instance.userLogged)
            {
                if (TaxiRepository.Instance.TaxiServiceRepository.Rides.Count() >0)
                {
                    foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer").Include("Comment").Include("CustomerLocation").Include("CustomerLocation.Address"))
                    {if (item.Customer != null)
                        {

                            if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.GetridesMain.Add(item);
                        }
                        }
                }

            }
            else if (TaxiRepository.Instance.dispecherLogged && TaxiRepository.Instance.TaxiServiceRepository.Rides.Count() > 0)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Dispatcher").Include("Comment"))
                {
                    if (item.Dispatcher != null)
                    {
                        if (item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username && item.Dispatcher != null) TaxiRepository.Instance.GetridesMain.Add(item);
                    }
                }

            }
            else if (TaxiRepository.Instance.driverLogged)
            {
                if (TaxiRepository.Instance.TaxiServiceRepository.Rides.Count() > 0)
                {
                    foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver").Include("Comment"))
                    {
                        if (item.DriverId == TaxiRepository.Instance.signedInD.Id)
                            TaxiRepository.Instance.GetridesMain.Add(item);
                       
                    }
                }

            }

            return TaxiRepository.Instance.GetridesMain;
        }
        [HttpPost]
        [Route("api/Taxi/CancelRide")]//////////
        public IHttpActionResult CancelRide([FromBody]Dodatno dodatno)
        {
            if (TaxiRepository.Instance.GetridesMain[dodatno.brojac].Status != RideStatus.Accepted)
            {
              

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer"))
                {
                    if (item.Customer != null)
                    {
                        if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username && TaxiRepository.Instance.GetridesMain[dodatno.brojac].Id == item.Id)
                        {
                            item.Status = RideStatus.Canceled;

                        }
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

             

                return Ok();
            }
            return NotFound();
        }
        [HttpPost]
        [Route("api/Taxi/Comment")]
        public IHttpActionResult Comment([FromBody]Dodatno comment)
        {
            
            
            

            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer").Include("Comment"))
            {
                if (item.Customer != null)
                {
                    if (item.CommentId == TaxiRepository.Instance.GetridesMain[comment.brojac].CommentId && item.Id == TaxiRepository.Instance.GetridesMain[comment.brojac].Id)
                    {
                        item.Comment = new Comment();
                        item.Comment.PublishDate = DateTime.Now.Date;
                        item.Comment.Description = comment.Description;                       
                        item.Comment.Rate = UInt32.Parse(comment.Rate);
                        item.Comment.Customer = TaxiRepository.Instance.getUser(TaxiRepository.Instance.signedIn.Username);

                    }
                }
            }

            TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

        

            return Ok();
        }
        [HttpPost]
        [Route("api/Taxi/AssignDriver")]
        public IHttpActionResult AssignDriver([FromBody]Dodatno dodatno)
        {
            if (TaxiRepository.Instance.ridesSl[dodatno.brojac].Status == RideStatus.Ordered || TaxiRepository.Instance.ridesSl[dodatno.brojac].Status == RideStatus.Processed)
            {
                TaxiRepository.Instance.ridesSl[dodatno.brojac].Driver = TaxiRepository.Instance.getDriver(TaxiRepository.Instance.signedInD.Username);
            
                foreach (var ride in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer"))
                {   if (ride.Customer != null)
                    {
                        if ((TaxiRepository.Instance.ridesSl[dodatno.brojac].Customer !=null && ride.Customer.Username == TaxiRepository.Instance.ridesSl[dodatno.brojac].Customer.Username) && TaxiRepository.Instance.ridesSl[dodatno.brojac].Id == ride.Id)
                        {
                            ride.Status = RideStatus.Accepted;
                            if (ride.Driver == null) ride.Driver = TaxiRepository.Instance.ridesSl[dodatno.brojac].Driver;
                        }

                    }
                    else
                    {
                        if(TaxiRepository.Instance.ridesSl[dodatno.brojac].Id == ride.Id)
                        {
                            ride.Status = RideStatus.Accepted;
                            if (ride.Driver == null) ride.Driver = TaxiRepository.Instance.ridesSl[dodatno.brojac].Driver;
                        }
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                
                return Ok();
            }
            return NotFound();
        }
        [HttpGet]
        [Route("api/Taxi/CurrentRide")]
        public IHttpActionResult CurrentRide()
        {
            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver").Include("CustomerLocation").Include("CustomerLocation.Address"))
            {
                if (item.Driver != null && item.Driver.Username == TaxiRepository.Instance.signedInD.Username)
                {
                    if (item.Status == RideStatus.Accepted) return Ok(item);
                }
            }

           
            
            return NotFound();
        }
        [HttpPost]
        [Route("api/Taxi/FinishedRide")]
        public void FinishedRide(Ride finishedRide)
        {

            if(finishedRide.Status == RideStatus.Successful)
            {
               
                
                

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver").Include("Destionation").Include("Destionation.Address"))
                {
                    if (finishedRide.Id == item.Id)
                    {
                            item.Status = RideStatus.Successful;
                            item.Fare = finishedRide.Fare;
                            item.Destionation = new Location();
                            item.Destionation.Latitude = finishedRide.CustomerLocation.Latitude;
                            item.Destionation.Longitude = finishedRide.CustomerLocation.Longitude;
                            item.Destionation.Address = new Address();
                            item.Destionation.Address.Street = finishedRide.CustomerLocation.Address.Street;
                            item.Destionation.Address.City = finishedRide.CustomerLocation.Address.City;

                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
            }
            else
            {

              

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver").Include("Comment"))
                {
                    if (finishedRide.Id == item.Id)
                    {
                            item.Status = RideStatus.Unsuccessful;
                            item.Comment = new Comment();
                            item.Comment.PublishDate = DateTime.Now;
                            item.Comment.Description = finishedRide.Comment.Description;
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
            }
        }
        [HttpGet]
        [Route("api/Taxi/AllRides")]
        public List<Ride> AllRides()
        {

            return TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("CustomerLocation").Include("Customer").Include("Dispatcher").Include("Comment").Include("Driver").ToList();
        }

        [HttpPost]
        [Route("api/Taxi/Filter")]
        public List<Ride> Filter([FromBody]Dodatno status)
        {
            if (TaxiRepository.Instance.dispecherLogged)
            {
                TaxiRepository.Instance.RidesDisp.Clear();
                

                    if(status.Status == RideStatus.NoFilter)
                    {
                        foreach (var item in TaxiRepository.Instance.AllRides)
                        {if (item.Dispatcher.Username != null)
                        {
                            if (item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username)
                            {
                                TaxiRepository.Instance.RidesDisp.Add(item);

                            }
                        }
                        }
                    }
                    else
                    {
                        foreach (var item in TaxiRepository.Instance.AllRides)
                        {
                        if (item.Dispatcher.Username != null)
                        {
                            if (item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username && item.Status == status.Status)
                            {
                                TaxiRepository.Instance.RidesDisp.Add(item);

                            }
                        }
                        }
                    }

                    return TaxiRepository.Instance.RidesDisp;
                

            }else if (TaxiRepository.Instance.userLogged)
            {
                TaxiRepository.Instance.RideUser.Clear();
                if (status.Status == RideStatus.NoFilter)
                {

                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {if (item.Customer.Username != null)
                        {
                            if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RideUser.Add(item);
                        }
                        }
                }
                else
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (item.Customer.Username != null)
                        {
                            if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username && item.Status == status.Status) TaxiRepository.Instance.RideUser.Add(item);
                        }
                    }
                }

                return TaxiRepository.Instance.RideUser;
            }
            else if(TaxiRepository.Instance.driverLogged)
            {
                TaxiRepository.Instance.RideDriver.Clear();
                if (status.Status == RideStatus.NoFilter)
                {

                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (item.Driver != null)
                        {
                            if (item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RideDriver.Add(item);
                        }
                    }
                }
                else
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (item.Customer != null)
                        {
                            if (item.Driver.Username == TaxiRepository.Instance.signedInD.Username && item.Status == status.Status) TaxiRepository.Instance.RideDriver.Add(item);
                        }
                    }
                }

                return TaxiRepository.Instance.RideDriver;
            }

            return null;
        }
        [HttpPost]
        [Route("api/Taxi/Sort")]
        public List<Ride> Sort([FromBody]Dodatno dodatno)
        {
            List<Ride> list = new List<Ride>();
            foreach (var item in TaxiRepository.Instance.AllRides)
            {
                if (TaxiRepository.Instance.userLogged && item.Customer.Username != null)
                {
                    
                    if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username) list.Add(item);

                }
                else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher.Username != null)
                {
                    if ( item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) list.Add(item);

                }
                else if (TaxiRepository.Instance.driverLogged && item.Driver.Username != null)
                {
                    if ( item.Driver.Username == TaxiRepository.Instance.signedInD.Username) list.Add(item);

                }
            }


            if (dodatno.SortDatum)
            {
                for (int i = 0; i < list.Count - 1; i++)
                {
                    for (int j = i + 1; j > 0; j--)
                    {
                        if (list[j - 1].OrderTime < list[j].OrderTime)
                        {
                            Ride temp = list[j - 1];
                            list[j - 1] = list[j];
                            list[j] = temp;
                        }
                    }
                }

            }
            else if(dodatno.SortOcena)
            {
                for (int i = 0; i < list.Count - 1; i++)
                {
                    for (int j = i + 1; j > 0; j--)
                    {
                        if (list[j - 1].Comment.Rate < list[j].Comment.Rate)
                        {
                            Ride temp = list[j - 1];
                            list[j - 1] = list[j];
                            list[j] = temp;
                        }
                    }
                }

            }
            return list;
        }
        [HttpPost]
        [Route("api/Taxi/Search")]
        public List<Ride> Search([FromBody]Dodatno dodatno)
        {
            List<Ride> rides = TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer").Include("Driver").Include("Comment").ToList();
            if (dodatno.SearchC)
            {
                if (dodatno.imeMusterije != null)
                    rides = rides.Where(x => x.Customer != null && x.Customer.Name == dodatno.imeMusterije).ToList();
                if (dodatno.prezimeMusterije != null)
                    rides = rides.Where(x => x.Customer != null && x.Customer.LastName == dodatno.prezimeMusterije).ToList();
            }
            if (dodatno.SearchD)
            {
                if (dodatno.imeVozaca != null)
                    rides = rides.Where(x => x.Driver.Name == dodatno.imeVozaca).ToList();
                if (dodatno.prezimeVozaca != null)
                    rides = rides.Where(x => x.Driver.LastName == dodatno.prezimeVozaca).ToList();
            }
            return rides;
        }

        private List<Ride> getAllRides(string username)
        {
            List<Ride> rides = new List<Ride>();
            if (TaxiRepository.Instance.dispecherLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Dispatcher").Include("Comment"))
                {
                    if (item.Dispatcher.Username == username && item.Dispatcher != null) rides.Add(item);
                }
            }else if (TaxiRepository.Instance.userLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer").Include("Comment"))
                {
                    if (item.Customer.Username == username) rides.Add(item);
                }
            }else if (TaxiRepository.Instance.driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver").Include("Comment"))
                {
                    if (item.Driver.Username == username) rides.Add(item);
                }
            }

            return rides;
        
        }

        [HttpPost]
        [Route("api/Taxi/Block")]
        public IHttpActionResult Block([FromBody]LoginParam username)
        {
            bool found = false;
            if (TaxiRepository.Instance.UserExists(username.Username))
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {   if (item.Username != "" || item.Username != null)
                    {
                        if (item.Username == username.Username && item.Role != UserRole.Dispatcher)
                        {
                            found = true;
                            item.Blocked = true;

                        }
                    }
                }
                foreach (var item in TaxiRepository.Instance.SignedUp.Values)
                {
                    if (item.Username == username.Username && item.Role != UserRole.Dispatcher)
                    {
                        item.Blocked = true;

                    }
                }
                if (found)
                {
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                        return Ok();
                }

            }else if (TaxiRepository.Instance.DriverExists(username.Username))
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (item.Username == username.Username)
                    {
                        found = true;
                        item.Blocked = true;
                       
                    }
                }
                foreach (var item in TaxiRepository.Instance.SignedUpD.Values)
                {
                    if (item.Username == username.Username)
                    {
                        found = true;
                        item.Blocked = true;

                    }
                }
                if (found)
                {
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    return Ok();
                }
            }
            return NotFound();
        }
        [HttpPost]
        [Route("api/Taxi/Unblock")]
        public IHttpActionResult UnBlock([FromBody]LoginParam username)
        {
            bool found = false;
            if (TaxiRepository.Instance.UserExists(username.Username) || TaxiRepository.Instance.SignedUp.ContainsKey(username.Username))
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {
                    if (item.Username != "" || item.Username != null)
                    {
                        if (item.Username == username.Username && item.Role != UserRole.Dispatcher)
                        {
                            item.Blocked = false;
                            found = true;

                        }
                    }
                }
                foreach (var item in TaxiRepository.Instance.SignedUp.Values)
                {
                    if (item.Username != "" || item.Username != null)
                    {
                        if (item.Username == username.Username && item.Role != UserRole.Dispatcher)
                        {
                            item.Blocked = false;
   

                        }
                    }
                }
                if (found)
                {
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    return Ok();
                }

            }
            else if (TaxiRepository.Instance.DriverExists(username.Username))
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (item.Username != "" || item.Username != null)
                    {

                        if (item.Username == username.Username)
                        {
                            item.Blocked = false;
                            found = true;
                        }
                    }
                }
                foreach (var item in TaxiRepository.Instance.SignedUpD.Values)
                {
                    if (item.Username != "" || item.Username != null)
                    {

                        if (item.Username == username.Username)
                        {
                            item.Blocked = false;

                        }
                    }
                }
                if (found)
                {
                    TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                    return Ok();
                }
            }
            return NotFound();
        }
        //lat = pow(driver.lat - custom.lat)
        //log = pow(driver.log - custom.log)
        //return sqrt(lat + log)
        [HttpGet]
        [Route("api/Taxi/getUsersDistance")]
        public List<Ride> getUsersDistance()
        {
            List<Ride> rides = new List<Ride>();
      
            List<double> distances = new List<double>();
            if (TaxiRepository.Instance.driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Driver").Include("CustomerLocation"))
                {
                    if(item.DriverId == TaxiRepository.Instance.signedInD.Id)
                    {
                        rides.Add(item);
                    }
                }

                foreach (var item in rides)
                {
                 
                    double lat = Math.Pow(TaxiRepository.Instance.signedInD.Location.Latitude - item.CustomerLocation.Latitude, 2);
                    double log = Math.Pow(TaxiRepository.Instance.signedInD.Location.Longitude - item.CustomerLocation.Longitude, 2);
                    distances.Add(Math.Sqrt(lat + log));
                    
                }

                for (int i = 0; i < distances.Count - 1; i++)
                {
                    for (int j = i + 1; j > 0; j--)
                    {
                        if (distances[j - 1] > distances[j])
                        {
                            double temp = distances[j - 1];
                            distances[j - 1] = distances[j];
                            distances[j] = temp;

                        }
                    }
                }
                List<Ride> sortRides = new List<Ride>();
                for (int j = 0; j < distances.Count; j++)
                {
                    for (int i = 0; i < rides.Count; i++)
                    {
                        if(distances[j] == (Math.Sqrt(Math.Pow(TaxiRepository.Instance.signedInD.Location.Latitude - rides[i].CustomerLocation.Latitude, 2) + Math.Pow(TaxiRepository.Instance.signedInD.Location.Longitude - rides[i].CustomerLocation.Longitude, 2)))){
                            sortRides.Add(rides[i]);
                        }

                    }
                }
                return sortRides;

            }

            return null;

        }
        [HttpPost]
        [Route("api/Taxi/Preview")]
        public List<Ride> Preview([FromBody]Dodatno dodatno)
        {
            List<Ride> allRides = TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer").Include("Driver").Include("Dispatcher").Include("CustomerLocation").Include("Comment").ToList();
            List<Ride> rides = new List<Ride>();
            if (TaxiRepository.Instance.dispecherLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer").Include("Driver").Include("Dispatcher").Include("CustomerLocation").Include("Comment"))
                {
                    if(item.Dispatcher != null && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username)
                    rides.Add(item);
                }
            }else if (TaxiRepository.Instance.userLogged)
            {
                foreach (var item in allRides)
                {
                    if(item.Customer != null && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) 
                    rides.Add(item);
                }
               
            }else if (TaxiRepository.Instance.driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides.Include("Customer").Include("Driver").Include("Dispatcher").Include("CustomerLocation").Include("Comment"))
                {
                    if (item.Driver != null && item.Driver.Username == TaxiRepository.Instance.signedInD.Username)
                        rides.Add(item);
                }
            }



            if(dodatno.Status != RideStatus.NoFilter)
            {
               rides = rides.Where(x => x.Status == dodatno.Status).ToList();

            }
            DateTime dateTime = new DateTime(1, 1, 1);
            if (dodatno.DatumOd.Date != dateTime)
            {
                rides = rides.Where(x => x.OrderTime.Date > dodatno.DatumOd).ToList();
                
            }
            if (dodatno.DatumDo != dateTime)
            {
                rides = rides.Where(x => x.OrderTime.Date < dodatno.DatumDo).ToList();
            }
            if(dodatno.CenaDo != -1)
            {
                rides = rides.Where(x => x.Fare < dodatno.CenaDo).ToList();
            }

            if (dodatno.CenaOd != -1)
            {
                rides = rides.Where(x => x.Fare > dodatno.CenaOd).ToList();
            }

            if(dodatno.OcenaDo != -1)
            {
                rides = rides.Where(x => x.Comment.Rate < dodatno.OcenaDo).ToList();
            }

            if (dodatno.OcenaOd != -1)
            {
                rides = rides.Where(x => x.Comment.Rate > dodatno.OcenaOd).ToList();
            }

            if (dodatno.SortDatum)
            {
                for (int i = 0; i < rides.Count - 1; i++)
                {
                    for (int j = i + 1; j > 0; j--)
                    {
                        if (rides[j - 1].OrderTime < rides[j].OrderTime)
                        {
                            Ride temp = rides[j - 1];
                            rides[j - 1] = rides[j];
                            rides[j] = temp;
                        }
                    }
                }

            }
            else if (dodatno.SortOcena)
            {
                for (int i = 0; i < rides.Count - 1; i++)
                {
                    for (int j = i + 1; j > 0; j--)
                    {
                        if (rides[j - 1].Comment.Rate < rides[j].Comment.Rate)
                        {
                            Ride temp = rides[j - 1];
                            rides[j - 1] = rides[j];
                            rides[j] = temp;
                        }
                    }
                }

            }
            return rides;
        }
    }
}