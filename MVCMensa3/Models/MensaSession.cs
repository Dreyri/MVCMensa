using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models
{
    public class MensaSession
    {
        public const string SessionUser = "user";
        public const string SessionRole = "role";

        public const string Mitarbeiter = "Mitarbeiter";
        public const string Gast = "Gast";
        public const string Student = "Student";
        public const string Unknown = "Unknown";

        public string User { get; set; }
        public Role Rolle { get; set; }

        public enum Role
        {
            STUDENT,
            MITARBEITER,
            GAST,
            NONE
        }

        public static Role RoleFromString(string rolle)
        {
            if (rolle == Gast)
            {
                return Role.GAST;
            }
            else if (rolle == Mitarbeiter)
            {
                return Role.MITARBEITER;
            }
            else if (rolle == Student)
            {
                return Role.STUDENT;
            }
            else
            {
                return Role.NONE;
            }
        }

        public static string RoleToString(Role rolle)
        {
            switch (rolle)
            {
                case Role.GAST:
                    return Gast;
                case Role.MITARBEITER:
                    return Mitarbeiter;
                case Role.STUDENT:
                    return Student;
                default:
                    return Unknown;
            }
        }

        public MensaSession(string user, string rolle)
        {
            User = user;
            Rolle = RoleFromString(rolle);
        }

        public MensaSession(HttpRequestBase req)
        {
            User = req.Params[SessionUser];
            Rolle = RoleFromString(req.Params[SessionRole]);
        }

        public static decimal DeterminePreis(Role rolle, DataModels.Preise preise)
        {
            var res = preise.Gastpreis;
            switch(rolle)
            {
                case Role.MITARBEITER:
                    res = preise.MAPreis.GetValueOrDefault(res);
                    break;
                case Role.STUDENT:
                    res = preise.Studentenpreis.GetValueOrDefault(res);
                    break;
                default:
                    break;
            }

            return res;
        }

        public decimal DeterminePreis(DataModels.Preise preis)
        {
            return DeterminePreis(Rolle, preis);
        }

        public static MensaSession FromCookie(HttpCookieCollection cookie)
        {
            if (cookie.AllKeys.Contains(SessionUser) && cookie.AllKeys.Contains(SessionRole))
            {
                return new MensaSession(cookie[SessionUser].Value, cookie[SessionRole].Value);
            }
            return null;
        }

        public static MensaSession FromContext()
        {
            return FromCookie(HttpContext.Current.Request.Cookies);
        }

        public static MensaSession Create(HttpCookieCollection cookies, string user, string rolle)
        {
            var userCookie = new HttpCookie(SessionUser, user);
            var roleCookie = new HttpCookie(SessionRole, rolle);

            userCookie.Expires = DateTime.Now.AddDays(1);
            roleCookie.Expires = DateTime.Now.AddDays(1);

            cookies.Add(userCookie);
            cookies.Add(roleCookie);

            return new MensaSession(user, rolle);
        }

        public static void Delete(HttpCookieCollection cookies)
        {
            var userCookie = new HttpCookie(SessionUser, "");
            var roleCookie = new HttpCookie(SessionRole, "");

            userCookie.Expires = DateTime.Now.AddDays(-1);
            roleCookie.Expires = DateTime.Now.AddDays(-1);

            cookies.Add(userCookie);
            cookies.Add(roleCookie);
        }

        public static MensaSession Create(HttpCookieCollection cookies, string user, Role rolle)
        {
            string roleStr = "";

            switch (rolle)
            {
                case Role.GAST:
                    roleStr = Gast;
                    break;
                case Role.MITARBEITER:
                    roleStr = Mitarbeiter;
                    break;
                case Role.STUDENT:
                    roleStr = Student;
                    break;
                default:
                    roleStr = Unknown;
                    break;
            }


            return MensaSession.Create(cookies, user, roleStr);
        }
    }
}