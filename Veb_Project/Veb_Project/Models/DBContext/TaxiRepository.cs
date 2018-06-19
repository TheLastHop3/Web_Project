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


        public Dictionary<string, User> LoggedInUsers { get; set; } = new Dictionary<string, User>();

        public bool UserExists(string username)
        {
            return (from user in _taxiServiceRepository.Users
                    where user.Username == username
                    select user).ToArray().Length != 0;
            
        }
        public bool CarExists(int carNumber)
        {
            return (from car in _taxiServiceRepository.Cars
                    where car.CarNumber == carNumber
                    select car).ToArray().Length != 0;
        }
    }
}