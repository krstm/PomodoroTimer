using PomodoroTimer.DataAccess;
using PomodoroTimer.Entities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace PomodoroTimer.Business
{
    internal class HistoryManager
    {
        public static bool uygunluk = true;

        private HistoryDal historyDal = new HistoryDal();

        internal List<History> GetHistories()
        {
            return historyDal.ReadHistories();
        }

        private bool RegexKontrol(string text)
        {
            string pattern = @"^(?:[012]?[0-9]|3[01])[.](?:0?[1-9]|1[0-2])[.](?:[0-9]{2}){1,2}$";
            Regex regex = new Regex(pattern);
            string[] regexDizi;

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

        internal void SaveHistory(History history)
        {
            historyDal.WriteHistory(history);
        }

        internal void SaveHistories(DataGrid dataGrid)
        {
            HistoryDal historyDal = new HistoryDal();
            List<string> eklenenTarihListesi = new List<string>();
            string hataliSatir = "";
            List<string> tumListe = new List<string>();

            eklenenTarihListesi.Clear();
            tumListe.Clear();
            HistoryManager historyManager = new HistoryManager();
            uygunluk = true;

            foreach (History history in dataGrid.ItemsSource)
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
                            Int32.TryParse(history.UzunMolaSayisi.ToString(), out int value4) &&
                            Int32.TryParse(history.ToplamPomodoroDakikasi.ToString(), out int value5))
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

                                HistoryWindow.HataMesaji(hataliSatir);
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

                            HistoryWindow.HataMesaji(hataliSatir);
                            uygunluk = false;
                            break;
                        }
                    }
                    else
                    {
                        hataliSatir = "Satırlar boş olamaz";

                        HistoryWindow.HataMesaji(hataliSatir);
                        uygunluk = false;
                        break;
                    }
                }
                else
                {
                    hataliSatir = "Veritabanı değişti. Geçmiş kaydedilemez.";

                    HistoryWindow.HataMesaji(hataliSatir);
                    uygunluk = false;
                    break;
                }
            }
            if (uygunluk)
            {
                historyDal.WriteHistories(dataGrid, tumListe);
            }
        }
    }
}