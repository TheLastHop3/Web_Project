using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using Veb_Project.Models;
using Veb_Project.Models.DBContext;

namespace Veb_Project.Controllers
{
    public class TaxiController : ApiController
    {
        //Done:Template for SignIn,Options,SignOff,SignUp,Details User/Driver,Edit info User/Driver,Edit pw,SignUp Driver
        //TO DO: logic,sort

        private User signedIn = new User();
        private Driver signedInD = new Driver();
        bool userLogged = false;
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
            else
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
    }
}