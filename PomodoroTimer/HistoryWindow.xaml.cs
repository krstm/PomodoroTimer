using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
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
            public string ToplamPomodoroDakikasi { get; set; }
        }

        private List<Gecmis> gecmisList = new List<Gecmis>();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();

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
                        UzunMolaSayisi = mainWindow.stringDizi[3],
                        ToplamPomodoroDakikasi = mainWindow.stringDizi[4]
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

        private List<string> tumListe = new List<string>();

        private static string pattern = "^(?:[012]?[0-9]|3[01])[.](?:0?[1-9]|1[0-2])[.](?:[0-9]{2}){1,2}$";
        private Regex regex = new Regex(pattern);
        private bool uygunluk = true;
        private string hataliSatir = "";

        private void YenidenBaslat()
        {
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }

        private void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            this.dataGridTablo.ItemsSource = gecmisList;
            uygunluk = true;

            foreach (Gecmis gecmis in gecmisList)
            {
                if (regex.IsMatch(gecmis.Tarih) && Int32.TryParse(gecmis.PomodoroSayisi, out int value2) && Int32.TryParse(gecmis.KisaMolaSayisi, out int value3) && Int32.TryParse(gecmis.UzunMolaSayisi, out int value4))
                {
                    if (gecmis.Tarih != string.Empty && gecmis.PomodoroSayisi != string.Empty && gecmis.KisaMolaSayisi != string.Empty && gecmis.UzunMolaSayisi != string.Empty && gecmis.ToplamPomodoroDakikasi != string.Empty)
                    {
                        tumListe.Add(gecmis.Tarih.ToString() + "," + gecmis.PomodoroSayisi.ToString() + "," + gecmis.KisaMolaSayisi.ToString() + "," + gecmis.UzunMolaSayisi.ToString() + "," + gecmis.ToplamPomodoroDakikasi.ToString());
                    }
                    else
                    {
                        hataliSatir = "Id: " + gecmis.Id.ToString() +
                            ", Tarih: " + gecmis.Tarih.ToString() +
                            ", Pomodoro Sayısı: " + gecmis.PomodoroSayisi.ToString() +
                            ", Kısa Mola Sayısı: " + gecmis.KisaMolaSayisi.ToString() +
                            ", Uzun Mola Sayısı: " + gecmis.UzunMolaSayisi.ToString() +
                            ", Toplam Pomodoro Dakikası: " + gecmis.ToplamPomodoroDakikasi.ToString() + " satırı hatalı.";

                        MessageBox.Show(hataliSatir);
                        uygunluk = false;
                        break;
                    }
                }
                else
                {
                    hataliSatir = "Id: " + gecmis.Id.ToString() +
                            ", Tarih: " + gecmis.Tarih.ToString() +
                            ", Pomodoro Sayısı: " + gecmis.PomodoroSayisi.ToString() +
                            ", Kısa Mola Sayısı: " + gecmis.KisaMolaSayisi.ToString() +
                            ", Uzun Mola Sayısı: " + gecmis.UzunMolaSayisi.ToString() +
                            ", Toplam Pomodoro Dakikası: " + gecmis.ToplamPomodoroDakikasi.ToString() + " satırı hatalı.";

                    MessageBox.Show(hataliSatir);
                    uygunluk = false;
                    break;
                }
            }

            if (uygunluk)
            {
                System.IO.File.WriteAllLines("PomodoroTimerDb.txt", tumListe);
                this.Close();
                YenidenBaslat();
            }
        }
    }
}