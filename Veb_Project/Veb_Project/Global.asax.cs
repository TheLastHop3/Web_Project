using System;
using System.Collections.Generic;
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
            repository.Users.Add(new Models.User() { Username = "Andrej" ,Name = "Andrej", LastName ="Iviciak", JMBG ="", Email = ""});
            repository.SaveChanges();
        }
    }
}
