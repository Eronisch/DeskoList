using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Models.Statistics
{
    public class BundleStatisticsModel
    {
        public BundleStatisticsModel(IList<MonthStatisticsModel> @in, IList<MonthStatisticsModel> @out,
            IList<MonthStatisticsModel> uniqueIn, IList<MonthStatisticsModel> uniqueOut)
        {
            In = @in;
            Out = @out;
            UniqueIn = uniqueIn;
            UniqueOut = uniqueOut;
        }

        public IList<MonthStatisticsModel> In { get; private set; }
        public IList<MonthStatisticsModel> Out { get; private set; }
        public IList<MonthStatisticsModel> UniqueIn { get; private set; }
        public IList<MonthStatisticsModel> UniqueOut { get; private set; }

        #region Total

        public int GetTotalIn()
        {
            return In.Sum(x => x.Amount);
        }

        public int GetTotalUniqueIn()
        {
            return UniqueIn.Sum(x => x.Amount);
        }

        public int GetTotalOut()
        {
            return Out.Sum(x => x.Amount);
        }
        

        public int GetTotalUniqueOut()
        {
            return UniqueOut.Sum(x => x.Amount);
        }

        #endregion

        #region Average

        public int GetInAverage()
        {
            return (int)Math.Round(In.Average(x => x.Amount), 0);
        }

        public int GetOutAverage()
        {
            return (int)Math.Round(Out.Average(x => x.Amount), 0);
        }

        public int GetUniqueInAverage()
        {
            return (int)Math.Round(UniqueIn.Average(x => x.Amount), 0);
        }

        public int GetUniqueOutAverage()
        {
            return (int)Math.Round(UniqueOut.Average(x => x.Amount), 0);
        }
      
        #endregion
    }
}
