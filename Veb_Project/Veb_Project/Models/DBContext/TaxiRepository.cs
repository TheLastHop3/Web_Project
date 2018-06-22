using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Veb_Project.Models.DBContext
{
    public class TaxiRepository
    {
        private static TaxiRepository _instance;
        private static object _syncLock = new object();
        private TaxiServiceContext _taxiServiceRepository;

        public TaxiServiceContext TaxiServiceRepository
        {
            get { return _taxiServiceRepository; }
        }
        private TaxiRepository()
        {
            _taxiServiceRepository = new TaxiServiceContext("TaxiConnectionString");
        }
        public static TaxiRepository Instance {
            get
            {
                if(_instance == null)
                {
                    lock (_syncLock)
                    {
                        if(_instance == null)
                        {
                            _instance = new TaxiRepository();
                        }
                    }
                }
                return _instance;
            }
        }


        public Dictionary<string, User> SignedUp { get; set; } = new Dictionary<string, User>();

        public bool UserExists(string username)
        {
            return (from user in TaxiServiceRepository.Users
                    where user.Username == username
                    select user).ToArray().Length != 0;
            
        }
        public bool UserLogin(string username, string password)
        {
            return (from user in TaxiServiceRepository.Users
                    where user.Username == username && user.Password == password
                    select user).ToArray().Length != 0;

        }
        public bool CarExists(int carNumber)
        {
            return (from car in TaxiServiceRepository.Cars
                    where car.CarNumber == carNumber
                    select car).ToArray().Length != 0;
        }

        public bool DriverExists(string username)
        {
            return (from driver in TaxiServiceRepository.Drivers
                    where driver.Username == username
                    select driver).ToArray().Length != 0;
        }
        public bool DriverLogin(string username, string password)
        {
            return (from driver in TaxiServiceRepository.Drivers
                    where driver.Username == username && driver.Password == password
                    select driver).ToArray().Length != 0;
        }
        public User getUser(string username)
        {
            return (from user in TaxiServiceRepository.Users
                    where user.Username == username
                    select user).ToList().First();
        }
        
    }
}