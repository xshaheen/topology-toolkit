using System;
using System.Collections.Generic;

namespace Topology.Infra
{
    public class ParseUtl
    {
        public static HashSet<string> StringToSet(string set)
        {
            var hashSet = new HashSet<string>();

            if (set == null) return hashSet;

            var elements = set.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var element in elements) hashSet.Add(element.Trim());

            return hashSet;
        }

        public static HashSet<HashSet<string>> StringToSetOfSets(string set)
        {
            var hashSet = new HashSet<HashSet<string>>();

            if (set == null) return hashSet;

            var elements = set.Split("},", StringSplitOptions.RemoveEmptyEntries);
            foreach (var e in elements)
                hashSet.Add(StringToSet(
                    e.Replace("{", "").Replace("}", "")));

            return hashSet;
        }
    }
}