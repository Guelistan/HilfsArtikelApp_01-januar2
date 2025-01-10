using MySql.Data.MySqlClient;
using System;
using System.Net.Mail;
using System.Windows;
using System.Windows.Controls;

namespace HilfsArtikelApp_01
{
    public partial class Registrierung : Window
    {
        // Verbindungszeichenfolge zur MySQL-Datenbank
        private string verbindungszeichenfolge = "Server=localhost;Database=pflegeartikel_db;User ID=root;Password=dein_passwort;";


        public Registrierung()
        {
            InitializeComponent();
            // Ereignisse für Textänderungen hinzufügen
            NameTextBox.TextChanged += TextBox_TextChanged;
            VornameTextBox.TextChanged += TextBox_TextChanged;
            EmailTextBox.TextChanged += TextBox_TextChanged;
            PasswortTextBox.PasswordChanged += PasswortBox_PasswordChanged;
        }

        // Zeigt oder versteckt den Platzhaltertext basierend auf dem Inhalt der TextBox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NamePlaceholder.Visibility = string.IsNullOrEmpty(NameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            VornamePlaceholder.Visibility = string.IsNullOrEmpty(VornameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            EmailPlaceholder.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        // Zeigt oder versteckt den Platzhaltertext basierend auf dem Inhalt der PasswordBox
        private void PasswortBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswortPlaceholder.Visibility = string.IsNullOrEmpty(PasswortTextBox.Password) ? Visibility.Visible : Visibility.Visible;
        }

        // Registrierung und Anzeige des Bestätigungsbuttons
        private void RegistrierenButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string vorname = VornameTextBox.Text;
            string email = EmailTextBox.Text;
            string passwort = PasswortTextBox.Password;

            try
            {
                // Verbindung zur Datenbank herstellen und Daten speichern
                using (MySqlConnection verbindung = new MySqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "INSERT INTO kunden (name, vorname, email, passwort) VALUES (@name, @vorname, @email, @passwort)";
                    using (MySqlCommand befehl = new MySqlCommand(abfrage, verbindung))
                    {
                        befehl.Parameters.AddWithValue("@name", name);
                        befehl.Parameters.AddWithValue("@vorname", vorname);
                        befehl.Parameters.AddWithValue("@email", email);
                        befehl.Parameters.AddWithValue("@passwort", passwort);
                        befehl.ExecuteNonQuery();
                    }
                    verbindung.Close();
                }

                // Bestätigungs-E-Mail senden
                SendeBestaetigungsEmail(email, passwort);
                MessageBox.Show("Registrierung erfolgreich. Bitte überprüfen Sie Ihre E-Mails für die Bestätigung.");

                // Bestätigungsbutton anzeigen
                BestaetigenButton.Visibility = Visibility.Visible;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show($"Fehler bei der Registrierung: {ex.Message}");
            }
        }

        // Methode zum Senden der Bestätigungs-E-Mail
        private void SendeBestaetigungsEmail(string email, string passwort)
        {
            try
            {
                MailMessage nachricht = new MailMessage();
                nachricht.From = new MailAddress("deineemail@beispiel.com");
                nachricht.To.Add(email);
                nachricht.Subject = "Registrierungsbestätigung";
                nachricht.Body = $"Vielen Dank für Ihre Registrierung! Ihr Passwort lautet: {passwort}";

                SmtpClient smtp = new SmtpClient("smtp.beispiel.com");
                smtp.Port = 587;
                smtp.Credentials = new System.Net.NetworkCredential("deineemail@beispiel.com", "deinpasswort");
                smtp.EnableSsl = true;

                smtp.Send(nachricht);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Senden der Bestätigungs-E-Mail: {ex.Message}");
            }
        }

        // Wechsel zur Anmeldeseite
        private void BestaetigenButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow anmeldung = new MainWindow();
            anmeldung.Show();
            this.Close();
        }
    }
}
