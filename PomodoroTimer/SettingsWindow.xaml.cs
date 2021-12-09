using System;
using System.Diagnostics;
using System.IO;
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
            if (File.Exists(MainWindow.SettingsFileName))
            {
                if (Int32.TryParse(tbxKisaMolaSuresi.Text, out int value2) && Int32.TryParse(tbxPomodoroSuresi.Text, out int value4) && Int32.TryParse(tbxUzunMolaSuresi.Text, out int value6))
                {
                    if (tbxKisaMolaSuresi.Text != string.Empty && tbxPomodoroSuresi.Text != string.Empty && tbxUzunMolaSuresi.Text != string.Empty)
                    {
                        MainWindow.BirKisaMolaZamani = Convert.ToInt32(tbxKisaMolaSuresi.Text);
                        MainWindow.BirPomodoroZamani = Convert.ToInt32(tbxPomodoroSuresi.Text);
                        MainWindow.BirUzunMolaZamani = Convert.ToInt32(tbxUzunMolaSuresi.Text);

                        System.IO.File.WriteAllText(MainWindow.SettingsFileName,
                MainWindow.BirPomodoroZamani.ToString() + "," +
                MainWindow.BirKisaMolaZamani.ToString() + "," +
                MainWindow.BirUzunMolaZamani.ToString() + "," +
                MainWindow.PTHeight.ToString() + "," +
                MainWindow.PTWidth.ToString());

                        MainWindow.ayarlarPenceresiAcikMi = false;

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
            else
            {
                MessageBox.Show("Ayar dosyası bulunamadı");
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
            tbxKisaMolaSuresi.Text = MainWindow.BirKisaMolaZamani.ToString();
            tbxPomodoroSuresi.Text = MainWindow.BirPomodoroZamani.ToString();
            tbxUzunMolaSuresi.Text = MainWindow.BirUzunMolaZamani.ToString();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void btnIptal_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.ayarlarPenceresiAcikMi = false;
            this.Close();
        }
    }
}