using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace MVCMensa3.Models
{
    public class Benutzer
    {
        [Key]
        public int Nummer { get; set; }

        [Required(ErrorMessage = "Nutzername ist noetig")]
        [StringLength(100, ErrorMessage = "Nutzername maximal 100 Zeichen")]
        public string Nutzername { get; set; }

        public string Hashk { get; set; }
        public string Salt { get; set; }
        [Required(ErrorMessage = "Vorname ist noetig")]
        [StringLength(100)]
        public string Vorname { get; set; }
        [Required(ErrorMessage = "Nachname ist noetig")]
        [StringLength(50)]
        public string Nachname { get; set; }

        public bool Aktiv { get; set; }


        public Benutzer()
         : this(0, null, null, null, null, null)
        {

        }

        public Benutzer(int num, string nutzer, string hash, string salt, string vor, string nach, bool aktiv = false)
        {
            Nummer = num;
            Nutzername = nutzer;
            Hashk = hash;
            Salt = salt;
            Vorname = vor;
            Nachname = nach;
            Aktiv = aktiv;
        }

        public static Benutzer GetBenutzerFromName(string nutzername)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();

                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT benutzer.Nummer, benutzer.Nutzername, benutzer.Hashk, benutzer.Salt, benutzer.Vorname, benutzer.Nachname, benutzer.Aktiv FROM benutzer " +
                                    "WHERE Nutzername = @user";

                    cmd.Parameters.AddWithValue("user", nutzername);

                    var r = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);

                    if (r.Read())
                    {
                        int nummer = r.GetInt32(r.GetOrdinal("Nummer"));
                        string nutzer = r.GetString(r.GetOrdinal("Nutzername"));
                        string hashk = r.GetString(r.GetOrdinal("Hashk"));
                        string salt = r.GetString(r.GetOrdinal("Salt"));
                        string vorname = r.GetString(r.GetOrdinal("Vorname"));
                        string nachname = r.GetString(r.GetOrdinal("Nachname"));
                        bool aktiv = r.GetBoolean(r.GetOrdinal("Aktiv"));

                        return new Benutzer(nummer, nutzer, hashk, salt, vorname, nachname, aktiv);
                    }

                    return null;
                }
            }
        }

        public static MensaSession.Role GetRole(int nummer)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT Rolle FROM v_Rolle WHERE Nummer = @num";
                    cmd.Parameters.AddWithValue("num", nummer);

                    var r = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);
                    if (r.Read())
                    {
                        string role = r.GetString(r.GetOrdinal("Rolle"));

                        return MensaSession.RoleFromString(role);
                    }

                    return MensaSession.Role.NONE;
                }
            }
        }

        public static Benutzer GetBenutzer(int id)
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                using (MySqlCommand cmd = c.CreateCommand())
                {
                    cmd.CommandText = "SELECT benutzer.Nummer, benutzer.Nutzername, benutzer.Hashk, benutzer.Salt, benutzer.Vorname, benutzer.Nachname FROM benutzer " +
                                    "WHERE Nummer = @id";
                    cmd.Parameters.AddWithValue("id", id);

                    var r = cmd.ExecuteReader(System.Data.CommandBehavior.SingleResult);

                    if (r.Read())
                    {
                        int nummer = r.GetInt32(r.GetOrdinal("Nummer"));
                        string nutzer = r.GetString(r.GetOrdinal("Nutzername"));
                        string hashk = r.GetString(r.GetOrdinal("Hashk"));
                        string salt = r.GetString(r.GetOrdinal("Salt"));
                        string vorname = r.GetString(r.GetOrdinal("Vorname"));
                        string nachname = r.GetString(r.GetOrdinal("Nachname"));

                        return new Benutzer(nummer, nutzer, hashk, salt, vorname, nachname);
                    }

                    return null;
                }
            }
        }
    }


}