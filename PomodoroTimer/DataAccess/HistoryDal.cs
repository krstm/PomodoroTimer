using PomodoroTimer.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PomodoroTimer.DataAccess
{
    internal class HistoryDal
    {
        private History history = new History();
        private List<History> historyList = new List<History>();
        private string[] stringArray;
        internal static string HistoryDbFileName = "PomodoroTimerDb.txt";
        private int count = 1;

        public List<History> ReadHistories()
        {
            if (File.Exists(HistoryDbFileName))
            {
                var allLines = File.ReadAllLines(HistoryDbFileName);
                List<string> lines = new List<string>(allLines);

                foreach (string line in lines)
                {
                    stringArray = line.Split(',');

                    historyList.Add(new History()
                    {
                        Id = count,
                        Tarih = stringArray[0],
                        PomodoroSayisi = Convert.ToInt32(stringArray[1]),
                        KisaMolaSayisi = Convert.ToInt32(stringArray[2]),
                        UzunMolaSayisi = Convert.ToInt32(stringArray[3]),
                        ToplamPomodoroDakikasi = Convert.ToInt32(stringArray[4]),
                    });

                    count++;
                }

                return historyList;
            }
            else
            {
                return new List<History>();
            }
        }

        public void WriteHistory(History history)
        {
            List<History> histories = new List<History>();
            HistoryDal historyDal = new HistoryDal();
            List<string> allLines = new List<string>();

            histories = historyDal.ReadHistories();

            if (histories.Last().Tarih == DateTime.Now.ToString("dd/MM/yyyy"))
            {
                histories.Remove(histories.Last());
                histories.Add(history);
            }
            else
            {
                histories.Add(history);
            }

            foreach (History h in histories)
            {
                allLines.Add(
                    h.Tarih + "," +
                    h.PomodoroSayisi + "," +
                    h.KisaMolaSayisi + "," +
                    h.UzunMolaSayisi + "," +
                    h.ToplamPomodoroDakikasi);
            }

            System.IO.File.WriteAllLines(HistoryDbFileName, allLines);
        }

        public void WriteHistories(List<History> histories)
        {
            List<string> tumListe = new List<string>();

            foreach (History history in histories)
            {
                tumListe.Add(history.Tarih.ToString() + "," +
                                    history.PomodoroSayisi.ToString() + "," +
                                    history.KisaMolaSayisi.ToString() + "," +
                                    history.UzunMolaSayisi.ToString() + "," +
                                    history.ToplamPomodoroDakikasi.ToString());
            }

            System.IO.File.WriteAllLines(HistoryDbFileName, tumListe);
        }
    }
}