using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
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

        public class ColumnNameAttribute : System.Attribute
        {
            public ColumnNameAttribute(string Name)
            { this.Name = Name; }

            public string Name { get; set; }
        }

        private void dgPrimaryGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var desc = e.PropertyDescriptor as PropertyDescriptor;
            var att = desc.Attributes[typeof(ColumnNameAttribute)] as ColumnNameAttribute;
            if (att != null)
            {
                e.Column.Header = att.Name;
            }
        }

        private class Gecmis
        {
            [ColumnName("Sıra")]
            public string Id { get; set; }

            public string Tarih { get; set; }

            [ColumnName("Pomodoro Sayısı")]
            public string PomodoroSayisi { get; set; }

            [ColumnName("Kısa Mola Sayısı")]
            public string KisaMolaSayisi { get; set; }

            [ColumnName("Uzun Mola Sayısı")]
            public string UzunMolaSayisi { get; set; }

            [ColumnName("Toplam Pomodoro Dakikası")]
            public string ToplamPomodoroDakikasi { get; set; }
        }

        private List<Gecmis> gecmisList = new List<Gecmis>();

        private MainWindow mainWindow = new MainWindow();

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists(mainWindow.DbFileName))
            {
                dataGridTablo.AutoGeneratingColumn += dgPrimaryGrid_AutoGeneratingColumn;

                var tumSatirlar = File.ReadAllLines(mainWindow.DbFileName);
                List<string> satirlar = new List<string>(tumSatirlar);
                int i = 1;
                foreach (string satir in satirlar)
                {
                    mainWindow.stringDizi = satir.Split(',');

                    gecmisList.Add(new Gecmis()
                    {
                        Id = i.ToString(),
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
                if (!(string.IsNullOrEmpty(gecmis.Id.ToString())) &&
                    !(string.IsNullOrEmpty(gecmis.Tarih.ToString())) &&
                    !(string.IsNullOrEmpty(gecmis.PomodoroSayisi.ToString())) &&
                    !(string.IsNullOrEmpty(gecmis.KisaMolaSayisi.ToString())) &&
                    !(string.IsNullOrEmpty(gecmis.UzunMolaSayisi.ToString())) &&
                    !(string.IsNullOrEmpty(gecmis.ToplamPomodoroDakikasi.ToString())))
                {
                    if (regex.IsMatch(gecmis.Tarih) &&
                        Int32.TryParse(gecmis.Id, out int value1) &&
                        Int32.TryParse(gecmis.PomodoroSayisi, out int value2) &&
                        Int32.TryParse(gecmis.KisaMolaSayisi, out int value3) &&
                        Int32.TryParse(gecmis.UzunMolaSayisi, out int value4))
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
                            ", Toplam Pomodoro Dakikası: " + gecmis.ToplamPomodoroDakikasi.ToString() + " satırı hatalı";

                        MessageBox.Show(hataliSatir);
                        uygunluk = false;
                        break;
                    }
                }
                else
                {
                    hataliSatir = "Satırlar boş olamaz";

                    MessageBox.Show(hataliSatir);
                    uygunluk = false;
                    break;
                }
            }

            if (uygunluk)
            {
                System.IO.File.WriteAllLines(mainWindow.DbFileName, tumListe);
                this.Close();
                YenidenBaslat();
            }
        }
    }
}