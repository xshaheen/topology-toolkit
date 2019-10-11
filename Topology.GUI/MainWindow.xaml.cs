using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using static Topology.Infra.ParseUtl;
using static Topology.Infra.TopologyUtl;

namespace Topology.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    // ReSharper disable once RedundantExtendsListEntry
    public partial class MainWindow : Window
    {
        private CancellationTokenSource _cancelToken;
        
        private Progress<double> _progressOperation;

        public MainWindow()
        {
            InitializeComponent();
            TopologyTabCancelBtn.IsEnabled = false;
        }

        #region Tabs

        #region Topology

        private async void GenerateTopologiesBtn_Click(object sender, RoutedEventArgs e)
        {
            var set = StringToSet(TopologyTabSetTextBox.Text);

            var sort = TopologyTabSortCheckBox.IsChecked != null && TopologyTabSortCheckBox.IsChecked != false;
            if (sort && set.Count > 4)
            {
                MessageBox.Show("I can not sort topologies defined on a set has element > 4 it cost a lot of time!", "Error");
                return;
            }

            _cancelToken = new CancellationTokenSource();
            GenerateTopologiesBtn.IsEnabled = false;
            ToExcelBtn.IsEnabled = false;
            TopologyTabCancelBtn.IsEnabled = true;

            _progressOperation = new Progress<double>(value => TopologyTabProcessBar.Value = value);

            TopologyTabProcessBar.Visibility = Visibility.Visible;

            TopologiesDataGrid.Items.Clear();

            TopologyTabStatusTxt.Text = "Generating...";

            try
            {
                if (sort)
                {  
                    var topologies = Topologies(set).ToList();
                    topologies.Sort(CompareSetByLength);

                    var counter = 0;
                    foreach (var topology in topologies)
                        TopologiesDataGrid.Items.Add(new ItemData
                            {Index = ++counter, Topology = SetToString(topology)});
                }
                else
                {
                    await TopologiesToDataGridAsync(set, _cancelToken.Token, _progressOperation);
                }

                TopologyTabStatusTxt.Text ="Operation Completed.";
            }
            catch (OperationCanceledException ex)
            {
                TopologyTabStatusTxt.Text ="Operation Cancelled | " + ex.Message;
            }
            catch (Exception ex)
            {
                TopologyTabStatusTxt.Text = "Operation Cancelled | " + ex.Message;
            }
            finally
            {
                _cancelToken.Dispose();
                GenerateTopologiesBtn.IsEnabled = true;
                ToExcelBtn.IsEnabled = true;
                TopologyTabCancelBtn.IsEnabled = false;
                TopologyTabProcessBar.Visibility = Visibility.Hidden;
            }
        }

        public async Task<HashSet<HashSet<HashSet<T>>>> TopologiesToDataGridAsync<T>(HashSet<T> set, CancellationToken ct, IProgress<double> progress)
        {
            // if > 6 will case overflow in the long type.
            if (set.Count > 5) throw new Exception("Set elements must be less than 6 elements.");

            var topologies = new HashSet<HashSet<HashSet<T>>>();
            progress.Report(0);

            await Task.Run(() =>
            {
                var powerSet = PowerSet(set);

                // remove the set and the empty set. for example, for set of 4 element this
                // make the complexity decrease from 2^(2^4)= 65,536 to 2^(2^4-2)= 16,384
                powerSet.RemoveWhere(s => s.Count == 0);         // O(2^set.Count)
                powerSet.RemoveWhere(s => s.Count == set.Count); // O(2^set.Count)

                var counter = 0;
                var n = 1L << powerSet.Count;
                // loop to get all n subsets
                for (long i = 0; i < n; i++)
                {
                    ct.ThrowIfCancellationRequested();

                    if (i % 100 == 0)
                    {
                        var x = i / (decimal) n;
                        var p = 100 * x;
                        progress.Report((double)p);
                    }

                    var subset = new HashSet<HashSet<T>>();

                    // loop though every element in the set and determine with number 
                    // should present in the current subset.
                    var j = 0;
                    foreach (var e in powerSet)
                        // if the jth element (bit) in the ith subset (binary number of i) add it.
                        if (((1L << j++) & i) > 0)
                            subset.Add(e);

                    subset.Add(new HashSet<T>());
                    subset.Add(set);

                    if (IsTopology(subset, set))
                    {
                        this.Dispatcher?.Invoke(delegate
                            {
                                TopologiesDataGrid.Items.Add(new ItemData
                                    {Index = ++counter, Topology = SetToString(subset)});
                            }
                        );
                    }
                }
            }, ct);

            return topologies;

        }

        private async void GenerateToExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            var set = StringToSet(TopologyTabSetTextBox.Text);
            var sort = TopologyTabSortCheckBox.IsChecked != null && TopologyTabSortCheckBox.IsChecked != false;

            if (sort && set.Count > 4)
            {
                MessageBox.Show("I can not sort topologies defined on a set that has greater than 4 elements. It cost a lot of time!", "Error");
                return;
            }

            _cancelToken = new CancellationTokenSource();
            GenerateTopologiesBtn.IsEnabled = false;
            ToExcelBtn.IsEnabled = false;
            TopologyTabCancelBtn.IsEnabled = true;

            TopologyTabStatusTxt.Text = "Generating...";

            try
            {
                await Task.Run(() => TopologiesToExcel(set, _cancelToken.Token, sort));
                TopologyTabStatusTxt.Text ="Operation Completed.";
            }
            catch (OperationCanceledException ex)
            {
                TopologyTabStatusTxt.Text ="Operation Cancelled | " + ex.Message;
            }
            catch (Exception ex)
            {
                TopologyTabStatusTxt.Text = "Operation Cancelled | " + ex.Message;
            }
            finally
            {
                _cancelToken.Dispose();
                GenerateTopologiesBtn.IsEnabled = true;
                ToExcelBtn.IsEnabled = true;
                TopologyTabCancelBtn.IsEnabled = false;
            }
        }

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _cancelToken.Cancel();
        }

        #endregion

        #region Points

        private void FindPointsBtn_Click(object sender, RoutedEventArgs e)
        {

            var setBox = PointsTabSetTextBox.Text;
            if (setBox.Length == 0)
            {
                MessageBox.Show("Please fill the set field.", "Required Field!");
                return;
            }

            var subsetBox = PointsTabSubsetTextBox.Text;
            if (subsetBox.Length == 0)
            {
                MessageBox.Show("Please fill the subset field.", "Required Field!");
                return;
            }

            var tBox = PointsTabTopologyTextBox.Text;
            if (tBox.Length == 0)
            {
                MessageBox.Show("Please fill the topology field.", "Required Field!");
                return;
            }

            var set = StringToSet(setBox);
            var subset = StringToSet(subsetBox);
            var t = StringToSetOfSets(tBox);

            var func = PointsTabPointsComboBox.Text;

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

                PointsTabPointsResult.Text = SetToString(result);
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

        public static void TopologiesToExcel<T>(HashSet<T> set, CancellationToken ct, bool sort = false)
        {
            try
            {
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
                        ct.ThrowIfCancellationRequested();
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
                        ct.ThrowIfCancellationRequested();
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
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message} | {e.Source}", "Error!");
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

        private void FindSubsetsPointsBtn_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }

    class ItemData
    {
        public int Index { get; set; }
        public string Topology { get; set; }
    }
}