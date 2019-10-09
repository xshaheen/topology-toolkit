using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using static Topology.Infra.TopologyUtl;
using static Topology.Infra.ParseUtl;

namespace Topology.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        #region Tabs

        #region Topology

        private void GenerateTopologiesBtn_Click(object sender, RoutedEventArgs e)
        {
            var set = StringToSet(TSetTextBox.Text);
            
            var counter = 0;
            foreach (var topology in Topologies(set))
            {
                TopologiesDataGrid.Items.Add(new {Index = ++counter, Topology = SetToString(topology)});
            }
        }

        private void GenerateToExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            var set = StringToSet(TSetTextBox.Text);

            TopologiesToExcel(set, SortCheckBox.IsEnabled);
        }

        #endregion

        #region Points

        private void FindPointsBtn_Click(object sender, RoutedEventArgs e)
        {

            var set = StringToSet(PSetTextBox.Text);
            var subset = StringToSet(SubsetTextBox.Text);
            var t = StringToSetOfSets(TopologyTextBox.Text);

            var func = PointsComboBox.Text;

            var result = func switch
            {
                "Limit Points" => LimitPoints(set, subset, t),
                "Closure Points" => ClosurePoints(set, subset, t),
                "Interior Points" => InteriorPoints(set, subset, t),
                "Exterior Points" => ExteriorPoints(set, subset, t),
                "Boundary Points" => BoundaryPoints(set, subset, t),
                _ => null
            };

            PointsResult.Text = result == null 
                ? "Select the points set." : SetToString(result);
        }

        #endregion

        #region PowerSet

        private void GeneratePowerSetBtn_Click(object sender, RoutedEventArgs e)
        {
            var set = StringToSet(PsSetTextBox.Text);
            var counter = 0;
            foreach (var subset in PowerSet(set))
                PowerSetDataGrid.Items.Add(new {Index = ++counter, Subset = SetToString(subset)});
        }

        #endregion

        #endregion

        private void GitHubBtn_Click(object sender, RoutedEventArgs e) 
            => OpenBrowser(@"https://github.com/shaheenzx/");

        private void LinkedInBtn_Click(object sender, RoutedEventArgs e) 
            => OpenBrowser(@"https://www.linkedin.com/in/shaheenzx/");

        #region To Excel

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
                MessageBox.Show($"Error: {e.Message} Line: {e.Source}", "Error!");
            }
        }

        private static int CompareSetByLength<T>(HashSet<T> x, HashSet<T> y)
        {
            // If x is not null and y is null, x is greater.
            // If x is not null and y is not null, compare the lengths of the two strings.
            if (x != null) return y == null ? 1 : x.Count.CompareTo(y.Count);

            // If x is null and y is null, they're equal. 
            if (y == null) return 0;

            // If x is null and y is not null, y is greater. 
            return -1;
        }

        #endregion

        #region Helper Method

        public static void OpenBrowser(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion
    }
}