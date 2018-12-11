using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace MVCMensa3.Models
{
    public class Kategorie
    {
        public int ID { get; set; }
        public string Bezeichnung { get; set; }
        public int? Oberkategorie { get; set; }
        public int? BildId { get; set; }
        public int PreisId { get; set; }

        public Kategorie(int id, string bezeichnung, int? oberkategorie, int? bildId, int preisId)
        {
            ID = id;
            Bezeichnung = bezeichnung;
            Oberkategorie = oberkategorie;
            BildId = bildId;
            PreisId = preisId;
        }

        public static Kategorie GetKategorie(int id)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT kategorien.ID as KategorieID, kategorien.Bezeichnung, kategorien.Oberkategorie, kategorien.BildID, kategorien.PreisID FROM kategorien " +
                                      "WHERE kategorien.ID = @id";
                    cmd.Parameters.AddWithValue("id", id);

                    var r = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);

                    if (r.Read())
                    {
                        int kategorieId = r.GetInt32(r.GetOrdinal("KategorieID"));
                        string bezeichnung = r.GetString(r.GetOrdinal("Bezeichnung"));

                        int oberkatCol = r.GetOrdinal("Oberkategorie");
                        int? oberkategorie = r.IsDBNull(oberkatCol) ? null : r.GetInt32(oberkatCol) as int?;
                        int bildCol = r.GetOrdinal("BildID");
                        int? bildId = r.IsDBNull(bildCol) ? null : r.GetInt32(bildCol) as int?;
                        int preisId = r.GetInt32(r.GetOrdinal("PreisID"));

                        return new Kategorie(kategorieId, bezeichnung, oberkategorie, bildId, preisId);
                    }

                    return null;
                }
            }
        }

        public static List<Kategorie> GetKategorieList()
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT kategorien.ID as KategorieID, kategorien.Bezeichnung, kategorien.Oberkategorie, kategorien.BildID, kategorien.PreisID FROM kategorien";

                    List<Kategorie> res = new List<Kategorie>();

                    var r = cmd.ExecuteReader();
                    while (r.Read())
                    {
                        int kategorieId = r.GetInt32(r.GetOrdinal("KategorieID")); //(r["KategorieID"] as int?).Value;
                        string bezeichnung = r["Bezeichnung"] as string;

                        int? oberkategorie;
                        int oberkatIndex = r.GetOrdinal("Oberkategorie");
                        if (r.IsDBNull(oberkatIndex))
                        {
                            oberkategorie = null;
                        }
                        else
                        {
                            oberkategorie = r.GetInt32(oberkatIndex);
                        }

                        int bildIndex = r.GetOrdinal("BildID");
                        int? bildId = r.IsDBNull(bildIndex) ? null : r.GetInt32(bildIndex) as int?;


                        int preisId = r.GetInt32(r.GetOrdinal("PreisID"));

                        res.Add(new Kategorie(kategorieId, bezeichnung, oberkategorie, bildId, preisId));
                    }

                    return res;
                }
            }
        }
    }
}