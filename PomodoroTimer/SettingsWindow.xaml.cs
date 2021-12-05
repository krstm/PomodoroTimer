using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace PomodoroTimer
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (Int32.TryParse(tbxKisaMolaSuresi.Text, out int value2) && Int32.TryParse(tbxPomodoroSuresi.Text, out int value4) && Int32.TryParse(tbxUzunMolaSuresi.Text, out int value6))
            {
                if (tbxKisaMolaSuresi.Text != string.Empty && tbxPomodoroSuresi.Text != string.Empty && tbxUzunMolaSuresi.Text != string.Empty)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.BirKisaMolaZamani = Convert.ToInt32(tbxKisaMolaSuresi.Text);
                    mainWindow.BirPomodoroZamani = Convert.ToInt32(tbxPomodoroSuresi.Text);
                    mainWindow.BirUzunMolaZamani = Convert.ToInt32(tbxUzunMolaSuresi.Text);

                    System.IO.File.WriteAllText("PomodoroTimerSettings.txt", mainWindow.BirPomodoroZamani.ToString() + "," +
                       mainWindow.BirKisaMolaZamani.ToString() + "," +
                       mainWindow.BirUzunMolaZamani.ToString());

                    if (mainWindow.pomodoroSayisiListesi.Count > 0)
                    {
                        mainWindow.pomodoroSayisiListesi.RemoveAt(mainWindow.pomodoroSayisiListesi.Count - 1);
                        mainWindow.pomodoroSayisiListesi.Add(mainWindow.PomodoroSayisi.ToString());
                    }
                    if (mainWindow.kisaMolaSayisiListesi.Count > 0)
                    {
                        mainWindow.kisaMolaSayisiListesi.RemoveAt(mainWindow.kisaMolaSayisiListesi.Count - 1);
                        mainWindow.kisaMolaSayisiListesi.Add(mainWindow.KisaMolaSayisi.ToString());
                    }
                    if (mainWindow.uzunMolaSayisiListesi.Count > 0)
                    {
                        mainWindow.pomodoroSayisiListesi.RemoveAt(mainWindow.pomodoroSayisiListesi.Count - 1);
                        mainWindow.pomodoroSayisiListesi.Add(mainWindow.PomodoroSayisi.ToString());
                    }
                    if (mainWindow.GunlukToplamPomodoroDakikaListesi.Count > 0)
                    {
                        mainWindow.GunlukToplamPomodoroDakikaListesi.RemoveAt(mainWindow.GunlukToplamPomodoroDakikaListesi.Count - 1);
                        mainWindow.GunlukToplamPomodoroDakikaListesi.Add(mainWindow.GunlukToplamPomodoroDakikasi.ToString());
                    }
                    if (mainWindow.tumListe.Count > 0)
                    {
                        mainWindow.tumListe.RemoveAt(mainWindow.tumListe.Count - 1);
                        mainWindow.tumListe.Add(mainWindow.tarihListesi[mainWindow.tarihListesi.Count - 1] + "," + mainWindow.PomodoroSayisi.ToString() + "," + mainWindow.KisaMolaSayisi.ToString() + "," + mainWindow.UzunMolaSayisi.ToString() + "," + mainWindow.GunlukToplamPomodoroDakikasi.ToString());
                    }

                    if (mainWindow.tarihListesi.Count > 0 && mainWindow.pomodoroSayisiListesi.Count > 0)
                    {
                        System.IO.File.WriteAllLines("PomodoroTimerDb.txt", mainWindow.tumListe);
                    }

                    this.Close();
                    YenidenBaslat();
                }
                else
                {
                    MessageBox.Show("Alanlar boş olamaz");
                }
            }
            else
            {
                MessageBox.Show("Alanlara sayı girmelisiniz");
            }
        }

        private void YenidenBaslat()
        {
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            tbxKisaMolaSuresi.Text = mainWindow.BirKisaMolaZamani.ToString();
            tbxPomodoroSuresi.Text = mainWindow.BirPomodoroZamani.ToString();
            tbxUzunMolaSuresi.Text = mainWindow.BirUzunMolaZamani.ToString();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnIptal_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}