using System.Linq;
using ObjectsComparer;
using schools_web_api.Model;
using System.Collections.Generic;

namespace schools_web_api.TokenManager 
{
    public static class FullSchoolExtensions
    {
        public static (bool isDifferent, List<Difference> differenses) isDifferentThan(this FullSchool f1, FullSchool f2)
        {
            var comparer = new ObjectsComparer.Comparer<FullSchool>();

            IEnumerable<Difference> diffs;

            var areEqual = comparer.Compare(f1, f2, out diffs);

            List<Difference> differences = diffs.ToList();

            return (!areEqual, differences);
        }
    }
}