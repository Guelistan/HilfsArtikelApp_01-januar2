using MySql.Data.MySqlClient;
using System;
using System.Windows;
using System.Windows.Controls;

namespace HilfsArtikelApp_01
{
    public partial class MainWindow : Window
    {
        // Verbindungszeichenfolge zur MySQL-Datenbank
        private string verbindungszeichenfolge = "Server=localhost;Database=pflegeartikel_db;User ID=root;Password=dein_passwort;";
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
                using (MySqlConnection verbindung = new MySqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "SELECT id FROM kunden WHERE email = @Email AND passwort = @Passwort";
                    using (MySqlCommand befehl = new MySqlCommand(abfrage, verbindung))
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
            catch (MySqlException ex)
            {
                MessageBox.Show($"Fehler bei der Anmeldung: {ex.Message}");
            }
        }
    }
}
