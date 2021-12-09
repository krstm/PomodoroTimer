using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Windows;
using System.Windows.Input;

namespace PomodoroTimer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            IlkAyarlar();
        }

        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private int Saniye { get; set; }
        public int PomodoroSayisi { get; set; }
        public int KisaMolaSayisi { get; set; }
        public int UzunMolaSayisi { get; set; }
        private int BirDakika { get; } = 60;
        public int BirPomodoroZamani { get; set; } = 25;
        public int BirKisaMolaZamani { get; set; } = 5;
        public int BirUzunMolaZamani { get; set; } = 15;
        public int PTHeight { get; set; } = 500;
        public int PTWidth { get; set; } = 350;
        private string Durum { get; set; }
        public string DbFileName { get; } = "PomodoroTimerDb.txt";
        public string SettingsFileName { get; } = "PomodoroTimerSettings.txt";

        public static bool ayarlarPenceresiAcikMi = false;
        public static bool gecmisPenceresiAcikMi = false;

        public List<string> tarihListesi = new List<string>();
        public List<string> pomodoroSayisiListesi = new List<string>();
        public List<string> kisaMolaSayisiListesi = new List<string>();
        public List<string> uzunMolaSayisiListesi = new List<string>();
        public List<string> tumListe = new List<string>();
        public string[] stringDizi;

        public int GunlukToplamPomodoroDakikasi { get; set; } = 0;
        public List<string> GunlukToplamPomodoroDakikaListesi = new List<string>();

        private void IlkAyarlar()
        {
            dispatcherTimer.Interval = TimeSpan.FromSeconds(1);
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            DosyaIslemleri();
            AyarlariOku();
        }

        private void DosyaIslemleri()
        {
            if (File.Exists(DbFileName))
            {
                var tumSatirlar = File.ReadAllLines(DbFileName);
                List<string> satirlar = new List<string>(tumSatirlar);
                foreach (string satir in satirlar)
                {
                    tumListe.Add(satir);
                    stringDizi = satir.Split(',');
                    tarihListesi.Add(stringDizi[0]);
                    pomodoroSayisiListesi.Add(stringDizi[1]);
                    kisaMolaSayisiListesi.Add(stringDizi[2]);
                    uzunMolaSayisiListesi.Add(stringDizi[3]);
                    GunlukToplamPomodoroDakikaListesi.Add(stringDizi[4]);
                }
                if (tarihListesi[tarihListesi.Count - 1] != DateTime.Now.ToString("dd/MM/yyyy"))
                {
                    tarihListesi.Add(DateTime.Now.ToString("dd/MM/yyyy"));
                    pomodoroSayisiListesi.Add(PomodoroSayisi.ToString());
                    kisaMolaSayisiListesi.Add(KisaMolaSayisi.ToString());
                    uzunMolaSayisiListesi.Add(UzunMolaSayisi.ToString());
                    GunlukToplamPomodoroDakikaListesi.Add(GunlukToplamPomodoroDakikasi.ToString());

                    tumListe.Add(DateTime.Now.ToString("dd/MM/yyyy") + "," + PomodoroSayisi.ToString() + "," + KisaMolaSayisi.ToString() + "," + UzunMolaSayisi.ToString() + "," + GunlukToplamPomodoroDakikasi.ToString());

                    System.IO.File.WriteAllLines(DbFileName, tumListe);
                    YenidenBaslat();
                }
                else
                {
                    PomodoroSayisi = Convert.ToInt32(pomodoroSayisiListesi[pomodoroSayisiListesi.Count - 1]);
                    btnPomodoro.Content = "Pomodoro (" + PomodoroSayisi + ")";

                    KisaMolaSayisi = Convert.ToInt32(kisaMolaSayisiListesi[kisaMolaSayisiListesi.Count - 1]);
                    btnShortBreak.Content = "Kısa Mola (" + KisaMolaSayisi + ")";

                    UzunMolaSayisi = Convert.ToInt32(uzunMolaSayisiListesi[uzunMolaSayisiListesi.Count - 1]);
                    btnLongBreak.Content = "Uzun Mola (" + UzunMolaSayisi + ")";

                    GunlukToplamPomodoroDakikasi = Convert.ToInt32(GunlukToplamPomodoroDakikaListesi[GunlukToplamPomodoroDakikaListesi.Count - 1]);
                    lblToplamPomodoro.Content = "Toplam Pomodoro: " + GunlukToplamPomodoroDakikasi;
                }
            }
            else
            {
                tarihListesi.Add(DateTime.Now.ToString("dd/MM/yyyy"));
                pomodoroSayisiListesi.Add(PomodoroSayisi.ToString());
                kisaMolaSayisiListesi.Add(KisaMolaSayisi.ToString());
                uzunMolaSayisiListesi.Add(UzunMolaSayisi.ToString());
                GunlukToplamPomodoroDakikaListesi.Add(GunlukToplamPomodoroDakikasi.ToString());

                tumListe.Add(DateTime.Now.ToString("dd/MM/yyyy") + "," + PomodoroSayisi.ToString() + "," + KisaMolaSayisi.ToString() + "," + UzunMolaSayisi.ToString() + "," + GunlukToplamPomodoroDakikasi.ToString());

                System.IO.File.WriteAllLines(DbFileName, tumListe);
                YenidenBaslat();
            }
        }

        private void AyarlariOku()
        {
            if (File.Exists(SettingsFileName))
            {
                var tumSatirlar = File.ReadAllLines(SettingsFileName);
                List<string> satirlar = new List<string>(tumSatirlar);
                foreach (string satir in satirlar)
                {
                    stringDizi = satir.Split(',');

                    BirPomodoroZamani = Convert.ToInt32(stringDizi[0]);
                    BirKisaMolaZamani = Convert.ToInt32(stringDizi[1]);
                    BirUzunMolaZamani = Convert.ToInt32(stringDizi[2]);
                    PTHeight = Convert.ToInt32(stringDizi[3]);
                    PTWidth = Convert.ToInt32(stringDizi[4]);

                    Application.Current.MainWindow.Height = PTHeight;
                    Application.Current.MainWindow.Width = PTWidth;
                }
            }
            else
            {
                System.IO.File.WriteAllText(SettingsFileName,
                    BirPomodoroZamani.ToString() + "," +
                    BirKisaMolaZamani.ToString() + "," +
                    BirUzunMolaZamani.ToString() + "," +
                    PTHeight.ToString() + "," +
                    PTWidth.ToString());

                YenidenBaslat();
            }
        }

        private void Kaydet()
        {
            if (pomodoroSayisiListesi.Count > 0)
            {
                pomodoroSayisiListesi.RemoveAt(pomodoroSayisiListesi.Count - 1);
                pomodoroSayisiListesi.Add(PomodoroSayisi.ToString());
            }
            if (kisaMolaSayisiListesi.Count > 0)
            {
                kisaMolaSayisiListesi.RemoveAt(kisaMolaSayisiListesi.Count - 1);
                kisaMolaSayisiListesi.Add(KisaMolaSayisi.ToString());
            }
            if (uzunMolaSayisiListesi.Count > 0)
            {
                pomodoroSayisiListesi.RemoveAt(pomodoroSayisiListesi.Count - 1);
                pomodoroSayisiListesi.Add(PomodoroSayisi.ToString());
            }
            if (GunlukToplamPomodoroDakikaListesi.Count > 0)
            {
                GunlukToplamPomodoroDakikaListesi.RemoveAt(GunlukToplamPomodoroDakikaListesi.Count - 1);
                GunlukToplamPomodoroDakikaListesi.Add(GunlukToplamPomodoroDakikasi.ToString());
            }

            switch (Durum)
            {
                case "Pomodoro":
                    GunlukToplamPomodoroDakikasi += BirPomodoroZamani;
                    lblToplamPomodoro.Content = "Toplam Pomodoro: " + GunlukToplamPomodoroDakikasi;
                    break;
            }

            if (tumListe.Count > 0)
            {
                tumListe.RemoveAt(tumListe.Count - 1);
                tumListe.Add(tarihListesi[tarihListesi.Count - 1] + "," + PomodoroSayisi.ToString() + "," + KisaMolaSayisi.ToString() + "," + UzunMolaSayisi.ToString() + "," + GunlukToplamPomodoroDakikasi.ToString());
            }

            if (tarihListesi.Count > 0 && pomodoroSayisiListesi.Count > 0)
            {
                System.IO.File.WriteAllLines(DbFileName, tumListe);
            }
        }

        private void YenidenBaslat()
        {
            var currentExecutablePath = Process.GetCurrentProcess().MainModule.FileName;
            Process.Start(currentExecutablePath);
            Application.Current.Shutdown();
        }

        private string Dakikalar()
        {
            if ((Saniye / 60) > 9)
            {
                return (Saniye / 60).ToString();
            }
            else
            {
                return "0" + (Saniye / 60).ToString();
            }
        }

        private string Saniyeler()
        {
            if ((Saniye % 60) > 9)
            {
                return (Saniye % 60).ToString();
            }
            else
            {
                return "0" + (Saniye % 60).ToString();
            }
        }

        private void ButonlariKilitle(bool durum)
        {
            btnShortBreak.IsEnabled = durum;
            btnPomodoro.IsEnabled = durum;
            btnLongBreak.IsEnabled = durum;
        }

        private void ButonlarinGorunurlugu(bool gorunurluk)
        {
            if (gorunurluk)
            {
                btnCancel.Visibility = Visibility.Visible;
            }
            else
            {
                btnCancel.Visibility = Visibility.Hidden;
            }
        }

        private void BildirimCal()
        {
            bool found = false;
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes\Apps\.Default\Notification.Default\.Current"))
                {
                    if (key != null)
                    {
                        Object o = key.GetValue(null);
                        if (o != null)
                        {
                            SoundPlayer theSound = new SoundPlayer((String)o);
                            theSound.Play();
                            found = true;
                        }
                    }
                }
            }
            catch
            { }
            if (!found)
                SystemSounds.Beep.Play();
        }

        private void GeriSayim(int saniye, string durum)
        {
            ButonlariKilitle(false);
            ButonlarinGorunurlugu(true);
            Durum = durum;
            BasliklariKalinlastir(Durum);
            Saniye = saniye;

            if (Durum == "Pomodoro")
            {
                PomodoroSayisi++;
                btnPomodoro.Content = "Pomodoro (" + PomodoroSayisi + ")";
            }
            else if (Durum == "Kısa Mola")
            {
                KisaMolaSayisi++;
                btnShortBreak.Content = "Kısa Mola (" + KisaMolaSayisi + ")";
            }
            else if (Durum == "Uzun Mola")
            {
                UzunMolaSayisi++;
                btnLongBreak.Content = "Uzun Mola (" + UzunMolaSayisi + ")";
            }

            dispatcherTimer.Start();
        }

        private void BasliklariKalinlastir(string baslik)
        {
            if (baslik == "Pomodoro")
            {
                btnPomodoro.FontWeight = FontWeights.Bold;
            }
            else if (baslik == "Kısa Mola")
            {
                btnShortBreak.FontWeight = FontWeights.Bold;
            }
            else if (baslik == "Uzun Mola")
            {
                btnLongBreak.FontWeight = FontWeights.Bold;
            }
            else
            {
                btnPomodoro.FontWeight = FontWeights.Normal;
                btnLongBreak.FontWeight = FontWeights.Normal;
                btnShortBreak.FontWeight = FontWeights.Normal;
            }
        }

        private void btnPomodoro_Click(object sender, RoutedEventArgs e)
        {
            GeriSayim(BirPomodoroZamani * BirDakika, "Pomodoro");
        }

        private void btnShortBreak_Click(object sender, RoutedEventArgs e)
        {
            GeriSayim(BirKisaMolaZamani * BirDakika, "Kısa Mola");
        }

        private void btnLongBreak_Click(object sender, RoutedEventArgs e)
        {
            GeriSayim(BirUzunMolaZamani * BirDakika, "Uzun Mola");
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (Saniye <= 0)
            {
                dispatcherTimer.Stop();
                ButonlariKilitle(true);
                ButonlarinGorunurlugu(false);
                BasliklariKalinlastir("");
                BildirimCal();
                Kaydet();
            }
            else
            {
                Saniye--;
                lblSure.Content = Dakikalar() + ":" + Saniyeler();
            }
        }

        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            if (Saniye > 0)
            {
                if (dispatcherTimer.IsEnabled)
                {
                    dispatcherTimer.Stop();
                    btnStop.Content = "Devam Et";
                }
                else
                {
                    dispatcherTimer.Start();
                    btnStop.Content = "Dur";
                }
            }
        }

        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Kapat();
        }

        private void Kapat()
        {
            if (Saniye > 0)
            {
                MessageBoxResult result = MessageBox.Show("Çıkmak istediğinize emin misiniz?", "Çıkış", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("İptal etmek istediğinize emin misiniz?", "İptal", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (Saniye > 0)
                {
                    dispatcherTimer.Stop();
                    lblSure.Content = "00:00";
                    Saniye = 0;
                    btnStop.Content = "Dur";
                    ButonlariKilitle(true);
                    ButonlarinGorunurlugu(false);

                    if (Durum == "Pomodoro")
                    {
                        PomodoroSayisi--;
                        btnPomodoro.Content = "Pomodoro (" + PomodoroSayisi + ")";
                    }
                    else if (Durum == "Kısa Mola")
                    {
                        KisaMolaSayisi--;
                        btnShortBreak.Content = "Kısa Mola (" + KisaMolaSayisi + ")";
                    }
                    else if (Durum == "Uzun Mola")
                    {
                        UzunMolaSayisi--;
                        btnLongBreak.Content = "Uzun Mola (" + UzunMolaSayisi + ")";
                    }
                    Durum = "";
                    BasliklariKalinlastir(Durum);
                }
            }
        }

        private void btnAyarlar_Click(object sender, RoutedEventArgs e)
        {
            if (!ayarlarPenceresiAcikMi)
            {
                ayarlarPenceresiAcikMi = true;

                if (Saniye < 1)
                {
                    SettingsWindow settingsWindow = new SettingsWindow();
                    settingsWindow.Show();
                }
                else
                {
                    MessageBox.Show("Ayarları açmak için " + Durum + " işlemini iptal etmeniz ya da bitirmeniz gerekmektedir.");
                }
            }
            else
            {
                MessageBox.Show("Ayarlar zaten açık");
            }
        }

        private void btnGecmis_Click(object sender, RoutedEventArgs e)
        {
            if (!gecmisPenceresiAcikMi)
            {
                gecmisPenceresiAcikMi = true;

                if (Saniye < 1)
                {
                    HistoryWindow historyWindow = new HistoryWindow();
                    historyWindow.Show();
                }
                else
                {
                    MessageBox.Show("Geçmişi açmak için " + Durum + " işlemini iptal etmeniz ya da bitirmeniz gerekmektedir.");
                }
            }
            else
            {
                MessageBox.Show("Geçmiş zaten açık");
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PTHeight = (int)Application.Current.MainWindow.Height;
            PTWidth = (int)Application.Current.MainWindow.Width;
            EkranAyarlariniKaydet();
        }

        private void EkranAyarlariniKaydet()
        {
            System.IO.File.WriteAllText(SettingsFileName,
                    BirPomodoroZamani.ToString() + "," +
                    BirKisaMolaZamani.ToString() + "," +
                    BirUzunMolaZamani.ToString() + "," +
                    PTHeight.ToString() + "," +
                    PTWidth.ToString());
        }
    }
}