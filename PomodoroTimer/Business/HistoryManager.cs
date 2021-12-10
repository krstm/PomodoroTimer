using PomodoroTimer.DataAccess;
using PomodoroTimer.Entities;
using System.Collections.Generic;

namespace PomodoroTimer.Business
{
    internal class HistoryManager
    {
        private HistoryDal historyDal = new HistoryDal();

        internal List<History> GetHistories()
        {
            return historyDal.ReadHistories();
        }

        internal void SaveHistories(History history)
        {
            historyDal.WriteHistories(history);
        }
    }
}