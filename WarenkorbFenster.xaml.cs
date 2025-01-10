using MySql.Data.MySqlClient;
using System.Collections.ObjectModel;
using System.Windows;

namespace HilfsArtikelApp_01
{
    public partial class WarenkorbFenster : Window
    {
        public ObservableCollection<WarenkorbEintrag> WarenkorbInhalt { get; set; }
        private int kundenId;
        private string verbindungszeichenfolge = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\HilfsArtikelApp_01 januar2\HilfsArtikelApp_01\Data\hilfsartikel.mdf;Integrated Security=True";

        public WarenkorbFenster(int kundenId)
        {
            InitializeComponent();
            WarenkorbInhalt = new ObservableCollection<WarenkorbEintrag>();
            this.kundenId = kundenId;
            LadeWarenkorbDaten(kundenId);
            this.DataContext = this;
        }

        private void LadeWarenkorbDaten(int kundenId)
        {
            try
            {
                using (SqlConnection verbindung = new SqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "SELECT a.name, w.anzahl, w.preis FROM warenkorb w JOIN artikel a ON w.artikel_id = a.id WHERE w.kunden_id = @kunden_id";
                    using (SqlCommand befehl = new SqlCommand(abfrage, verbindung))
                    {
                        befehl.Parameters.AddWithValue("@kunden_id", kundenId);
                        using (SqlDataReader reader = befehl.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                WarenkorbInhalt.Add(new WarenkorbEintrag
                                {
                                    Name = reader.GetString(0),
                                    Anzahl = reader.GetInt32(1),
                                    Preis = reader.GetDecimal(2)
                                });
                            }
                        }
                    }
                    verbindung.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Fehler beim Laden des Warenkorbs: {ex.Message}");
            }
        }

        private void Bezahlen(int kundenId)
        {
            try
            {
                using (SqlConnection verbindung = new SqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "DELETE FROM warenkorb WHERE kunden_id = @kunden_id";
                    using (SqlCommand befehl = new SqlCommand(abfrage, verbindung))
                    {
                        befehl.Parameters.AddWithValue("@kunden_id", kundenId);
                        befehl.ExecuteNonQuery();
                    }
                    verbindung.Close();
                }
                WarenkorbInhalt.Clear();
                MessageBox.Show("Bezahlung erfolgreich. Der Warenkorb wurde geleert.");
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Fehler beim Bezahlen: {ex.Message}");
            }
        }

        private void ZurueckButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BezahlenButton_Click(object sender, RoutedEventArgs e)
        {
            Bezahlen(kundenId);
        }

        private void BereitsGekaufteArtikelButton_Click(object sender, RoutedEventArgs e)
        {
            GekaufteArtikelAnzeigen(kundenId);
        }

        private void GekaufteArtikelAnzeigen(int kundenId)
        {
            try
            {
                using (SqlConnection verbindung = new SqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "SELECT a.name, SUM(w.anzahl) as anzahl FROM warenkorb w JOIN artikel a ON w.artikel_id = a.id WHERE w.kunden_id = @kunden_id GROUP BY a.name";
                    using (SqlCommand befehl = new SqlCommand(abfrage, verbindung))
                    {
                        befehl.Parameters.AddWithValue("@kunden_id", kundenId);
                        using (SqlDataReader reader = befehl.ExecuteReader())
                        {
                            string gekaufteArtikel = "Gekaufte Artikel:\n";
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
                MessageBox.Show($"Fehler beim Anzeigen der gekauften Artikel: {ex.Message}");
            }
        }
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

    // Klasse zur Darstellung eines Warenkorbeintrags
    public class WarenkorbEintrag
    {
        public string Name { get; set; }
        public int Anzahl { get; set; }
        public decimal Preis { get; set; }
    }
}
