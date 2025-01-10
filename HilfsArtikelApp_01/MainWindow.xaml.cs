using System;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace HilfsArtikelApp_01
{
    public partial class MainWindow : Window
    {
        // Verbindungszeichenfolge zur lokalen SQL Server-Datenbank
        private string verbindungszeichenfolge = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\HilfsArtikelApp_01 januar2\HilfsArtikelApp_01\Data\hilfsartikel.mdf;Integrated Security = True";

        // Kunden-ID
        private int kundenId = -1; // Initialwert für die Kunden-ID

        public MainWindow()
        {
            InitializeComponent();
            // Ereignisse für Platzhalter-Text hinzufügen
            EmailTextBox.TextChanged += TextBox_TextChanged;
            PasswortBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        // Zeigt oder versteckt den Platzhalter-Text für die E-Mail-TextBox basierend auf dem Inhalt
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            EmailPlaceholder.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        private void GastzugangButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Gastzugang aktiviert");
            ArtikelFenster artikelFenster = new ArtikelFenster(kundenId);
            artikelFenster.Show();
            this.Close();
        }

        // Zeigt oder versteckt den Platzhalter-Text für die PasswortBox basierend auf dem Inhalt
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswortBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        // Öffnet das Registrierungsfenster
        private void RegistrierenButton_Click(object sender, RoutedEventArgs e)
        {
            Registrierung registrierung = new Registrierung();
            registrierung.ShowDialog();
        }

        // Überprüft die Anmeldeinformationen und öffnet das ArtikelFenster bei Erfolg
        private void AnmeldenButton_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text;
            string passwort = PasswortBox.Password;

            try
            {
                using (SqlConnection verbindung = new SqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "SELECT id FROM kunden WHERE email = @Email AND passwort = @Passwort";
                    using (SqlCommand befehl = new SqlCommand(abfrage, verbindung))
                    {
                        befehl.Parameters.AddWithValue("@Email", email);
                        befehl.Parameters.AddWithValue("@Passwort", passwort);
                        object result = befehl.ExecuteScalar();
                        if (result != null)
                        {
                            kundenId = Convert.ToInt32(result);
                            MessageBox.Show("Anmeldung erfolgreich.");
                            ArtikelFenster artikelFenster = new ArtikelFenster(kundenId);
                            artikelFenster.Show();
                            this.Close();
                        }
                        else
                        {
                            FehlermeldungTextBlock.Text = "Ungültige Anmeldeinformationen.";
                        }
                    }
                    verbindung.Close();
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show($"Fehler bei der Anmeldung: {ex.Message}");
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
}

