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
        public User signedIn = new User();
        public Driver signedInD = new Driver();
        public List<Driver> slobodniVozaci = new List<Driver>();
        public Ride dispRide = new Ride();
        public bool userLogged = false;
        public bool driverLogged = false;
        public bool dispecherLogged = false;
        public bool dispNapravioVoznju = false;
        public List<Ride> ridesSl = new List<Ride>();
        public Ride selectedRideToAssign = new Ride();
        public List<Ride> GetridesMain = new List<Ride>();
        public List<Ride> RidesDisp= new List<Ride>();
        public List<Ride> RideUser = new List<Ride>();
        public List<Ride> RideDriver = new List<Ride>();
        public Dictionary<string, User> SignedUp { get; set; } = new Dictionary<string, User>();
        public Dictionary<string, Driver> SignedUpD { get; set; } = new Dictionary<string, Driver>();
        public List<Ride> AllRides { get; set; } = new List<Ride>();
        public Dictionary<int, Car> allCars { get; set; } = new Dictionary<int, Car>();
        public bool UserExists(string username)
        {
            var user = TaxiRepository.Instance.getUser(username);
            if (user == null)
                return false;
            return true;

        }
        public bool UserLogin(string username, string password)
        {
            var user = TaxiRepository.Instance.getUser(username);
            if (user == null)
                return false;

            if(user.Password == password)
            {
                return true;
            }
            return false;
        }
        public bool CarExists(int carNumber)
        {
            var user = TaxiRepository.Instance.getCar(carNumber);
            if (user == null)
                return false;
            return true;

        }

        public bool DriverExists(string username)
        {
            var user = TaxiRepository.Instance.getDriver(username);
            if (user == null)
                return false;
            return true;
        }
        public bool DriverLogin(string username, string password)
        {
            var user = TaxiRepository.Instance.getDriver(username);
            if (user == null)
                return false;

            if (user.Password == password)
            {
                return true;
            }
            return false;
        }
        public Car getCar(int carNumber)
        {
            return Instance.TaxiServiceRepository.Cars.FirstOrDefault(x => x.CarNumber == carNumber);
        }
        public User getUser(string username)
        {
            return Instance._taxiServiceRepository.Users.FirstOrDefault(x => x.Username == username);
        }
       
        public Ride getRide(string username)
        {
            return (from ride in TaxiServiceRepository.Rides
                    where ride.Customer.Username == username || ride.Driver.Username == username || ride.Dispatcher.Username == username
                    select ride).ToList().First();
                    
        }

        public Driver getDriver(string username)
        {
            return Instance._taxiServiceRepository.Drivers.Include("Location").Include("Location.Address").FirstOrDefault(x => x.Username == username);

        }

        public List<Ride> SortRideTime()
        {
            //treba po opadajucem
            return (from ride in TaxiServiceRepository.Rides orderby ride.OrderTime select ride).ToList();
        }

        public List<Ride> SortRideRate()
        {
            //isto opadajuecm
            return (from ride in TaxiServiceRepository.Rides orderby ride.Comment.Rate select ride).ToList();
        }
    }
}