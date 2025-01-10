using System;

namespace HilfsArtikelApp_01.Data
{
    public class GekaufteArtikel
    {
        public int Id { get; set; }
        public int KundenId { get; set; }
        public int ArtikelId { get; set; }
        public int Anzahl { get; set; }
        public DateTime Kaufdatum { get; set; }
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
