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

        private MainWindow mainWindow = new MainWindow();

        private void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists(mainWindow.SettingsFileName))
            {
                if (Int32.TryParse(tbxKisaMolaSuresi.Text, out int value2) && Int32.TryParse(tbxPomodoroSuresi.Text, out int value4) && Int32.TryParse(tbxUzunMolaSuresi.Text, out int value6))
                {
                    if (tbxKisaMolaSuresi.Text != string.Empty && tbxPomodoroSuresi.Text != string.Empty && tbxUzunMolaSuresi.Text != string.Empty)
                    {
                        mainWindow.BirKisaMolaZamani = Convert.ToInt32(tbxKisaMolaSuresi.Text);
                        mainWindow.BirPomodoroZamani = Convert.ToInt32(tbxPomodoroSuresi.Text);
                        mainWindow.BirUzunMolaZamani = Convert.ToInt32(tbxUzunMolaSuresi.Text);

                        System.IO.File.WriteAllText(mainWindow.SettingsFileName,
                mainWindow.BirPomodoroZamani.ToString() + "," +
                mainWindow.BirKisaMolaZamani.ToString() + "," +
                mainWindow.BirUzunMolaZamani.ToString() + "," +
                mainWindow.PTHeight.ToString() + "," +
                mainWindow.PTWidth.ToString());

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
            MainWindow.ayarlarPenceresiAcikMi = false;
            this.Close();
        }
    }
}