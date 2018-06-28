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
       
        //kada se salje datum kroz text input(moze ali se mora napisati u formatu:m.d.y),takodje moze i preko date inputa,jedini problem jeste sto time je 12.00.00 tj kod provere gledati samo date ne time
        //GoogleMaps:napraviti novu classu string lat,string log,string addressa,parsirati addresu,kada ocemo edit,ili prikazati(preraditi init da prihvata lat long)
        //lat = pow(driver.lat - custom.lat)
        //log = pow(driver.log - custom.log)
        //return sqrt(lat + log)

        
        [HttpGet]
        [Route("api/taxi/getuser")]
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
        [Route("api/taxi/signin")]
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
        [Route("api/taxi/signup")]
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
        [Route("api/taxi/signoff")]
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
        [Route("api/taxi/signupdriver")]
        public IHttpActionResult SignUpDriver([FromBody]Driver driver)
        {
            Driver tmp = new Driver();
            tmp = driver;
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
        [Route("api/taxi/editdriver")]
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
                        item.Location.Address.Number = driver.Location.Address.Number;
                        item.Location.Address.PostalCode = driver.Location.Address.PostalCode;
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
        [Route("api/taxi/edituser")]
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
        [Route("api/taxi/editpassword")]
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
        [Route("api/taxi/addride")]
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
            ride.CustomerLocation.Address.Number = loctype.CustomerLocation.Address.Number;
            ride.CustomerLocation.Address.PostalCode = loctype.CustomerLocation.Address.PostalCode;
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
                newRide.CustomerLocation.Address.Number = loctype.CustomerLocation.Address.Number;
                newRide.CustomerLocation.Address.PostalCode = loctype.CustomerLocation.Address.PostalCode;
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
        [Route("api/taxi/getdrivers")]
        public List<Driver> getDrivers()
        {
            bool slobodan;
            TaxiRepository.Instance.slobodniVozaci.Clear();
            if (TaxiRepository.Instance.TaxiServiceRepository.Drivers.Count() > 0)
            {
                foreach (var driver in TaxiRepository.Instance.SignedUpD.Values)
                {

                    slobodan = true;
                foreach (var item in TaxiRepository.Instance.AllRides)
                {
                        if (item.Driver.Username != null)
                        {
                            if (item.Status == RideStatus.Accepted && item.Driver.Username == driver.Username)
                            {
                                slobodan = false;
                            }
                            if (slobodan)
                            {
                                TaxiRepository.Instance.slobodniVozaci.Add(driver);
                                // ride.Status = RideStatus.Accepted;
                                //item.Rides.Add(ride);
                                //TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                            }
                        }
                }
                }

                return TaxiRepository.Instance.slobodniVozaci;
            }

            return new List<Driver>();
        }
        [HttpGet]
        [Route("api/taxi/getrides")]
        public List<Ride> getRides()
        {

            //slobodniVozaci.Clear();
            TaxiRepository.Instance.ridesSl.Clear();
            if (TaxiRepository.Instance.dispecherLogged)
            {
                foreach (var item in TaxiRepository.Instance.AllRides)
                {

                    if (item.Status == RideStatus.Ordered || item.Status == RideStatus.Processed)
                    {
                        TaxiRepository.Instance.ridesSl.Add(item);
                    }


                }
            }
            else if (TaxiRepository.Instance.driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.AllRides)
                {

                    if (item.Status == RideStatus.Ordered)
                    {
                        TaxiRepository.Instance.ridesSl.Add(item);
                    }


                }
            }

            return TaxiRepository.Instance.ridesSl;

        }
        //select driver thats gonna accept the ride
        [HttpGet]
        [Route("api/taxi/selectdriver")]
        public IHttpActionResult SelectDriver([FromUri]int i)
        {
            bool found = false;
            if (TaxiRepository.Instance.dispNapravioVoznju)
            {
                TaxiRepository.Instance.slobodniVozaci[i].Rides.Add(TaxiRepository.Instance.dispRide);
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Dispatcher.Username != null || item.Dispatcher.Username != "")
                    {

                        if (TaxiRepository.Instance.dispRide.Dispatcher.Username == item.Dispatcher.Username)
                        {
                            TaxiRepository.Instance.dispRide.Status = RideStatus.Accepted;
                            TaxiRepository.Instance.dispRide.Driver = TaxiRepository.Instance.slobodniVozaci[i];
                            item.Driver = TaxiRepository.Instance.slobodniVozaci[i];
                            //TaxiRepository.Instance.TaxiServiceRepository.Rides.Add(TaxiRepository.Instance.dispRide);
                            item.Status = RideStatus.Accepted;
                            found = true;
                            break;
                        }
                    }
                }
                if (found)
                {
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                }
                foreach (var item in TaxiRepository.Instance.AllRides)
                {
                    if (item.Dispatcher.Username != null)
                    {
                        if (TaxiRepository.Instance.dispRide.Dispatcher.Username == item.Dispatcher.Username)
                        {
                            TaxiRepository.Instance.dispRide.Status = RideStatus.Accepted;
                            //TaxiRepository.Instance.AllRides.Add(TaxiRepository.Instance.dispRide);
                            item.Driver = TaxiRepository.Instance.slobodniVozaci[i];
                            item.Status = RideStatus.Accepted;
                        }
                    }
                }

             
            }
            else
            {

                TaxiRepository.Instance.selectedRideToAssign.Status = RideStatus.Accepted;
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Customer.Username != null || item.Customer.Username != "")
                    {

                        if (item.Customer.Username == TaxiRepository.Instance.selectedRideToAssign.Customer.Username)
                        {
                            item.Driver = TaxiRepository.Instance.selectedRideToAssign.Driver;
                            item.Status = RideStatus.Accepted;
                            break;
                        }
                    }
                }
                foreach (var item in TaxiRepository.Instance.AllRides)
                {
                    if (item.Customer.Username != null || item.Customer.Username != "")
                    {

                        if (item.Customer.Username == TaxiRepository.Instance.selectedRideToAssign.Customer.Username)
                        {
                            item.Status = RideStatus.Accepted;
                            item.Driver = TaxiRepository.Instance.selectedRideToAssign.Driver;
                            break;
                        }
                    }
                }

                TaxiRepository.Instance.slobodniVozaci[i].Rides.Add(TaxiRepository.Instance.selectedRideToAssign);
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

            }
            return Ok();
        }
        [HttpGet]
        [Route("api/taxi/selectride")]
        public IHttpActionResult SelectRide([FromUri]int i)
        {

            TaxiRepository.Instance.selectedRideToAssign = TaxiRepository.Instance.ridesSl[i];
            
            return Ok();

        }
        [HttpGet]
        [Route("api/taxi/getridesmain")]
        public List<Ride> getRidesMain()
        {
            TaxiRepository.Instance.GetridesMain.Clear();
            if (TaxiRepository.Instance.userLogged)
            {
                if (TaxiRepository.Instance.AllRides.Count() >0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {if (item.Customer != null)
                        {

                            if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.GetridesMain.Add(item);
                        }
                        }
                }

            }
            else if (TaxiRepository.Instance.dispecherLogged)
            {
                if (TaxiRepository.Instance.AllRides.Count() > 0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (item.Dispatcher != null)
                        {

                            if (item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username)
                            {
                                TaxiRepository.Instance.GetridesMain.Add(item);

                            }
                        }
                    }
                }

            }
            else if (TaxiRepository.Instance.driverLogged)
            {
                if (TaxiRepository.Instance.AllRides.Count() > 0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (item.Driver != null)
                        {
                            if (item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.GetridesMain.Add(item);
                        }
                        }
                }

            }

            return TaxiRepository.Instance.GetridesMain;
        }
        [HttpPost]
        [Route("api/taxi/cancelride")]
        public IHttpActionResult CancelRide([FromBody]int i)
        {
            if (TaxiRepository.Instance.GetridesMain[i].Status != RideStatus.Accepted)
            {
              

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Customer.Username != null || item.Customer.Username != "")
                    {
                        if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username)
                        {
                            item.Status = RideStatus.Canceled;

                        }
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                foreach (var item in TaxiRepository.Instance.AllRides)
                {
                    if (item.Customer.Username != null || item.Customer.Username != "")
                    {
                        if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username)
                        {
                            item.Status = RideStatus.Canceled;

                        }
                    }
                }

                return Ok();
            }
            return NotFound();
        }
        [HttpPost]
        [Route("api/taxi/comment")]
        public IHttpActionResult Comment([FromBody]Comment Comment)
        {
            

            foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
            {
                if (item.Customer.Username != null || item.Customer.Username != "" || item.Comment.Description != "")
                {
                    if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username)
                    {
                        item.Comment.PublishDate = DateTime.Now.Date;
                        item.Comment.Description = Comment.Description;
                        item.Comment.Rate = Comment.Rate;
                        item.Comment.Customer = TaxiRepository.Instance.getUser(TaxiRepository.Instance.signedIn.Username);

                    }
                }
            }

            TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

            foreach (var item in TaxiRepository.Instance.AllRides)
            {
                if (item.Customer.Username != null || item.Customer.Username != "" || item.Comment.Description != "")
                {
                    if (item.Customer.Username == TaxiRepository.Instance.signedIn.Username)
                    {
                        item.Comment.PublishDate = DateTime.Now.Date;
                        item.Comment.Description = Comment.Description;
                        item.Comment.Rate = Comment.Rate;
                        item.Comment.Customer = TaxiRepository.Instance.getUser(TaxiRepository.Instance.signedIn.Username);

                    }
                }
            }

            return Ok();
        }
        [HttpPost]
        [Route("api/taxi/assigndriver")]
        public IHttpActionResult AssignDriver([FromBody]int i)
        {
            if (TaxiRepository.Instance.ridesSl[i].Status == RideStatus.Ordered)
            {
                TaxiRepository.Instance.ridesSl[i].Driver = TaxiRepository.Instance.getDriver(TaxiRepository.Instance.signedInD.Username);
            
                foreach (var ride in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {   if (ride.Customer.Username != "" || ride.Customer.Username != null)
                    {
                        if (ride.Customer.Username == TaxiRepository.Instance.ridesSl[i].Customer.Username)
                        {
                            ride.Status = RideStatus.Accepted;
                        }
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                foreach (var ride in TaxiRepository.Instance.AllRides)
                {
                    if (ride.Customer.Username != "" || ride.Customer.Username != null)
                    {
                        if (ride.Customer.Username == TaxiRepository.Instance.ridesSl[i].Customer.Username)
                        {
                            ride.Status = RideStatus.Accepted;
                        }
                    }
                }
                return Ok();
            }
            return NotFound();
        }
        [HttpGet]
        [Route("api/taxi/currentride")]
        public IHttpActionResult CurrentRide()
        {
           

            foreach (var item in TaxiRepository.Instance.AllRides)
            {
                if(item.Driver.Username == TaxiRepository.Instance.signedInD.Username)
                {
                    if (item.Status == RideStatus.Accepted) return Ok(item);
                }
            }
            
            return NotFound();
        }
        [HttpPost]
        [Route("api/taxi/finishedride")]
        public void FinishedRide(Ride finishedRide)
        {

            if(finishedRide.Status == RideStatus.Successful)
            {
               
                foreach (var item in TaxiRepository.Instance.AllRides)
                {
                    if (item.Driver.Username != null)
                    {

                        if (item.Driver.Username == TaxiRepository.Instance.signedInD.Username)
                        {
                            item.Status = RideStatus.Successful;
                            item.Fare = finishedRide.Fare;
                            item.Destionation.Latitude = finishedRide.CustomerLocation.Latitude;
                            item.Destionation.Longitude = finishedRide.CustomerLocation.Longitude;
                            item.Destionation.Address.Street = finishedRide.CustomerLocation.Address.Street;
                            item.Destionation.Address.Number = finishedRide.CustomerLocation.Address.Number;
                            item.Destionation.Address.City = finishedRide.CustomerLocation.Address.City;
                            item.Destionation.Address.PostalCode = finishedRide.CustomerLocation.Address.PostalCode;
                        }
                    }
                }
                

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Driver.Username != "" || item.Driver.Username != null)
                    {
                        if (item.Driver.Username == TaxiRepository.Instance.signedInD.Username)
                        {
                            item.Status = RideStatus.Successful;
                            item.Fare = finishedRide.Fare;
                            item.Destionation.Latitude = finishedRide.CustomerLocation.Latitude;
                            item.Destionation.Longitude = finishedRide.CustomerLocation.Longitude;
                            item.Destionation.Address.Street = finishedRide.CustomerLocation.Address.Street;
                            item.Destionation.Address.Number = finishedRide.CustomerLocation.Address.Number;
                            item.Destionation.Address.City = finishedRide.CustomerLocation.Address.City;
                            item.Destionation.Address.PostalCode = finishedRide.CustomerLocation.Address.PostalCode;

                        }
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
            }
            else
            {

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Drivers)
                {
                    if (item.Username != null)
                    {
                        if (TaxiRepository.Instance.signedInD.Username == item.Username)
                        {
                            foreach (var ride in item.Rides)
                            {
                                ride.Status = RideStatus.Unsuccessful;
                                ride.Comment.Description = finishedRide.Comment.Description;

                            }
                        }
                    }
                }

                foreach (var user in TaxiRepository.Instance.TaxiServiceRepository.Users)
                {
                    foreach (var ride in user.Rides)
                    {
                        if (user.Username != null && ride.Customer.Username != null)
                        {
                            if (user.Username == ride.Customer.Username)
                            {
                                ride.Status = RideStatus.Unsuccessful;
                                ride.Comment.Description = finishedRide.Comment.Description;
                            }
                        }
                    }
                }

                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {if (item.Driver.Username != null)
                    {
                        if (item.Driver.Username == TaxiRepository.Instance.signedInD.Username)
                        {
                            item.Status = RideStatus.Unsuccessful;
                            item.Comment.Description = finishedRide.Comment.Description;
                        }
                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
            }
        }
        [HttpGet]
        [Route("api/taxi/allrides")]
        public List<Ride> AllRides()
        {

            return TaxiRepository.Instance.AllRides;
        }

        [HttpPost]
        [Route("api/taxi/filter")]
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
        [Route("api/taxi/sort")]
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
        [Route("api/taxi/search")]
        public List<Ride> Search([FromBody]Dodatno dodatno)
        {
            TaxiRepository.Instance.RidesDisp.Clear();
            if (dodatno.SearchOcena)
            {
                if(dodatno.OcenaDo != 0 && dodatno.OcenaOd != 0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer!= null)
                        {
                            if (item.Comment.Rate <= dodatno.OcenaDo && item.Comment.Rate >= dodatno.OcenaOd && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Comment.Rate <= dodatno.OcenaDo && item.Comment.Rate >= dodatno.OcenaOd && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                        
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Comment.Rate <= dodatno.OcenaDo && item.Comment.Rate >= dodatno.OcenaOd && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                    }
                }
                else if(dodatno.OcenaOd != 0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Comment.Rate >= dodatno.OcenaOd && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Comment.Rate >= dodatno.OcenaOd && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                   
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Comment.Rate >= dodatno.OcenaOd && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                    }
                }
                else if(dodatno.OcenaDo != 0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Comment.Rate <= dodatno.OcenaDo  && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher!=null)
                        {
                            if (item.Comment.Rate <= dodatno.OcenaDo  && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                           
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Comment.Rate <= dodatno.OcenaDo  && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                           
                        }
                    }
                }
                return TaxiRepository.Instance.RidesDisp;
            }
            if (dodatno.SearchCena)
            {
                if(dodatno.CenaDo !=0 && dodatno.CenaOd != 0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Fare <= dodatno.CenaDo && item.Fare >= dodatno.CenaOd && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Fare <= dodatno.CenaDo && item.Fare >= dodatno.CenaOd && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver!= null)
                        {
                            if (item.Fare <= dodatno.CenaDo && item.Fare >= dodatno.CenaOd && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                    }
                }
                else if(dodatno.CenaOd != 0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Fare >= dodatno.CenaOd && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Fare >= dodatno.CenaOd && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Fare >= dodatno.CenaOd && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                    }
                }
                else if(dodatno.CenaDo != 0)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Fare <= dodatno.CenaDo && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Fare <= dodatno.CenaDo && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Fare <= dodatno.CenaDo && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                           
                        }
                    }
                }
                return TaxiRepository.Instance.RidesDisp;
            }
            if (dodatno.SearchDatum)
            {
                if(dodatno.DatumDo != null && dodatno.DatumOd != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer!= null)
                        {
                            if (item.OrderTime.Date <= dodatno.DatumDo.Date && item.OrderTime.Date >= dodatno.DatumOd.Date && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.OrderTime.Date <= dodatno.DatumDo.Date && item.OrderTime.Date >= dodatno.DatumOd.Date && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.OrderTime.Date <= dodatno.DatumDo.Date && item.OrderTime.Date >= dodatno.DatumOd.Date && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                    }
                }
                else if(dodatno.DatumDo != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.OrderTime.Date <= dodatno.DatumDo.Date  && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.OrderTime.Date <= dodatno.DatumDo.Date  && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                           
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.OrderTime.Date <= dodatno.DatumDo.Date  && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                    }
                }
                else if(dodatno.DatumOd != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.OrderTime.Date >= dodatno.DatumOd.Date && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.OrderTime.Date >= dodatno.DatumOd.Date && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                     
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.OrderTime.Date >= dodatno.DatumOd.Date && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                    }
                }
                return TaxiRepository.Instance.RidesDisp;
            }
            if (dodatno.SearchC)
            {
                if(dodatno.imeMusterije != null && dodatno.prezimeMusterije != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Customer.Name.Contains(dodatno.imeMusterije) && item.Customer.LastName.Contains(dodatno.prezimeMusterije) && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Customer.Name.Contains(dodatno.imeMusterije) && item.Customer.LastName.Contains(dodatno.prezimeMusterije) && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Customer.Name.Contains(dodatno.imeMusterije) && item.Customer.LastName.Contains(dodatno.prezimeMusterije)&& item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                    }
                }
                else if(dodatno.imeMusterije != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Customer.Name.Contains(dodatno.imeMusterije) && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Customer.Name.Contains(dodatno.imeMusterije)  && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Customer.Name.Contains(dodatno.imeMusterije) && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                        
                        }
                    }
                }
                else if(dodatno.prezimeMusterije != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if ( item.Customer.LastName.Contains(dodatno.prezimeMusterije) && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Customer.LastName.Contains(dodatno.prezimeMusterije) && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Customer.LastName.Contains(dodatno.prezimeMusterije) && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
              
                        }
                    }
                }
                return TaxiRepository.Instance.RidesDisp;
            }
           if (dodatno.SearchD)
            {
                if (dodatno.imeVozaca != null && dodatno.prezimeVozaca != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca) && item.Driver.LastName.Contains(dodatno.prezimeVozaca) && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca) && item.Driver.LastName.Contains(dodatno.prezimeVozaca) && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca) && item.Driver.LastName.Contains(dodatno.prezimeVozaca) && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                           
                        }
                    }
                }
                else if (dodatno.imeVozaca != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca)  && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca)  && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                         
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null)
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca)  && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                           
                        }
                    }
                }
                else if (dodatno.prezimeVozaca != null)
                {
                    foreach (var item in TaxiRepository.Instance.AllRides)
                    {
                        if (TaxiRepository.Instance.userLogged && item.Customer != null)
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca) && item.Customer.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);

                        }
                        else if (TaxiRepository.Instance.dispecherLogged && item.Dispatcher != null)
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca) && item.Dispatcher.Username == TaxiRepository.Instance.signedIn.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                            
                        }
                        else if (TaxiRepository.Instance.driverLogged && item.Driver != null) 
                        {
                            if (item.Driver.Name.Contains(dodatno.imeVozaca) && item.Driver.Username == TaxiRepository.Instance.signedInD.Username) TaxiRepository.Instance.RidesDisp.Add(item);
                          
                        }
                    }
                }
                return TaxiRepository.Instance.RidesDisp;
            }

            return null;
        }

        private List<Ride> getAllRides(string username)
        {
            List<Ride> rides = new List<Ride>();
            if (TaxiRepository.Instance.dispecherLogged)
            {

            }else if (TaxiRepository.Instance.userLogged)
            {
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                    if (item.Customer.Username == username) rides.Add(item);
                }
            }

            return rides;
        
        }

        [HttpPost]
        [Route("api/taxi/block")]
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
        [Route("api/taxi/unblock")]
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
        [Route("api/taxi/getUsersDistance")]
        public List<Ride> getUsersDistance()
        {
            List<Ride> rides = new List<Ride>();
      
            List<double> distances = new List<double>();
            if (TaxiRepository.Instance.driverLogged)
            {
                foreach (var item in TaxiRepository.Instance.AllRides)
                {
                    if(item.Driver != null && item.Driver.Username == TaxiRepository.Instance.signedInD.Username)
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
    }
}