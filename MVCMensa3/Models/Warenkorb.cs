using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCMensa3.Models
{
    public class Warenkorb
    {
        public Dictionary<string, Dictionary<int, int>> Dict { get; set; }

        private Warenkorb(Dictionary<string, Dictionary<int, int>> dict = null)
        {
            if (dict == null)
            {
                Dict = new Dictionary<string, Dictionary<int, int>>();
            }
            else
            {
                Dict = dict;
            }
        }

        public static Warenkorb FromContext()
        {
            var res = new Warenkorb();
            var warenkorbCookie = HttpContext.Current.Request.Cookies.Get("warenkorb");
            if (warenkorbCookie == null)
            {
                return res;
            }

            res.Dict = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<int, int>>>(warenkorbCookie.Value);

            return res;
        }

        public void ChangeMahlzeitAnz(int id, int anz, string user)
        {
            if (!Dict.ContainsKey(user))
            {
                var userDict = new Dictionary<int, int>();
                userDict.Add(id, anz);
                Dict.Add(user, userDict);
            }
            else
            {
                var anzDict = Dict[user];
                if (!anzDict.ContainsKey(id))
                {
                    anzDict.Add(id, anz);
                }
                else
                {
                    // only difference to AddMahlzeit
                    anzDict[id] = anz;
                }
            }
        }

        public void AddMahlzeit(int id, string user)
        {
            if (!Dict.ContainsKey(user))
            {
                var userDict = new Dictionary<int, int>();
                userDict.Add(id, 1);
                Dict.Add(user, userDict);
            }
            else
            {
                var anzDict = Dict[user];
                if (!anzDict.ContainsKey(id))
                {
                    anzDict.Add(id, 1);
                }
                else
                {
                    anzDict[id] += 1;
                }
            }
        }

        public void MergeViewModel(Models.MensaSession session, BestellungViewModel model)
        {
            if (model == null || session == null)
            {
                return;
            }

            if (Dict.ContainsKey(session.User))
            {
                var activeDict = Dict[session.User];

                foreach (var best in model.Bestellungen)
                {
                    if (activeDict.ContainsKey((int)best.ID))
                    {
                        activeDict[(int)best.ID] = best.Anzahl;
                    }
                }
            }
        }

        public void Commit()
        {
            var korbCookie = new HttpCookie("warenkorb")
            {
                Value = JsonConvert.SerializeObject(Dict)
            };
            korbCookie.Expires = DateTime.Now.AddHours(6);
            HttpContext.Current.Response.Cookies.Set(korbCookie);
        }

        public static void DoViewBag(dynamic viewbag)
        {
            var korb = FromContext();
            var session = MensaSession.FromContext();
            if (session != null && korb != null && korb.Dict.ContainsKey(session.User))
            {
                var bestellungCount = 0;
                foreach (var pair in korb.Dict[session.User])
                {
                    bestellungCount += pair.Value;
                }
                viewbag.Bestellungen = bestellungCount;
            }
            else
            {
                viewbag.Bestellungen = 0;
            }
        }
    }

   
}