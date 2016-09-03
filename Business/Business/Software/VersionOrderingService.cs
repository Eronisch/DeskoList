using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Business.Software
{
    public class VersionOrderingService : IComparer<string>
    {
        /// <summary>
        /// Check if left is equal, greater or lower than right
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public int Compare(string x, string y)
        {
            if (x == y) return 0;

            var version = new { First = GetVersion(x), Second = GetVersion(y) };
            int limit = Math.Max(version.First.Length, version.Second.Length);

            for (int i = 0; i < limit; i++)
            {
                int first = version.First.ElementAtOrDefault(i);
                int second = version.Second.ElementAtOrDefault(i);
                if (first > second) return 1;
                if (second > first) return -1;
            }

            return 0;
        }

        public bool Equals(string x, string y)
        {
            return Compare(x, y) == 0;

        }

        public bool IsHigher(string x, string y)
        {
            int compareResult = Compare(x, y);

            return compareResult == 1;
        }

        private int[] GetVersion(string version)
        {
            return (from part in version.Split('.')
                    select Parse(part)).ToArray();
        }

        private int Parse(string version)
        {
            int result;
            int.TryParse(version, out result);
            return result;
        }
    }
}
