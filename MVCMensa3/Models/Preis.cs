using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MySql.Data.MySqlClient;

namespace MVCMensa3.Models
{
    public class Preis
    {
        public int ID { get; set; }
        public int Jahr { get; set; }
        public decimal GastPreis { get; set; }
        public decimal? StudentPreis { get; set; }
        public decimal? MAPreis { get; set; }

        public Preis(int id, decimal gast, decimal? student, decimal? ma, int jahr = 2018)
        {
            ID = id;
            Jahr = jahr;
            GastPreis = gast;
            StudentPreis = student;
            MAPreis = ma;
        }

        public decimal GetForRole(MensaSession.Role rolle)
        {
            decimal res = GastPreis;

            switch (rolle)
            {
                case MensaSession.Role.MITARBEITER:
                    if (MAPreis.HasValue)
                    {
                        res = MAPreis.Value;
                    }
                    break;
                case MensaSession.Role.STUDENT:
                    if (StudentPreis.HasValue)
                    {
                        res = StudentPreis.Value;
                    }
                    break;
                default:
                    break;

            }

            return res;
        }

        public static Preis GetPreis(int id)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();

                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT preise.ID as PreisID, preise.Jahr, preise.Gastpreis, preise.Studentenpreis, preise.MAPreis from preise " +
                                      "WHERE preise.ID = @id";
                    cmd.Parameters.AddWithValue("id", id);

                    var r = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);

                    if (r.Read())
                    {
                        int preisId = r.GetInt32(r.GetOrdinal("PreisID"));
                        int jahr = r.GetInt32(r.GetOrdinal("Jahr"));
                        decimal gastPreis = r.GetDecimal(r.GetOrdinal("GastPreis"));

                        int studentCol = r.GetOrdinal("Studentenpreis");
                        decimal? studentenPreis = r.IsDBNull(studentCol) ? null : r.GetDecimal(studentCol) as decimal?;

                        int maCol = r.GetOrdinal("MAPreis");
                        decimal? maPreis = r.IsDBNull(maCol) ? null : r.GetDecimal(maCol) as decimal?;

                        return new Preis(preisId, gastPreis, studentenPreis, maPreis, jahr);
                    }

                    return null;
                }
            }
        }
    }
}