using MySql.Data.MySqlClient;
using System;
using System.Windows;

namespace HilfsArtikelApp_01
{
    public partial class ArtikelFenster : Window
    {
        private string verbindungszeichenfolge = "Server=localhost;Database=pflegeartikel_db;User ID=root;Password=dein_passwort;";
        private int kundenId;

        public ArtikelFenster(int kundenId)
        {
            InitializeComponent();
            this.kundenId = kundenId;
        }

        private void AddHandschuhe_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Medizinische Handschuhe");
        }

        private void AddDesinfektionsmittel_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Desinfektionsmittel");
        }

        private void AddUnterlagen_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Unterlagen");
        }

        private void AddWindeln_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Windeln");
        }

        private void AddWundschutzcreme_Click(object sender, RoutedEventArgs e)
        {
            SpeichereArtikelInDatenbank("Wundschutzcreme");
        }

        // Speichert den ausgewählten Artikel in der Datenbank und berücksichtigt die Preisregelungen
        private void SpeichereArtikelInDatenbank(string artikelName)
        {
            try
            {
                using (MySqlConnection verbindung = new MySqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();

                    int artikelId = GetArtikelId(artikelName);
                    if (artikelId == -1)
                    {
                        MessageBox.Show($"Artikel {artikelName} nicht gefunden.");
                        return;
                    }

                    DateTime heute = DateTime.Now;
                    string monat = heute.ToString("yyyy-MM");

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
                            preis += 1.0M; // Preis um 1 Euro erhöhen
                        }

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
                    MessageBox.Show($"{artikelName} wurde dem Warenkorb hinzugefügt.");
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Fehler beim Speichern des Artikels: {ex.Message}");
            }
        }

        // Gibt die Artikel-ID für den angegebenen Artikel zurück
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

        // Öffnet das Warenkorb-Fenster
        private void ShowWarenkorb_Click(object sender, RoutedEventArgs e)
        {
            WarenkorbFenster warenkorbFenster = new WarenkorbFenster(kundenId);
            warenkorbFenster.Show();
        }
    }
}
