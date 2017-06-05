using System;
using System.Globalization;
using System.Web;

namespace Calabonga.TicTac.Web.infrastructure
{
    public interface ICookieService
    {
        string GetCookie(string cookieName);
        
        void SetCookie(string cookieName, string cookieValue);

        void SetCookie(string cookieValue);
    }

    public class CookieService: ICookieService
    {
        private const string AppCookieKeyName = "LanguageCookieName";

        /// <summary>
        /// Читать Cookie
        /// </summary>
        /// <param name="cookieName"></param>
        public string GetCookie(string cookieName)
        {
            if ((HttpContext.Current.Request.Cookies[AppCookieKeyName] != null)
                && (HttpContext.Current.Request.Cookies[AppCookieKeyName][cookieName] != null))
            {
                return HttpContext.Current.Request.Cookies[AppCookieKeyName][cookieName].ToString(CultureInfo.InvariantCulture);
            }
            return null;
        }

        /// <summary>
        /// Записать Cookies
        /// </summary>
        /// <param name="cookieName"></param>
        /// <param name="cookieValue"></param>
        public void SetCookie(string cookieName, string cookieValue)
        {
            var cookie = new HttpCookie(AppCookieKeyName);
            cookie[cookieName] = cookieValue;
            cookie.Expires = DateTime.Now.AddDays(30);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        /// <summary>
        /// Записать Cookies
        /// </summary>
        /// <param name="cookieValue"></param>
        public void SetCookie(string cookieValue)
        {
            var cookie = new HttpCookie(AppCookieKeyName);
            cookie[cookieValue.GetType().Name] = cookieValue;
            cookie.Expires = DateTime.Now.AddDays(30);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

    }
}