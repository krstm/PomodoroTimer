using PomodoroTimer.Business;
using PomodoroTimer.DataAccess;
using PomodoroTimer.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        private void dgPrimaryGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var desc = e.PropertyDescriptor as PropertyDescriptor;
            var att = desc.Attributes[typeof(ColumnNameAttribute)] as ColumnNameAttribute;
            if (att != null)
            {
                e.Column.Header = att.Name;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            HistoryManager historyManager = new HistoryManager();
            dataGridTablo.ItemsSource = historyManager.GetHistories();
        }

        private List<string> tumListe = new List<string>();

        private static string pattern = @"^(?:[012]?[0-9]|3[01])[.](?:0?[1-9]|1[0-2])[.](?:[0-9]{2}){1,2}$";
        private Regex regex = new Regex(pattern);
        private bool uygunluk = true;
        private string hataliSatir = "";
        private List<string> eklenenTarihListesi = new List<string>();

        private string[] regexDizi;

        private bool RegexKontrol(string text)
        {
            if (regex.IsMatch(text))
            {
                regexDizi = text.Split('.');

                if (regexDizi[0].Length == 2 &&
                    regexDizi[1].Length == 2 &&
                    regexDizi[2].Length == 4 &&

                    Convert.ToInt32(regexDizi[0]) < 32 &&
                    Convert.ToInt32(regexDizi[0]) > 0 &&
                    Convert.ToInt32(regexDizi[1]) < 13 &&
                    Convert.ToInt32(regexDizi[1]) > 0 &&
                    Convert.ToInt32(regexDizi[2]) < 2099 &&
                    Convert.ToInt32(regexDizi[2]) > 1999)
                {
                    return true;
                }
            }
            return false;
        }

        private void btnKaydet_Click(object sender, RoutedEventArgs e)
        {
            eklenenTarihListesi.Clear();
            tumListe.Clear();
            HistoryManager historyManager = new HistoryManager();
            uygunluk = true;

            foreach (History history in dataGridTablo.ItemsSource)
            {
                if (!MainWindow.GecmisKapatilamaz)
                {
                    if (!(string.IsNullOrEmpty(history.Id.ToString())) &&
                    !(string.IsNullOrEmpty(history.Tarih.ToString())) &&
                    !(string.IsNullOrEmpty(history.PomodoroSayisi.ToString())) &&
                    !(string.IsNullOrEmpty(history.KisaMolaSayisi.ToString())) &&
                    !(string.IsNullOrEmpty(history.UzunMolaSayisi.ToString())) &&
                    !(string.IsNullOrEmpty(history.ToplamPomodoroDakikasi.ToString())))
                    {
                        if (RegexKontrol(history.Tarih) &&
                            Int32.TryParse(history.Id.ToString(), out int value1) &&
                            Int32.TryParse(history.PomodoroSayisi.ToString(), out int value2) &&
                            Int32.TryParse(history.KisaMolaSayisi.ToString(), out int value3) &&
                            Int32.TryParse(history.UzunMolaSayisi.ToString(), out int value4))
                        {
                            if (!eklenenTarihListesi.Contains(history.Tarih))
                            {
                                eklenenTarihListesi.Add(history.Tarih);

                                tumListe.Add(history.Tarih.ToString() + "," +
                                    history.PomodoroSayisi.ToString() + "," +
                                    history.KisaMolaSayisi.ToString() + "," +
                                    history.UzunMolaSayisi.ToString() + "," +
                                    history.ToplamPomodoroDakikasi.ToString());
                            }
                            else
                            {
                                hataliSatir = "Sıra: " + history.Id.ToString() +
                                ", Tarih: " + history.Tarih.ToString() +
                                ", Pomodoro Sayısı: " + history.PomodoroSayisi.ToString() +
                                ", Kısa Mola Sayısı: " + history.KisaMolaSayisi.ToString() +
                                ", Uzun Mola Sayısı: " + history.UzunMolaSayisi.ToString() +
                                ", Toplam Pomodoro Dakikası: " + history.ToplamPomodoroDakikasi.ToString() +
                                Environment.NewLine +
                                Environment.NewLine +
                                "Aynı tarih birden fazla satırda olamaz";

                                MessageBox.Show(hataliSatir);
                                uygunluk = false;
                                break;
                            }
                        }
                        else
                        {
                            hataliSatir = "Sıra: " + history.Id.ToString() +
                                ", Tarih: " + history.Tarih.ToString() +
                                ", Pomodoro Sayısı: " + history.PomodoroSayisi.ToString() +
                                ", Kısa Mola Sayısı: " + history.KisaMolaSayisi.ToString() +
                                ", Uzun Mola Sayısı: " + history.UzunMolaSayisi.ToString() +
                                ", Toplam Pomodoro Dakikası: " + history.ToplamPomodoroDakikasi.ToString() +
                                Environment.NewLine +
                                Environment.NewLine +
                                "Satır hatalı";

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
                else
                {
                    hataliSatir = "Veritabanı değişti. Geçmiş kaydedilemez.";

                    MessageBox.Show(hataliSatir);
                    uygunluk = false;
                    break;
                }
            }

            if (uygunluk)
            {
                System.IO.File.WriteAllLines(HistoryDal.HistoryDbFileName, tumListe);

                MainWindow.PomodoroSayisi = historyManager.GetHistories().Last().PomodoroSayisi;
                MainWindow.KisaMolaSayisi = historyManager.GetHistories().Last().KisaMolaSayisi;
                MainWindow.UzunMolaSayisi = historyManager.GetHistories().Last().UzunMolaSayisi;

                MainWindow.IsTheHistoryWindowOpen = false;
                this.Close();
            }
        }

        private void btnKapat_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.GecmisKapatilamaz = false;
            MainWindow.IsTheHistoryWindowOpen = false;
            this.Close();
        }
    }
}