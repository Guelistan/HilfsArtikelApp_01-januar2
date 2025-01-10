using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System;
 
 
namespace HilfsArtikelApp_01
{
    public partial class ArtikelFenster : Window
    {
        // Verbindungszeichenfolge zur lokalen SQL Server-Datenbank
        private string verbindungszeichenfolge = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\HilfsArtikelApp_01 januar2\HilfsArtikelApp_01\Data\hilfsartikel.mdf;Integrated Security = True";

        // Kunden-ID, die beim Erstellen des Fensters übergeben wird
        private int kundenId;

        // Konstruktor, der die Kunden-ID entgegennimmt und das Fenster initialisiert
        public ArtikelFenster(int kundenId)
        {
            InitializeComponent();
            this.kundenId = kundenId;
        }

        // Event-Handler für den Klick auf die Schaltfläche "Handschuhe hinzufügen"
        private void AddHandschuhe_Click(object sender, RoutedEventArgs e)
        {
            // Speichert den Artikel "Medizinische Handschuhe" in der Datenbank
            SpeichereArtikelInDatenbank("Medizinische Handschuhe", sender as Button);
        }

        // Event-Handler für den Klick auf die Schaltfläche "Desinfektionsmittel hinzufügen"
        private void AddDesinfektionsmittel_Click(object sender, RoutedEventArgs e)
        {
            // Speichert den Artikel "Desinfektionsmittel" in der Datenbank
            SpeichereArtikelInDatenbank("Desinfektionsmittel", sender as Button);
        }

        // Event-Handler für den Klick auf die Schaltfläche "Unterlagen hinzufügen"
        private void AddUnterlagen_Click(object sender, RoutedEventArgs e)
        {
            // Speichert den Artikel "Unterlagen" in der Datenbank
            SpeichereArtikelInDatenbank("Unterlagen", sender as Button);
        }

        // Event-Handler für den Klick auf die Schaltfläche "Windeln hinzufügen"
        private void AddWindeln_Click(object sender, RoutedEventArgs e)
        {
            // Speichert den Artikel "Windeln" in der Datenbank
            SpeichereArtikelInDatenbank("Windeln", sender as Button);
        }

        // Event-Handler für den Klick auf die Schaltfläche "Wundschutzcreme hinzufügen"
        private void AddWundschutzcreme_Click(object sender, RoutedEventArgs e)
        {
            // Speichert den Artikel "Wundschutzcreme" in der Datenbank
            SpeichereArtikelInDatenbank("Wundschutzcreme", sender as Button);
        }

        // Speichert den ausgewählten Artikel in der Datenbank und berücksichtigt die Preisregelungen
        private void SpeichereArtikelInDatenbank(string artikelName, Button button)
        {
            try
            {
                using (SqlConnection verbindung = new SqlConnection(verbindungszeichenfolge))
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

                    // Überprüfen, wie oft der Artikel im aktuellen Monat bereits gekauft wurde
                    string countAbfrage = "SELECT COUNT(*) FROM warenkorb WHERE kunden_id = @kunden_id AND artikel_id = @artikel_id AND FORMAT(kaufdatum, 'yyyy-MM') = @monat";
                    using (SqlCommand countBefehl = new SqlCommand(countAbfrage, verbindung))
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

                        // Artikel in den Warenkorb einfügen
                        string insertAbfrage = "INSERT INTO warenkorb (kunden_id, artikel_id, kaufdatum, anzahl, preis) VALUES (@kunden_id, @artikel_id, @kaufdatum, @anzahl, @preis)";
                        using (SqlCommand insertBefehl = new SqlCommand(insertAbfrage, verbindung))
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

                    // Button-Farbe ändern
                    button.Background = new SolidColorBrush(Colors.Gray);

                    // Ausgewählten Artikel anzeigen
                    AusgewählteArtikelTextBlock.Text += $"\n{artikelName}";
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Fehler beim Speichern des Artikels: {ex.Message}");
            }
        }

        // Gibt die Artikel-ID für den angegebenen Artikel zurück
        private int GetArtikelId(string artikelName)
        {
            try
            {
                using (SqlConnection verbindung = new SqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "SELECT id FROM artikel WHERE name = @name";
                    using (SqlCommand befehl = new SqlCommand(abfrage, verbindung))
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
            catch (SqlException ex)
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

        // Zeigt bereits gekaufte Artikel an
        private void BereitsGekaufteArtikelAnzeigen(int kundenId)
        {
            try
            {
                using (SqlConnection verbindung = new SqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "SELECT a.name, SUM(w.anzahl) as anzahl FROM gekaufte_artikel w JOIN artikel a ON w.artikel_id = a.id WHERE w.kunden_id = @kunden_id GROUP BY a.name";
                    using (SqlCommand befehl = new SqlCommand(abfrage, verbindung))
                    {
                        befehl.Parameters.AddWithValue("@kunden_id", kundenId);
                        using (SqlDataReader reader = befehl.ExecuteReader())
                        {
                            string gekaufteArtikel = "Bereits gekaufte Artikel:\n";
                            while (reader.Read())
                            {
                                gekaufteArtikel += $"{reader.GetString(0)} - {reader.GetInt32(1)}\n";
                            }
                            MessageBox.Show(gekaufteArtikel);
                        }
                    }
                    verbindung.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Fehler beim Anzeigen der bereits gekauften Artikel: {ex.Message}");
            }
        }
    }
}
 

//### Erläuterung der Änderungen und Kommentare:
//1. * *Verbindungszeichenfolge * *: Die Verbindungszeichenfolge zur SQL Server-Datenbank wird definiert.
//2. **Konstruktor**: Der Konstruktor nimmt die Kunden-ID entgegen und initialisiert das Fenster.
//3. **Event-Handler**: Für jeden Artikel gibt es einen Event-Handler, der den Artikel in der Datenbank speichert.
//4. **SpeichereArtikelInDatenbank**: Diese Methode speichert den ausgewählten Artikel in der Datenbank und berücksichtigt die Preisregelungen. Sie ändert auch die Farbe des Buttons und zeigt den ausgewählten Artikel an.
//5. **GetArtikelId**: Diese Methode gibt die Artikel-ID für den angegebenen Artikel zurück.
//6. **ShowWarenkorb_Click**: Diese Methode öffnet das Warenkorb-Fenster.
//7. **BereitsGekaufteArtikelAnzeigen**: Diese Methode zeigt die bereits gekauften Artikel an.

 