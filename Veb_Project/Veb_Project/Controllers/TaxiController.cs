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
        //Done:Template for SignIn,Options,SignOff,SignUp,Details User/Driver,Edit info User/Driver,Edit pw,SignUp Driver,printMainCustomer,Add ride disp,custom
        //TO DO: cancel ride,rest
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
        public IHttpActionResult SignIn(string username,string password,bool checkBox)
        {
            if (!TaxiRepository.Instance.UserLogin(username,password))
            {
                User user = new User();
                if (checkBox)
                {
                    
                    if(user == null)
                    {
                        user = TaxiRepository.Instance.getUser(username);
                    }

                }
                else
                {
                    
                }
                signedIn = TaxiRepository.Instance.getUser(username);
                return Ok(user);
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
                if(item.Username == signedInD.Username)
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
                    if(item.Username == signedIn.Username)
                    {
                        item.Password = password;
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                        return Ok();
                    }
                        
                }
            }
            else if(driverLogged)
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
        public IHttpActionResult AddRide(Location location,CarType type)
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
                    if(item.Username == signedIn.Username)
                    {
                        item.Rides.Add(ride);
                        TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();

                    }
                }
                TaxiRepository.Instance.TaxiServiceRepository.Rides.Add(ride);
                TaxiRepository.Instance.TaxiServiceRepository.SaveChanges();
                return Ok("Successfuly ordered ride,the available driver will pick you up shorlty");
            }else if (dispecherLogged)
            {
                dispNapravioVoznju = true;
                dispRide.CarType = type;
                dispRide.CustomerLocation = location;
                dispRide.OrderTime = DateTime.Now;
                dispRide.Status = RideStatus.Processed;
                dispRide.Dispatcher = signedIn;
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
            
            slobodniVozaci.Clear();
            
                foreach (var item in TaxiRepository.Instance.TaxiServiceRepository.Rides)
                {
                  
                   if(item.Status != RideStatus.Accepted && item.Status != RideStatus.Succesful && item.Status != RideStatus.Unsuccessful)
                    {
                        ridesSl.Add(item);
                    }
                       
                   
                }      
            return ridesSl;

        }
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
                    if(item.Customer.Username == selectedRideToAssign.Customer.Username)
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

            }else if (dispecherLogged)
            {


            }else if (driverLogged)
            {


            }

            return GetridesMain;
        }
    }
}