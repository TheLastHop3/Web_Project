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
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            /*  TaxiServiceContext db = new TaxiServiceContext("connstring");
              db.users.Add(new Models.User() { Username = "dajiodja" });
              db.SaveChanges();
              */
         
            var repository = TaxiRepository.Instance.TaxiServiceRepository;
            foreach (var item in repository.Users)
            {
                if (item.Role == UserRole.Dispatcher);
                repository.Users.Remove(item);

            }
            repository.SaveChanges();
            LoadDispechers(@"C:\Users\andre\Desktop\Web_Project\Veb_Project\Veb_Project\App_Data\Dispechers.txt");
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
                    if (!TaxiRepository.Instance.SignedUp.Keys.Contains(dispData[0]))
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
