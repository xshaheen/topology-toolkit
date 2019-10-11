using System;
using System.Collections.Generic;
using static Topology.Infra.TopologyUtl;
using static Topology.Infra.ParseUtl;

namespace Topology.CLI
{
    public class Program
    {
        // ToDo: Add validation and error messages.
        public static void Main()
        { 
            const string sp = "-----------------------------------";
            var isExit = false;

            while (!isExit)
            {
                Console.WriteLine("> 1. Generate all topologies defined on a set.");
                Console.WriteLine("> 2. Find limit, interior, closure, boundary and exterior points.");
                Console.WriteLine("> 3. Generate power set for a given set.");
                Console.WriteLine("X 0. Exit");
                Console.Write("? Your choice: ");
                var option = Console.ReadLine();

                Console.WriteLine(sp);

                HashSet<string> set;

                switch (option)
                {
                    case "1": // Generate all topologies defined on a set
                        Console.WriteLine("Enter set elements [Hint: a,b,c]:");
                        set = StringToSet(Console.ReadLine());
                        Console.WriteLine($"{sp}\nTopologies on: {SetToString(set)}\n{sp}");

                        var start = DateTime.Now;
                        var counter = 0;
                        foreach (var topology in Topologies(set))
                            Console.WriteLine($"{++counter,4}. {SetToString(topology)} " +
                                              $"| {DateTime.Now - start}");
                        
                        Console.WriteLine($"{sp}\nTotal number of topologies that defined on the set: {counter}\n{sp}");
                        break;
                    case "2": // Find limit points
                        Console.WriteLine("Enter set elements like [Hint: a,b,c]:");
                        set = StringToSet(Console.ReadLine());
                        Console.WriteLine("Enter subset elements like [Hint: a,b,c]:");
                        var subset = StringToSet(Console.ReadLine());
                        Console.WriteLine("Enter topology elements [Hint: {},{a,b,c},{a},{b},{a,b}]:");
                        var t = StringToSetOfSets(Console.ReadLine());
                        // Print Pints Set
                        Console.WriteLine(sp);
                        Console.WriteLine($"Limit Points: {SetToString(LimitPoints(set, subset, t))}");
                        Console.WriteLine($"Closure Points: {SetToString(ClosurePoints(set, subset, t))}");
                        Console.WriteLine($"Interior Points: {SetToString(InteriorPoints(set, subset, t))}");
                        Console.WriteLine($"Exterior Points: {SetToString(ExteriorPoints(set, subset, t))}");
                        Console.WriteLine($"Boundary Points: {SetToString(BoundaryPoints(set, subset, t))}");
                        Console.WriteLine(sp);
                        break;
                    case "3": // Find limit points
                        Console.WriteLine("Enter set elements like [Hint: a,b,c]:");
                        set = StringToSet(Console.ReadLine());
                        Console.WriteLine($"{sp}\nPowerSet of the set {SetToString(set)}: \n" +
                                          $"{SetToString(PowerSet(set))}\n{sp}");
                        break;
                    case "0":
                        isExit = true;
                        break;
                    default:
                        Console.WriteLine($"Invalid option!\n{sp}");
                        break;
                }
            }
            //////////////////
            Console.ReadLine();
        }
    }
}