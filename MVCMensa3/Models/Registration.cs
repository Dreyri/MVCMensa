using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;

namespace MVCMensa3.Models
{
    public class Registration
    {
       
        [Required(ErrorMessage = "Nutzername ist noetig")]
        [StringLength(100, ErrorMessage = "Nutzername maximal 100 Zeichen")]
        public string Nutzername { get; set; }

        [Required(ErrorMessage = "Passwort ist noetig")]
        [StringLength(64)]
        public string Passwort { get; set; }

        [Required(ErrorMessage = "Wiederholung ist noetig")]
        [StringLength(64)]
        [Compare("Passwort")]
        public string Wiederholung { get; set; }

        [Required(ErrorMessage = "Vorname bitte")]
        [StringLength(100)]
        public string Vorname { get; set; }

        [Required(ErrorMessage = "Nachname bitte")]
        [StringLength(50)]
        public string Nachname { get; set; }

        
        [Required(ErrorMessage = "Geben sie eine Email an oder...")]
        [StringLength(100)]
        public string Email { get; set; }

        public Registration()
        {
            Nutzername = null;
            Passwort = null;
            Wiederholung = null;
            Vorname = null;
            Nachname = null;
            Email = null;
        }

        public Registration(Registration r)
        {
            Nutzername = r.Nutzername;
            Passwort = r.Passwort;
            Wiederholung = r.Wiederholung;
            Vorname = r.Vorname;
            Nachname = r.Nachname;
            Email = r.Email;
        }
       
        public void TransactBenutzer(MySqlConnection c, MySqlTransaction tr)
        {
            MySqlCommand nutzerCmd = new MySqlCommand();
            nutzerCmd.Connection = c;
            nutzerCmd.Transaction = tr;

            var hashsalt = PasswordStorage.CreateHash(Passwort);

            var hashsalt1 = hashsalt.Substring(14);
            string[] hashsaltsplit = hashsalt1.Split(':');

            string salt = hashsaltsplit[0];
            string hash = hashsaltsplit[1];

            nutzerCmd.CommandText = "INSERT INTO benutzer (Vorname, Nachname, Email, Nutzername, Salt, Hashk) " +
                "VALUES (@vorname, @nachname, @email, @nutzer, @salt, @hash)";
            nutzerCmd.Parameters.AddWithValue("vorname", Vorname);
            nutzerCmd.Parameters.AddWithValue("nachname", Nachname);
            nutzerCmd.Parameters.AddWithValue("email", Email);
            nutzerCmd.Parameters.AddWithValue("nutzer", Nutzername);
            nutzerCmd.Parameters.AddWithValue("salt", salt);
            nutzerCmd.Parameters.AddWithValue("hash", hash);

            nutzerCmd.ExecuteNonQuery();
        }
    }

    public class GastRegistration : Registration
    {
        [Required(ErrorMessage = "Geben sie einen Grund an")]
        [StringLength(255)]
        public string Grund { get; set; }

        public GastRegistration()
            : base()
        {
            Grund = null;
        }

        public GastRegistration(Registration r)
            : base(r)
        {
            Grund = null;
        }

        public bool Register()
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                var tr = c.BeginTransaction();

                try {
                    TransactBenutzer(c, tr);

                    MySqlCommand gastCmd = new MySqlCommand();
                    gastCmd.Connection = c;
                    gastCmd.Transaction = tr;

                    gastCmd.CommandText = "INSERT INTO gaeste (Nummer, Grund) " +
                        "VALUES (LAST_INSERT_ID(), @grund)";
                    gastCmd.Parameters.AddWithValue("grund", Grund);

                    gastCmd.ExecuteNonQuery();
                    tr.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    tr.Rollback();
                    return false;
                }
            }
        }
    }

    public class StudentRegistration : Registration
    {
        [Required(ErrorMessage = "Geben sie ihren Studiengang an")]
        [StringLength(3)]
        public string Studiengang { get; set; }

        [Required(ErrorMessage = "Nummer bitte")]
        [Range(10000000, 999999999, ErrorMessage = "Eine Matrikelnummer hat 8-9 Stellen")]
        public int Matrikelnummer { get; set; }

        public StudentRegistration()
            : base()
        {
            Studiengang = null;
            Matrikelnummer = 0;
        }

        public StudentRegistration(Registration r)
            : base(r)
        {
            Studiengang = null;
            Matrikelnummer = 0;
        }

        public bool Register()
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                var tr = c.BeginTransaction();

                try
                {
                    TransactBenutzer(c, tr);

                    MySqlCommand angehoerigCmd = new MySqlCommand();
                    angehoerigCmd.Connection = c;
                    angehoerigCmd.Transaction = tr;

                    angehoerigCmd.CommandText = "INSERT INTO fhangehoerige (Nummer) VALUES (LAST_INSERT_ID())";
                    angehoerigCmd.ExecuteNonQuery();

                    MySqlCommand studentCmd = new MySqlCommand();
                    studentCmd.Connection = c;
                    studentCmd.Transaction = tr;

                    studentCmd.CommandText = "INSERT INTO studenten (Nummer, Studiengang, Matrikelnummer) " +
                        "VALUES (LAST_INSERT_ID(), @gang, @matrikel)";
                    studentCmd.Parameters.AddWithValue("gang", Studiengang);
                    studentCmd.Parameters.AddWithValue("matrikel", Matrikelnummer);

                    studentCmd.ExecuteNonQuery();
                    tr.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    tr.Rollback();
                    return false;
                }
            }
        }
    }

    public class MARegistration : Registration
    {
        [StringLength(25)]
        public string Buero { get; set; }
        [StringLength(25)]
        public string Telefon { get; set; }

        public MARegistration()
            : base()
        {
            Buero = null;
            Telefon = null;
        }

        public MARegistration(Registration r)
            : base(r)
        {
            Buero = null;
            Telefon = null;
        }

        public bool Register()
        {
            using (MySqlConnection c = new MySqlConnection(Database.ConnectionString()))
            {
                c.Open();
                var tr = c.BeginTransaction();

                try
                {
                    TransactBenutzer(c, tr);

                    MySqlCommand maCmd = new MySqlCommand();
                    maCmd.Connection = c;
                    maCmd.Transaction = tr;

                    MySqlCommand angehoerigCmd = new MySqlCommand();
                    angehoerigCmd.Connection = c;
                    angehoerigCmd.Transaction = tr;

                    angehoerigCmd.CommandText = "INSERT INTO fhangehoerige (Nummer) VALUES (LAST_INSERT_ID())";
                    angehoerigCmd.ExecuteNonQuery();

                    maCmd.CommandText = "INSERT INTO studenten (Nummer, Buero, Telefon) " +
                        "VALUES (LAST_INSERT_ID(), @buero, @tele)";
                    maCmd.Parameters.AddWithValue("buero", Buero);
                    maCmd.Parameters.AddWithValue("tele", Telefon);

                    maCmd.ExecuteNonQuery();
                    tr.Commit();
                    return true;
                }
                catch (Exception e)
                {
                    tr.Rollback();
                    return false;
                }
            }
        }
    }
}

