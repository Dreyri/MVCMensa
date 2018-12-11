using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace MVCMensa3.Models
{
    public class Bild
    {
        public int ID { get; set; }
        public string Titel { get; set; }
        public string AltTitel { get; set; }
        public string Binaerdaten { get; set; }

        public Bild(int id, string titel, string alt, string binary)
        {
            ID = id;
            Titel = titel;
            AltTitel = alt;
            Binaerdaten = binary;
        }

        public static Bild GetFirstBildForMahlzeit(int mahlzeitId)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();

                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT bilder.ID as BildID, bilder.AltText, bilder.Titel, bilder.Binaerdaten FROM mahlzeitenbilder " +
                                      "LEFT JOIN bilder ON bilder.ID = mahlzeitenbilder.BildID " +
                                      "WHERE mahlzeitenbilder.MahlzeitID = @id";

                    cmd.Parameters.AddWithValue("id", mahlzeitId);

                    var r = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);

                    if (r.Read())
                    {
                        int bildId = r.GetInt32(r.GetOrdinal("BildID"));

                        string altText = r.GetString(r.GetOrdinal("AltText"));
                        string titel = r.GetString(r.GetOrdinal("Titel"));
                        string binaerdaten = r.GetString(r.GetOrdinal("Binaerdaten"));

                        return new Bild(bildId, titel, altText, binaerdaten);
                    }

                    return null;
                }
            }
        }
    }
}