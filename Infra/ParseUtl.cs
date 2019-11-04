using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;

namespace Infra
{
    public class ParseUtl
    {
        /// <summary>
        /// Converts the set to printable string.
        /// </summary>
        /// <typeparam name="T">Type of set elements.</typeparam>
        /// <param name="set">The set to convert to string</param>
        /// <returns>Printable string represent the set.</returns>
        public static string SetToString<T>(HashSet<T> set)
        {
            var sb = new StringBuilder("{");

            if (set != null)
            {
                foreach (var e in set) sb.Append(e + ", ");

                var len = sb.Length;
                // remove the extra ", "
                if (len > 1) sb.Remove(len - 2, 2);
            }

            sb.Append("}");

            return sb.ToString();
        }

        /// <summary>
        /// Converts the set of sets to printable string.
        /// </summary>
        /// <typeparam name="T">Type of set elements.</typeparam>
        /// <param name="set">The set to convert to string</param>
        /// <returns>Printable string represent the set.</returns>
        public static string SetToString<T>(HashSet<HashSet<T>> set)
        {
            var sb = new StringBuilder("{ ");

            if (set != null)
            {
                foreach (var e in set) sb.Append(SetToString(e) + ", ");

                // remove the extra ", "
                var len = sb.Length;
                if (len > 1) sb.Remove(len - 2, 2);
            }

            sb.Append(" }");

            return sb.ToString();
        }

        /// <summary>
        /// Converts the string to a HashSet.
        /// 1. Elements must separated using commas ','
        /// 2. Element must be mix of A-Z, a-z, 0-9, '_'
        /// 3. The brackets are optional.
        /// 4. Empty set (phi) is assumed as a subset of the set.
        /// </summary>
        public static HashSet<string> StringToSet(string set)
        {
            var hashSet = new HashSet<string>();

            // Set of sets pattern
            // var setsPattern = new Regex(pattern: @"^\s*(?<set>\{(.)*\})\s*,?");

            // Set of elements pattern
            // var elementsPattern = new Regex(@"^\G(\s*(?<element>\w+)\s*,?\s*)+\b");

            if (set == null || set == "{}" || set == "{ }") return hashSet;

            set = set.Replace("{", "");
            set = set.Replace("}", "");

            var elements = set.Split(',', StringSplitOptions.RemoveEmptyEntries);
            foreach (var element in elements) hashSet.Add(element.Trim());

            return hashSet;
        }

        /// <summary>
        /// Converts the string to a HashSet which its elements is a HashSet. elements
        /// must be formatted like {1, 2}, { 
        /// </summary>
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

        public static void ExportTopologiesToExcel(
            HashSet<string> topologies,
            string path)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (topologies == null) throw new ArgumentNullException(nameof(topologies));

            // creates a blank workbook.
            using var p = new ExcelPackage();

            // a workbook must have at least on cell, so lets add one... 
            var sheet = p.Workbook.Worksheets.Add("Exported Topologies Sheet");

            // fill Header
            sheet.Cells[1, 1].Value = "N";
            sheet.Cells[1, 2].Value = "Topologies";

            // fill table rows.
            var x = 2;
            foreach (var topology in topologies)
            {
                sheet.Cells[x, 1].Value = x - 1;
                sheet.Cells[x, 2].Value = topology;
                x++;
            }

            // style header
            sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

            using var range = sheet.Cells[1, 1, 1, 2];
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
            range.Style.Font.Color.SetColor(Color.White);

            //Save the new workbook.
            p.SaveAs(new FileInfo(path));
        }

        public static void ExportTopologiesToExcel(
            HashSet<string> topologies,
            string path,
            CancellationToken ct,
            IProgress<double> progress)
        {
            if (path == null) throw new ArgumentNullException(nameof(path));
            if (topologies == null) throw new ArgumentNullException(nameof(topologies));

            try
            {
                progress.Report(0);

                // creates a blank workbook.
                using var p = new ExcelPackage();

                // a workbook must have at least on cell, so lets add one... 
                var sheet = p.Workbook.Worksheets.Add("Exported Topologies Sheet");

                // fill Header
                sheet.Cells[1, 1].Value = "N";
                sheet.Cells[1, 2].Value = "Topologies";

                // fill table rows.
                var x = 2;
                var n = topologies.Count;
                foreach (var topology in topologies)
                {
                    ct.ThrowIfCancellationRequested();
                    progress.Report((double)(x - 1) / n * 100);

                    sheet.Cells[x, 1].Value = x - 1;
                    sheet.Cells[x, 2].Value = topology;
                    x++;
                }

                // style header
                sheet.Cells[sheet.Dimension.Address].AutoFitColumns();

                using var range = sheet.Cells[1, 1, 1, 2];
                range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(Color.RoyalBlue);
                range.Style.Font.Color.SetColor(Color.White);

                //Save the new workbook.
                p.SaveAs(new FileInfo(path));
            }
            finally
            {
                progress.Report(0);
            }
        }

    }
}