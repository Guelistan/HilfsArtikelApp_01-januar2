using MySql.Data.MySqlClient;
using System;
using System.Windows;
using MySqlConnector;
using MySqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using MySqlException = MySql.Data.MySqlClient.MySqlException;

namespace HilfsArtikelApp_01
{
    public partial class ArtikelFenster : Window
    {
        // Verbindungszeichenfolge zur MySQL-Datenbank
        string verbindungszeichenfolge = "Server=myServerAddress;Database=myDataBase;Uid=myUsername;Pwd=myPassword;";

        // Kunden-ID, die beim Erstellen des Fensters �bergeben wird
        private int kundenId;

        // Konstruktor, der die Kunden-ID entgegennimmt und das Fenster initialisiert
        public ArtikelFenster(int kundenId)
        {
            InitializeComponent();
            this.kundenId = kundenId;
        }

        // Event-Handler f�r den Klick auf die Schaltfl�che "Handschuhe hinzuf�gen"
        private void AddHandschuhe_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Medizinische Handschuhe");
        }

        // Event-Handler f�r den Klick auf die Schaltfl�che "Desinfektionsmittel hinzuf�gen"
        private void AddDesinfektionsmittel_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Desinfektionsmittel");
        }

        // Event-Handler f�r den Klick auf die Schaltfl�che "Unterlagen hinzuf�gen"
        private void AddUnterlagen_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Unterlagen");
        }

        // Event-Handler f�r den Klick auf die Schaltfl�che "Windeln hinzuf�gen"
        private void AddWindeln_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Windeln");
        }

        // Event-Handler f�r den Klick auf die Schaltfl�che "Wundschutzcreme hinzuf�gen"
        private void AddWundschutzcreme_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Wundschutzcreme");
        }

        // Speichert den ausgew�hlten Artikel in der Datenbank und ber�cksichtigt die Preisregelungen
        private void SpeichereArtikelInDatenbank(string artikelName)
        {
            try
            {
                using (MySqlConnection verbindung = new MySqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();

                    // Artikel-ID anhand des Artikelnamens abrufen
                    int artikelId = GetArtikelId(artikelName);
                    if (artikelId == -1)
                    {
                        MessageBox.Show($"Artikel {artikelName} nicht gefunden.");
                        return;
                    }

                    DateTime heute = DateTime.Now;
                    string monat = heute.ToString("yyyy-MM");

                    // �berpr�fen, wie oft der Artikel im aktuellen Monat bereits gekauft wurde
                    string countAbfrage = "SELECT COUNT(*) FROM warenkorb WHERE kunden_id = @kunden_id AND artikel_id = @artikel_id AND DATE_FORMAT(kaufdatum, '%Y-%m') = @monat";
                    using (MySqlCommand countBefehl = new MySqlCommand(countAbfrage, verbindung))
                    {
                        countBefehl.Parameters.AddWithValue("@kunden_id", kundenId);
                        countBefehl.Parameters.AddWithValue("@artikel_id", artikelId);
                        countBefehl.Parameters.AddWithValue("@monat", monat);

                        int artikelCount = Convert.ToInt32(countBefehl.ExecuteScalar());
                        decimal preis = 0.5M;
                        if (artikelCount >= 1)
                        {
                            preis += 1.0M; // Preis um 1 Euro erh�hen
                        }

                        // Artikel in den Warenkorb einf�gen
                        string insertAbfrage = "INSERT INTO warenkorb (kunden_id, artikel_id, kaufdatum, anzahl, preis) VALUES (@kunden_id, @artikel_id, @kaufdatum, @anzahl, @preis)";
                        using (MySqlCommand insertBefehl = new MySqlCommand(insertAbfrage, verbindung))
                        {
                            insertBefehl.Parameters.AddWithValue("@kunden_id", kundenId);
                            insertBefehl.Parameters.AddWithValue("@artikel_id", artikelId);
                            insertBefehl.Parameters.AddWithValue("@kaufdatum", heute);
                            insertBefehl.Parameters.AddWithValue("@anzahl", 1);
                            insertBefehl.Parameters.AddWithValue("@preis", preis);

                            insertBefehl.ExecuteNonQuery();
                        }
                    }

                    verbindung.Close();
                    MessageBox.Show($"{artikelName} wurde dem Warenkorb hinzugef�gt.");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Fehler beim Speichern des Artikels: {ex.Message}");
            }
        }

        // Gibt die Artikel-ID f�r den angegebenen Artikel zur�ck
        private int GetArtikelId(string artikelName)
        {
            try
            {
                using (MySqlConnection verbindung = new MySqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "SELECT id FROM artikel WHERE name = @name";
                    using (MySqlCommand befehl = new MySqlCommand(abfrage, verbindung))
                    {
                        befehl.Parameters.AddWithValue("@name", artikelName);
                        object result = befehl.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                    }
                    verbindung.Close();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Fehler beim Abrufen der Artikel-ID: {ex.Message}");
            }
            return -1;
        }

        // �ffnet das Warenkorb-Fenster
        private void ShowWarenkorb_Click(object sender, RoutedEventArgs e)
        {
            WarenkorbFenster warenkorbFenster = new WarenkorbFenster(kundenId);
            warenkorbFenster.Show();
        }
    }
}
