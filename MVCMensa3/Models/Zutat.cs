using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace MVCMensa3.Models
{
    public class Zutat
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Bio { get; set; }
        public bool Vegetarisch { get; set; }
        public bool Vegan { get; set; }
        public bool Glutenfrei { get; set; }

        public Zutat(int id, string name, bool bio = false, bool vegetarisch = false, bool vegan = false, bool glutenfrei = false)
        {
            ID = id;
            Name = name;
            Bio = bio;
            Vegetarisch = vegetarisch;
            Vegan = vegan;
            Glutenfrei = glutenfrei;
        }

        public static List<Zutat> GetSortedZutaten()
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();

                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT zutaten.ID as ZutatID, zutaten.Name, zutaten.Bio, zutaten.Vegetarisch, zutaten.Vegan, zutaten.Glutenfrei FROM zutaten " +
                        "ORDER BY zutaten.Bio DESC, zutaten.Name ASC";

                    var zutaten = new List<Zutat>();

                    var r = cmd.ExecuteReader();

                    while (r.Read())
                    {
                        int zutatId = r.GetInt32(r.GetOrdinal("ZutatID"));
                        string name = r.GetString(r.GetOrdinal("Name"));
                        bool bio = r.GetBoolean(r.GetOrdinal("Bio"));
                        bool vegetarisch = r.GetBoolean(r.GetOrdinal("Vegetarisch"));
                        bool vegan = r.GetBoolean(r.GetOrdinal("Vegan"));
                        bool glutenfrei = r.GetBoolean(r.GetOrdinal("Glutenfrei"));

                        zutaten.Add(new Zutat(zutatId, name, bio, vegetarisch, vegan, glutenfrei));
                    }

                    return zutaten;
                }
            }
        }

        public static List<Zutat> GetZutatenListForMahlzeit(int mahlzeitId)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();

                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT zutaten.ID as ZutatID, zutaten.Name, zutaten.Bio, zutaten.Vegetarisch, zutaten.Vegan, zutaten.Glutenfrei FROM zutaten " +
                                      "LEFT JOIN mahlzeitenzutat ON mahlzeitenzutat.ZutatID = zutaten.ID " +
                                      "WHERE mahlzeitenzutat.MahlzeitID = @id";
                    cmd.Parameters.AddWithValue("id", mahlzeitId);

                    var r = cmd.ExecuteReader();

                    List<Zutat> zutaten = new List<Zutat>();

                    while (r.Read())
                    {
                        int zutatId = r.GetInt32(r.GetOrdinal("ZutatID"));
                        string name = r.GetString(r.GetOrdinal("Name"));
                        bool bio = r.GetBoolean(r.GetOrdinal("Bio"));
                        bool vegetarisch = r.GetBoolean(r.GetOrdinal("Vegetarisch"));
                        bool vegan = r.GetBoolean(r.GetOrdinal("Vegan"));
                        bool glutenfrei = r.GetBoolean(r.GetOrdinal("Glutenfrei"));

                        zutaten.Add(new Zutat(zutatId, name, bio, vegetarisch, vegan, glutenfrei));
                    }

                    return zutaten;
                }
            }
        }
    }
}