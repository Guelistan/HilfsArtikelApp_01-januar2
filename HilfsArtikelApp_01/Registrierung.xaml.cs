using System.Data.SqlClient;
using System.Windows.Controls;
using System.Windows;
using System;
using System.Net.Mail;

namespace HilfsArtikelApp_01
{
    public partial class Registrierung : Window
    {
        // Verbindungszeichenfolge zur lokalen SQL Server-Datenbank
        private string verbindungszeichenfolge = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\HilfsArtikelApp_01 januar2\HilfsArtikelApp_01\Data\hilfsartikel.mdf;Integrated Security = True";

        public Registrierung()
        {
            InitializeComponent();
            // Ereignisse für Textänderungen hinzufügen
            NameTextBox.TextChanged += TextBox_TextChanged;
            VornameTextBox.TextChanged += TextBox_TextChanged;
            EmailTextBox.TextChanged += TextBox_TextChanged;
            PasswortTextBox.PasswordChanged += PasswortBox_PasswordChanged;
            AdresseTextBox.TextChanged += TextBox_TextChanged;
            StrasseTextBox.TextChanged += TextBox_TextChanged;
            PLZTextBox.TextChanged += TextBox_TextChanged;
            TelefonnummerTextBox.TextChanged += TextBox_TextChanged;
        }

        // Zeigt oder versteckt den Platzhaltertext basierend auf dem Inhalt der TextBox
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            NamePlaceholder.Visibility = string.IsNullOrEmpty(NameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            VornamePlaceholder.Visibility = string.IsNullOrEmpty(VornameTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            EmailPlaceholder.Visibility = string.IsNullOrEmpty(EmailTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            AdressePlaceholder.Visibility = string.IsNullOrEmpty(AdresseTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            StrassePlaceholder.Visibility = string.IsNullOrEmpty(StrasseTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            PLZPlaceholder.Visibility = string.IsNullOrEmpty(PLZTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
            TelefonnummerPlaceholder.Visibility = string.IsNullOrEmpty(TelefonnummerTextBox.Text) ? Visibility.Visible : Visibility.Hidden;
        }

        // Zeigt oder versteckt den Platzhaltertext basierend auf dem Inhalt der PasswordBox
        private void PasswortBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswortPlaceholder.Visibility = string.IsNullOrEmpty(PasswortTextBox.Password) ? Visibility.Visible : Visibility.Hidden;
        }

        // Registrierung und Anzeige des Bestätigungsbuttons
        private void RegistrierenButton_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            string vorname = VornameTextBox.Text;
            string email = EmailTextBox.Text;
            string passwort = PasswortTextBox.Password;
            string adresse = AdresseTextBox.Text;
            string strasse = StrasseTextBox.Text;
            string plz = PLZTextBox.Text;
            string telefonnummer = TelefonnummerTextBox.Text;
            string bezahloption = (BezahloptionComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();

            try
            {
                // Verbindung zur Datenbank herstellen und Daten speichern
                using (SqlConnection verbindung = new SqlConnection(verbindungszeichenfolge))
                {
                    verbindung.Open();
                    string abfrage = "INSERT INTO kunden (name, vorname, email, passwort, adresse, strasse, plz, telefonnummer, bezahloption) VALUES (@name, @vorname, @adresse, @strasse, @plz, @telefonnummer, @bezahloption)";
                    using (SqlCommand befehl = new SqlCommand(abfrage, verbindung))
                    {
                        befehl.Parameters.AddWithValue("@name", name);
                        befehl.Parameters.AddWithValue("@vorname", vorname);
                        befehl.Parameters.AddWithValue("@email", email);
                        befehl.Parameters.AddWithValue("@passwort", passwort);
                        befehl.Parameters.AddWithValue("@adresse", adresse);
                        befehl.Parameters.AddWithValue("@strasse", strasse);
                        befehl.Parameters.AddWithValue("@plz", plz);
                        befehl.Parameters.AddWithValue("@telefonnummer", telefonnummer);
                        befehl.Parameters.AddWithValue("@bezahloption", bezahloption);
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
            catch (SqlException ex)
            {
                MessageBox.Show($"Fehler bei der Registrierung: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ein unerwarteter Fehler ist aufgetreten: {ex.Message}");
            }
        }

        // Methode zum Senden der Bestätigungs-E-Mail
        private void SendeBestaetigungsEmail(string email, string passwort)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.your-isp.com");

                mail.From = new MailAddress("your_email@yourdomain.com");
                mail.To.Add(email);
                mail.Subject = "Registrierungsbestätigung";
                mail.Body = $"Vielen Dank für Ihre Registrierung. Ihr Passwort lautet: {passwort}";

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("username", "password");
                SmtpServer.EnableSsl = true;

                SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Senden der Bestätigungs-E-Mail: {ex.Message}");
            }
        }

        private void BestaetigenButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}