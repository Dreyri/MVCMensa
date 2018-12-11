using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace MVCMensa3.Models
{
    public class Mahlzeit
    {
        public int ID { get; }
        public string Name { get; }

        public string Beschreibung { get; set; }
        public int Vorrat { get; set; }
        public bool Verfuegbar
        {
            get
            {
                return Vorrat > 0;
            }
        }
        //public List<Bild> Bilder { get; set; }
        //public List<Zutat> Zutaten { get; set; }
        //public List<Deklaration> Deklarationen { get; set; }
        public int KategorieId { get; set; }
        public int PreisId { get; set; }

        public Mahlzeit(int id, string name, string beschr, int vorrat, int kategorieId, int preisId)
        {
            ID = id;
            Name = name;
            Beschreibung = beschr;
            Vorrat = vorrat;
            KategorieId = kategorieId;
            PreisId = preisId;
        }

        public static Mahlzeit GetMahlzeit(int id)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT mahlzeiten.ID as MahlzeitID, mahlzeiten.Name, mahlzeiten.Vorrat, mahlzeiten.Verfuegbar, mahlzeiten.KategorieID, mahlzeiten.PreisID, mahlzeiten.Beschreibung FROM mahlzeiten " +
                        "WHERE mahlzeiten.ID = @id";
                    cmd.Parameters.AddWithValue("id", id);

                    var r = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);

                    if (r.Read())
                    {
                        int mahlzeitId = r.GetInt32(r.GetOrdinal("MahlzeitID"));
                        string name = r.GetString(r.GetOrdinal("Name"));
                        int vorrat = r.GetInt32(r.GetOrdinal("Vorrat"));
                        bool verfuegbar = r.GetBoolean(r.GetOrdinal("Verfuegbar"));

                        int kategorieId = r.GetInt32(r.GetOrdinal("KategorieID"));
                        int preisId = r.GetInt32(r.GetOrdinal("PreisID"));
                        string beschreibung = r.GetString(r.GetOrdinal("Beschreibung"));

                        return new Mahlzeit(mahlzeitId, name, beschreibung, vorrat, kategorieId, preisId);
                    }

                    return null;
                }
            }
        }

        public static List<Mahlzeit> GetMahlzeitenList(int limit)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT mahlzeiten.ID as MahlzeitID, mahlzeiten.Name, mahlzeiten.Vorrat, mahlzeiten.Verfuegbar, mahlzeiten.KategorieID, mahlzeiten.PreisID, mahlzeiten.Beschreibung FROM mahlzeiten LIMIT @limit";
                    cmd.Parameters.AddWithValue("limit", limit);

                    var r = cmd.ExecuteReader();

                    List<Mahlzeit> mahlzeiten = new List<Mahlzeit>();

                    while (r.Read())
                    {
                        int mahlzeitId = r.GetInt32(r.GetOrdinal("MahlzeitID"));
                        string name = r.GetString(r.GetOrdinal("Name"));
                        int vorrat = r.GetInt32(r.GetOrdinal("Vorrat"));
                        bool verfuegbar = r.GetBoolean(r.GetOrdinal("Verfuegbar"));

                        int kategorieId = r.GetInt32(r.GetOrdinal("KategorieID"));
                        int preisId = r.GetInt32(r.GetOrdinal("PreisID"));
                        string beschreibung = r.GetString(r.GetOrdinal("Beschreibung"));

                        mahlzeiten.Add(new Mahlzeit(mahlzeitId, name, beschreibung, vorrat, kategorieId, preisId));
                    }

                    return mahlzeiten;
                }
            }
        }

        public static List<Mahlzeit> GetMahlzeitenWithFilter(int kategorieId, bool requireVegan, bool requireVegetarisch, bool requireVerfuegbar, int limit)
        {
            List<Mahlzeit> mahlzeiten = Mahlzeit.GetMahlzeitenList(200);
            List<Mahlzeit> filtered = new List<Mahlzeit>();

            foreach (var mahlzeit in mahlzeiten)
            {
                if (mahlzeit.KategorieId != kategorieId && kategorieId != 0) // value 0 used by Alle anzeigen
                {
                    continue;
                }

                if (requireVerfuegbar && !mahlzeit.Verfuegbar) //we require verfuegbar but it's not then cancel this one
                {
                    continue;
                }

                var zutaten = Zutat.GetZutatenListForMahlzeit(mahlzeit.ID);
                bool meetsRequirements = true;

                foreach (var zutat in zutaten)
                {
                    if (requireVegan && !zutat.Vegan)
                    {
                        meetsRequirements = false;
                    }
                    if (requireVegetarisch && !zutat.Vegetarisch)
                    {
                        meetsRequirements = false;
                    }
                }

                if (meetsRequirements)
                {
                    filtered.Add(mahlzeit);
                }
            }

            return filtered;
        }
    }
}