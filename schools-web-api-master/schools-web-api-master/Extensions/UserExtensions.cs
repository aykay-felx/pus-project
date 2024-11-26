using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ObjectsComparer;
using schools_web_api.TokenManager.Extensions;
using schools_web_api.Model;

namespace schools_web_api.TokenManager 
{
    public static class UserExtensions
    {
        public static (bool isDifferent, List<Difference> differenses) isDifferentThan(this User u1, User u2)
        {
            var comparer = new ObjectsComparer.Comparer<User>();

            IEnumerable<Difference> diffs;

            var areEqual = comparer.Compare(u1, u2, out diffs);

            List<Difference> differences = diffs.ToList();

            return (!areEqual, differences);
        }
    }
}