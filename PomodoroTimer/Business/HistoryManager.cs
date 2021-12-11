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

        internal void SaveHistory(History history)
        {
            historyDal.WriteHistory(history);
        }

        internal void SaveHistories(List<History> historyList)
        {
            historyDal.WriteHistories(historyList);
        }
    }
}