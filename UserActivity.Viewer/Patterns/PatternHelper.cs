using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.Patterns
{
    public class PatternHelper
    {
        public static int[] SelectClasses(int[][] sessions)
        {
            var classes = sessions.DistinctMany();
            return classes;
        }

        public static int[][] GeneratePatterns(int[] classes, int minLen, int maxLen)
        {
            var patterns = classes.Variations(minLen, maxLen);
            return patterns;
        }

        public static int CalculateCount(int[] pattern, int[][] sessions)
        {
            int sum = sessions.Sum(s => s.Count(pattern));
            return sum;
        }

        public static double CalculateSupport(int[] pattern, int[][] sessions)
        {
            int total = sessions.Sum(s => s.Length);
            int sum = sessions.Sum(s => s.Count(pattern));
            double support = (double)sum * (double)pattern.Length / (double)total;
            return support;
        }
    }
}
