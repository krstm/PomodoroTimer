using PomodoroTimer.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace PomodoroTimer.DataAccess
{
    internal class SettingDal
    {
        private Setting setting = new Setting();
        private string SettingsDbFileName { get; } = "PomodoroTimerSettings.txt";
        private string[] stringArray;

        public SettingDal(string SettingFileName)
        {
            SettingsDbFileName = SettingFileName;
        }

        public Setting ReadSettings()
        {
            if (File.Exists(SettingsDbFileName))
            {
                var allLines = File.ReadAllLines(SettingsDbFileName);
                List<string> lines = new List<string>(allLines);

                foreach (string line in lines)
                {
                    stringArray = line.Split(',');
                    setting.PomodoroSuresi = Convert.ToInt32(stringArray[0]);
                    setting.KisaMolaSuresi = Convert.ToInt32(stringArray[1]);
                    setting.UzunMolaSuresi = Convert.ToInt32(stringArray[2]);
                    setting.Height = Convert.ToInt32(stringArray[3]);
                    setting.Width = Convert.ToInt32(stringArray[4]);
                }
                return setting;
            }

            return new Setting();
        }

        public void WriteSettings(Setting setting)
        {
            if (File.Exists(SettingsDbFileName))
            {
                System.IO.File.WriteAllText(SettingsDbFileName,
                setting.PomodoroSuresi.ToString() + "," +
                setting.KisaMolaSuresi.ToString() + "," +
                setting.UzunMolaSuresi.ToString() + "," +
                setting.Height.ToString() + "," +
                setting.Width.ToString());
            }
        }
    }
}