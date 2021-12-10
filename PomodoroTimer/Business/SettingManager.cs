using PomodoroTimer.DataAccess;
using PomodoroTimer.Entities;

namespace PomodoroTimer.Business
{
    internal class SettingManager
    {
        public static string SettingsDatabaseFileName { get; } = "PomodoroTimerSettings.txt";
        private SettingDal settingDal = new SettingDal(SettingsDatabaseFileName);

        internal Setting GetSettings()
        {
            return settingDal.ReadSettings();
        }

        internal void SaveSettings(Setting setting)
        {
            settingDal.WriteSettings(setting);
        }
    }
}