using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Excel = Microsoft.Office.Interop.Excel;
using static Topology.Infra.TopologyUtl;

namespace Topology.CLI
{
    public class Program
    {
        private const string Sp = "-----------------------------------";

        public static void Main()
        {
            var isExit = false;

            while (!isExit)
            {
                Console.WriteLine("> 1. Generate all topologies defined on a set.");
                Console.WriteLine("> 2. Export all topologies to Excel file.");
                Console.WriteLine("> 3. Find limit, interior, closure, boundary and exterior points.");
                Console.WriteLine("X 0. Exit");
                Console.Write("? Your choice: ");
                var option = Console.ReadLine();
                Console.WriteLine(Sp);

                HashSet<string> set;

                switch (option)
                {
                    case "1": // Generate all topologies defined on a set
                        Console.WriteLine("Enter set elements [Hint: a,b,c]:");
                        set = StringToSet(Console.ReadLine());
                        TopologiesToConsole(set);
                        break;
                    case "2": // Export all topologies to Excel file
                        Console.WriteLine("Enter set elements [Hint: a,b,c]:");
                        set = StringToSet(Console.ReadLine());

                        while (true)
                        {
                            Console.Write("Sort it [Y/N]:");
                            var sort = Console.ReadLine();

                            if (string.Equals(sort, "Y", StringComparison.InvariantCultureIgnoreCase))
                            {
                                TopologiesToExcel(set, true);
                                break;
                            }
                            
                            if (string.Equals(sort, "N", StringComparison.InvariantCultureIgnoreCase))
                            {
                                TopologiesToConsole(set);
                                break;
                            }

                        }
                        Console.WriteLine(Sp);
                        TopologiesToExcel(set);
                        break;
                    case "3": // Find limit points
                        Console.WriteLine("Enter set elements like [Hint: a,b,c]:");
                        set = StringToSet(Console.ReadLine());
                        Console.WriteLine("Enter subset elements like [Hint: a,b,c]:");
                        var subset = StringToSet(Console.ReadLine());
                        Console.WriteLine("Enter topology elements [Hint: {},{a,b,c},{a},{b},{a,b}]:");
                        var t = StringToSetOfSets(Console.ReadLine());
                        PrintPoints(set, subset, t);
                        break;
                    case "0":
                        isExit = true;
                        break;
                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
            //////////////////
            Console.ReadLine();
        }

        public static void PrintPoints<T>(HashSet<T> set, HashSet<T> subset, HashSet<HashSet<T>> t)
        {
            Console.WriteLine(Sp);
            Console.WriteLine($"Limit Points: {SetToString(LimitPoints(set, subset, t))}");
            Console.WriteLine($"Closure Points: {SetToString(ClosurePoints(set, subset, t))}");
            Console.WriteLine($"Interior Points: {SetToString(InteriorPoints(set, subset, t))}");
            Console.WriteLine($"Exterior Points: {SetToString(ExteriorPoints(set, subset, t))}");
            Console.WriteLine($"Boundary Points: {SetToString(BoundaryPoints(set, subset, t))}");
            Console.WriteLine(Sp);
        }

        public static void TopologiesToExcel<T>(HashSet<T> set, bool sort = false)
        {
            try
            {
                // Start Excel and get Application object.
                var excelApp = new Excel.Application {Visible = true};
                excelApp.Workbooks.Add();
                var sheet = (Excel._Worksheet) excelApp.ActiveSheet;
        
                // Add table headers going cell by cell.
                sheet.Cells[1, "A"] = "Number";
                sheet.Cells[1, "B"] = "Topologies";

                var topologies = Topologies(set);

                if (sort)
                {
                    var sortedTopologies = Topologies(set).ToList();
                    sortedTopologies.Sort(CompareSetByLength);

                    var i = 2;
                    foreach (var topology in sortedTopologies)
                    {
                        sheet.Cells[i, "A"] = i - 1;
                        sheet.Cells[i, "B"] = SetToString(topology);
                        i++;
                    }
                }
                else
                {
                    var i = 2;
                    foreach (var topology in topologies)
                    {
                        sheet.Cells[i, "A"] = i - 1;
                        sheet.Cells[i, "B"] = SetToString(topology);
                        i++;
                    }
                }

                // AutoFit columns A:B
                var range = sheet.Range["A1", "B1"];
                range.EntireColumn.AutoFit();

                // Give our table data a nice look and feel.
                range = sheet.Range["A1"];
                range.AutoFormat(Excel.XlRangeAutoFormat.xlRangeAutoFormatClassic2);

                // Format A1:D1 as bold, V/H alignment = center
                range = sheet.Range["A1", "B1"];
                range.Font.Bold = true;
                // range.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
                range.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                // Make sure Excel is visible and give the user control
                // of Microsoft Excel's lifetime.
                excelApp.Visible = true;
                excelApp.UserControl = true;
        
                excelApp.Quit();
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message} Line: {e.Source}");
            }
        }

        public static void TopologiesToConsole<T>(HashSet<T> set)
        {
            Console.WriteLine($"{Sp}\nTopologies on: {SetToString(set)}\n{Sp}");

            var start = DateTime.Now;
            var counter = 0;
            foreach (var t in Topologies(set))
            {
                Console.WriteLine($"{++counter,4}. {SetToString(t)} " +
                                  $"| {DateTime.Now - start}");
            }
            Console.WriteLine($"{Sp}\nTotal number of topologies that defined on the set: {counter}\n{Sp}");
        }

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

        private static int CompareSetByLength<T>(HashSet<T> x, HashSet<T> y)
        {
            if (x == null)
            {
                if (y == null)
                    // If x is null and y is null, they're equal. 
                    return 0;

                // If x is null and y is not null, y is greater. 
                return -1;
            }

            // If x is not null...
            
            // ...and y is null, x is greater.
            if (y == null) return 1;

            // ...and y is not null, compare the lengths of the two strings.
            return x.Count.CompareTo(y.Count);
        }
    }
}