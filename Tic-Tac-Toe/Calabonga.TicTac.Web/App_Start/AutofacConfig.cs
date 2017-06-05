using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using Calabonga.TicTac.Web.infrastructure;
using Calabonga.TicTac.Web.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Security;

namespace Calabonga.TicTac.Web
{
    public static class AutofacConfig
    {
        public static void Initialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(Assembly.GetExecutingAssembly());
            builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly).AsImplementedInterfaces();
            builder.RegisterModule(new AutofacWebTypesModule());
            builder.RegisterFilterProvider();

            builder.RegisterType<ApplicationDbContext>().InstancePerRequest();
            builder.Register(c => new UserStore<ApplicationUser>((ApplicationDbContext)c.Resolve<ApplicationDbContext>()))
                .AsImplementedInterfaces();

            builder.Register(c => HttpContext.Current.GetOwinContext().Authentication).As<IAuthenticationManager>()
            .InstancePerRequest();

            builder.RegisterType<ApplicationUserManager>();
            builder.RegisterType<ApplicationSignInManager>();

            builder.RegisterType<CookieService>().As<ICookieService>();
            builder.RegisterInstance(UserConnectionManager.Instance).ExternallyOwned();
            builder.RegisterInstance(GameManager.Instance).ExternallyOwned();

            //------------------------------------------------------------
            // SignalR begin
            //------------------------------------------------------------
            builder.RegisterType<ConnectionHub>().ExternallyOwned();
            builder.RegisterType<GameHub>().ExternallyOwned();
            //------------------------------------------------------------
            // SignalR end
            //------------------------------------------------------------

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            GlobalHost.DependencyResolver = new Autofac.Integration.SignalR.AutofacDependencyResolver(container);
        }
    }
}