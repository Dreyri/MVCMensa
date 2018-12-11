using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models
{
    public class LoginFeedback
    {
        public string SavedUser { get; set; }
        public string ErrorInfo { get; set; }
        public bool Success { get; set; }

        public LoginFeedback(string user, string error, bool success = false)
        {
            SavedUser = user;
            ErrorInfo = error;
            Success = success;
        }

        public static LoginFeedback TryLogin(string user, string pw)
        {
            var nutzer = Benutzer.GetBenutzerFromName(user);

            if (nutzer != null)
            {
                if (PasswordStorage.VerifyPassword(pw, string.Format("sha1:64000:18:{0}:{1}", nutzer.Salt, nutzer.Hashk)))
                {
                    if (!nutzer.Aktiv)
                    {
                        return new LoginFeedback(user, "Benutzer ist noch nicht aktiviert");
                    }

                    return new LoginFeedback(user, null, true);
                }

            }

            return new LoginFeedback(user, "Falsches Passwort oder Nutzer.");
        }
    }
}