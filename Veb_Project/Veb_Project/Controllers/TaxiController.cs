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
        //Done:Template for SignIn,Options,SignOff,SignUp
        //TO DO: Add driver,Add car,Add car to driver,logic

        private User signedIn = new User();
        public IHttpActionResult SignIn(string username,string password,bool checkBox)
        {
            if (!TaxiRepository.Instance.UserExists(username,password))
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
            if (!TaxiRepository.Instance.UserExists(user.Username, user.Password))
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


    }
}