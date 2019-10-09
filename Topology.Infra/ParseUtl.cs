using System;
using System.Collections.Generic;

namespace Topology.Infra
{
    public class ParseUtl
    {
        public static HashSet<string> StringToSet(string set)
        {
            var elements = set.Split(',', StringSplitOptions.RemoveEmptyEntries);
            if (elements.Length == 0) return new HashSet<string>();

            var hashSet = new HashSet<string>();
            foreach (var element in elements) hashSet.Add(element.Trim());

            return hashSet;
        }

        public static HashSet<HashSet<string>> StringToSetOfSets(string set)
        {
            var elements = set.Split("},", StringSplitOptions.RemoveEmptyEntries);

            var hashSet = new HashSet<HashSet<string>>();

            foreach (var e in elements)
                hashSet.Add(StringToSet(
                    e.Replace("{", "").Replace("}", "")));

            return hashSet;
        }
    }
}