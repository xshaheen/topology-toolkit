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
            
            TopologiesDataGrid.Items.Clear();
            
            try
            {
                var sort = SortCheckBox.IsChecked != null && SortCheckBox.IsChecked != false;

                if (sort && set.Count > 4)
                {
                    MessageBox.Show("I can not sort topologies defined on a set has element > 4 it cost a lot of time!", "Error");
                    return;
                }

                if (sort)
                {  
                    var sortedTopologies = Topologies(set).ToList();
                    sortedTopologies.Sort(CompareSetByLength);

                    var counter = 0;
                    foreach (var topology in sortedTopologies)
                        TopologiesDataGrid.Items.Add(new ItemData{Index = ++counter, Topology = SetToString(topology)});
                }
                else
                {
                    var counter = 0;
                    foreach (var topology in Topologies(set))
                        TopologiesDataGrid.Items.Add(new ItemData{Index = ++counter, Topology = SetToString(topology)});
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!");
            }
            
        }

        private void GenerateToExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            var set = StringToSet(TSetTextBox.Text);

            TopologiesToExcel(set, SortCheckBox.IsChecked != null && SortCheckBox.IsChecked != false);
        }

        #endregion

        #region Points

        private void FindPointsBtn_Click(object sender, RoutedEventArgs e)
        {

            var setBox = PSetTextBox.Text;
            if (setBox.Length == 0)
            {
                MessageBox.Show("Please fill the set field.", "Required Field!");
                return;
            }

            var subsetBox = SubsetTextBox.Text;
            if (subsetBox.Length == 0)
            {
                MessageBox.Show("Please fill the subset field.", "Required Field!");
                return;
            }

            var tBox = TopologyTextBox.Text;
            if (tBox.Length == 0)
            {
                MessageBox.Show("Please fill the topology field.", "Required Field!");
                return;
            }

            var set = StringToSet(setBox);
            var subset = StringToSet(subsetBox);
            var t = StringToSetOfSets(tBox);

            var func = PointsComboBox.Text;

            if (func.Length == 0)
            {
                MessageBox.Show("Please, select the points set.", "Required Field!");
                return;
            }

            try
            {
                var result = func switch
                {
                    "Limit Points" => LimitPoints(set, subset, t),
                    "Closure Points" => ClosurePoints(set, subset, t),
                    "Interior Points" => InteriorPoints(set, subset, t),
                    "Exterior Points" => ExteriorPoints(set, subset, t),
                    "Boundary Points" => BoundaryPoints(set, subset, t),
                    _ => null
                };

                PointsResult.Text = SetToString(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error!");
            }

        }

        #endregion

        #region PowerSet

        private void GeneratePowerSetBtn_Click(object sender, RoutedEventArgs e)
        {
            var setBox = PsSetTextBox.Text;
            if (setBox.Length == 0)
            {
                MessageBox.Show("Please fill the set field.", "Required Field!");
                return;
            }

            var set = StringToSet(setBox);

            PowerSetDataGrid.Items.Clear();
            var counter = 0;

            try
            {
                foreach (var subset in PowerSet(set))
                    PowerSetDataGrid.Items.Add(new {Index = ++counter, Subset = SetToString(subset)});
            }
            catch (Exception exception)
            {
                MessageBox.Show($"Error: {exception.Message}", "Error!");
                throw;
            }
        }

        #endregion

        #endregion

        #region Footer

        private void GitHubBtn_Click(object sender, RoutedEventArgs e) 
            => OpenBrowser(@"https://github.com/shaheenzx/");

        private void LinkedInBtn_Click(object sender, RoutedEventArgs e) 
            => OpenBrowser(@"https://www.linkedin.com/in/shaheenzx/");

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

        #endregion

        #region To Excel

        public static void TopologiesToExcel<T>(HashSet<T> set, bool sort = false)
        {
            try
            {
                if (sort && set.Count > 4)
                {
                    MessageBox.Show("I can not sort topologies defined on a set has element > 4 it cost a lot of time!", "Error");
                    return;
                }

                // Just for force the method to check the assertions and
                // throw any exception before open the excel
                var _ = Topologies(set).FirstOrDefault();

                // Start Excel and get Application object.
                var excelApp = new Excel.Application {Visible = true};
                excelApp.Workbooks.Add();
                var sheet = (Excel._Worksheet) excelApp.ActiveSheet;
        
                // Add table headers going cell by cell.
                sheet.Cells[1, "A"] = "Number";
                sheet.Cells[1, "B"] = "Topologies";


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
                    foreach (var topology in Topologies(set))
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
                MessageBox.Show($"Error: {e.Message}.", "Error!");
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
    }

    class ItemData
    {
        public int Index { get; set; }
        public string Topology { get; set; }
    }
}