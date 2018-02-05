using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserActivity.Viewer.Patterns
{
    public class PatternHelper
    {
        public static string[] SelectClasses(string[][] sessions)
        {
            var classes = sessions.DistinctMany();
            return classes;
        }

        public static string[][] GeneratePatterns(string[] classes, int minLen, int maxLen)
        {
            var patterns = classes.Variations(minLen, maxLen);
            return patterns;
        }

        public static int CalculateCount(string[] pattern, string[][] sessions)
        {
            int sum = sessions.Sum(s => s.Count(pattern));
            return sum;
        }

        public static double CalculateSupport(string[] pattern, string[][] sessions)
        {
            int total = sessions.Sum(s => s.Length);
            int sum = sessions.Sum(s => s.Count(pattern));
            double support = (double)sum * (double)pattern.Length / (double)total;
            return support;
        }
    }
}
