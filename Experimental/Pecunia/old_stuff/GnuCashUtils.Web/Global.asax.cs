using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using GnuCashUtils.Framework;
using GnuCashUtils.Web.Controllers;
using GnuCashUtils.Web.Infrastucture;
using GnuCashUtils.Web.Models;
using MvcContrib.Castle;

namespace GnuCashUtils.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start ()
        {
            ConfigureWindsorContainer();
            RegisterRoutes (RouteTable.Routes);
            RegisterViewEngine();
        }

        private void ConfigureWindsorContainer()
        {
            IWindsorContainer windsorContainer = new WindsorContainer();

            ControllerBuilder.Current.SetControllerFactory (
                new WindsorControllerFactory (windsorContainer));

            Book book = new XmlBookReader (
                Path.Combine (
                    Path.GetDirectoryName (this.Server.MapPath(".")),
                    "Data/Igor.xml")).Read ();

            windsorContainer.RegisterControllers (typeof (AccountsController).Assembly);
            windsorContainer.Kernel.AddComponentInstance("book", book);
            windsorContainer.Register(
                AllTypes
                    .Pick()
                    .FromAssemblyNamed("GnuCashUtils.Web")
                    .If(Component.IsInNamespace("GnuCashUtils.Web.Models"))
                    .WithService.FirstInterface());

            //AccountsRepository accountsRepository = new AccountsRepository();
        }

        private static void RegisterRoutes (RouteCollection routes)
        {
            routes.IgnoreRoute ("{resource}.axd/{*pathInfo}");

            routes.MapRoute (
                "Default",                                              // Route name
                "{controller}/{action}/{id}",                           // URL with parameters
                new { controller = "Accounts", action = "List", id = "" }  // Parameter defaults
                );
        }

        private static void RegisterViewEngine()
        {
            ViewEngines.Engines.Insert(0, new VidiViewEngine());
        }

        private static void RegisterWindsorControllerFactory(IWindsorContainer container)
        {
        }
    }
}