using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private class Gecmis
        {
            public int Id { get; set; }
            public string Tarih { get; set; }
            public string PomodoroSayisi { get; set; }
            public string KisaMolaSayisi { get; set; }
            public string UzunMolaSayisi { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            List<Gecmis> gecmisList = new List<Gecmis>();

            if (File.Exists(mainWindow.DbFileName))
            {
                var tumSatirlar = File.ReadAllLines(mainWindow.DbFileName);
                List<string> satirlar = new List<string>(tumSatirlar);
                int i = 1;
                foreach (string satir in satirlar)
                {
                    mainWindow.stringDizi = satir.Split(',');

                    gecmisList.Add(new Gecmis()
                    {
                        Id = i,
                        Tarih = mainWindow.stringDizi[0],
                        PomodoroSayisi = mainWindow.stringDizi[1],
                        KisaMolaSayisi = mainWindow.stringDizi[2],
                        UzunMolaSayisi = mainWindow.stringDizi[3]
                    });

                    i++;
                }
                dataGridTablo.ItemsSource = gecmisList;
            }
            else
            {
                MessageBox.Show("Veritabanı bulunamadı.");
                this.Close();
            }
        }

        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}