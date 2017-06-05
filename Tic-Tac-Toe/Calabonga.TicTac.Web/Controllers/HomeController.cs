using System.Threading;
using System.Web;
using System.Web.Mvc;
using Calabonga.TicTac.Web.infrastructure;

namespace Calabonga.TicTac.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ICookieService _cookieService;

        // DI Container enabled
        //public HomeController(): this(new CookieService()){}

        public HomeController(ICookieService cookieService )
        {
            _cookieService = cookieService;
        }

        public ActionResult Index()
        {
            var cook = _cookieService.GetCookie(WebApp.LanguageCookieName);
            if (cook== null)
            {
                _cookieService.SetCookie(WebApp.LanguageCookieName, Thread.CurrentThread.CurrentCulture.Name);
            }
            return View();
        }

        
    }
}