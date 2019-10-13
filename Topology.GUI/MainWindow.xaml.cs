using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using static Topology.Infra.ParseUtl;
using static Topology.Infra.TopologyUtl;

namespace Topology.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly HashSet<string> _topologies;

        private CancellationTokenSource _cancelToken;
        
        private Progress<double> _progressOperation;

        public MainWindow()
        {
            InitializeComponent();
            this.Closing += MainWindow_Closing;
            TabsControl.MouseDown += MainWindows_ButtonX;

            TopologyTabCancelBtn.IsEnabled = false;
            SaveToExcelBtn.IsEnabled = false;

            _topologies = new HashSet<string>();
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
            SaveToExcelBtn.IsEnabled = false;
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

                    _topologies.Clear();

                    var n = topologies.Count;
                    for (var i = 0; i < n;)
                    {
                        var topology = SetToString(topologies[i]);
                        _topologies.Add(topology);
                        TopologiesDataGrid.Items.Add(new TopologyItemData {Index = ++i, Topology = topology});
                    }
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
                SaveToExcelBtn.IsEnabled = true;
                TopologyTabCancelBtn.IsEnabled = false;
                TopologyTabProcessBar.Visibility = Visibility.Hidden;
            }
        }

        private async Task TopologiesToDataGridAsync(HashSet<string> set, CancellationToken ct,
            IProgress<double> progress)
        {
            // if > 6 will case overflow in the long type.
            if (set.Count > 5) throw new Exception("Set elements must be less than 6 elements.");

            progress.Report(0);

            _topologies.Clear();

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

                    var subset = new HashSet<HashSet<string>>();

                    // loop though every element in the set and determine with number 
                    // should present in the current subset.
                    var j = 0;
                    foreach (var e in powerSet)
                        // if the jth element (bit) in the ith subset (binary number of i) add it.
                        if (((1L << j++) & i) > 0)
                            subset.Add(e);

                    subset.Add(new HashSet<string>());
                    subset.Add(set);

                    if (IsTopology(subset, set))
                    {
                        var s = SetToString(subset);
                        _topologies.Add(s);
                        this.Dispatcher?.Invoke(delegate
                            {
                                TopologiesDataGrid.Items.Add(new TopologyItemData
                                    {Index = ++counter, Topology = s});
                            }
                        );
                    }
                }
            }, ct);

            progress.Report(0);
        }

        #region To Excel

        private async void SaveToExcelBtn_Click(object sender, RoutedEventArgs e)
        {

            _cancelToken = new CancellationTokenSource();

            GenerateTopologiesBtn.IsEnabled = false;
            SaveToExcelBtn.IsEnabled = false;
            TopologyTabCancelBtn.IsEnabled = true;

            _progressOperation = new Progress<double>(value => TopologyTabProcessBar.Value = value);

            TopologyTabProcessBar.Visibility = Visibility.Visible;

            TopologyTabStatusTxt.Text = "Exporting...";

            try
            {
                await Task.Run(() => ExportTopologiesDataGridToExcel(_cancelToken.Token, _progressOperation));
                TopologyTabStatusTxt.Text ="Operation Completed.";
            }
            catch (OperationCanceledException ex)
            {
                TopologyTabStatusTxt.Text ="Operation Cancelled | " + ex.Message;
            }
            catch (Exception ex)
            {
                TopologyTabStatusTxt.Text = "Error! Operation Cancelled | " + ex.Message;
            }
            finally
            {
                _cancelToken.Dispose();
                GenerateTopologiesBtn.IsEnabled = true;
                SaveToExcelBtn.IsEnabled = false;
                TopologyTabCancelBtn.IsEnabled = false;
                TopologyTabProcessBar.Visibility = Visibility.Hidden;
            }
        }

        public void ExportTopologiesDataGridToExcel(CancellationToken ct, IProgress<double> progress)
        {
            try
            {
                progress.Report(0);

                var sfd = new SaveFileDialog
                {
                    Filter = "Excel Documents (*.xlsx)|*.xlsx",
                    FileName = "Topologies.xlsx"
                };

                if (sfd.ShowDialog() != true) return;


                //Creates a blank workbook.
                using var p = new ExcelPackage();

                //A workbook must have at least on cell, so lets add one... 
                var sheet = p.Workbook.Worksheets.Add("Exported Topologies Sheet");

                // fill Header
                sheet.Cells[1, 1].Value = "N";
                sheet.Cells[1, 2].Value = "Topologies";

                // fill table rows.
                var x = 2;
                var n = _topologies.Count;
                foreach (var topology in _topologies)
                {
                    ct.ThrowIfCancellationRequested();
                    progress.Report((double) (x - 1) / n * 100);

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

                // using var range2 = sheet.Cells[1, 1];
                
                //Save the new workbook.
                p.SaveAs(new FileInfo(sfd.FileName));
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                MessageBox.Show($"Error: {e.Message} | {e.Source}", "Error!");
            }
            finally
            {
                progress.Report(0);
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

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _cancelToken.Cancel();
        }

        #endregion

        #region SubsetsPoints

        private void FindSubsetsPointsBtn_Click(object sender, RoutedEventArgs e)
        {
            SubsetPointsDataGrid.Items.Clear();

            var setBox = SubsetPointsTabSetTextBox.Text;
            if (setBox.Length == 0)
            {
                MessageBox.Show("Please fill the set field.", "Required Field!");
                return;
            }

            var tBox = SubsetPointsTabTopologyTextBox.Text;
            if (tBox.Length == 0)
            {
                MessageBox.Show("Please fill the topology field.", "Required Field!");
                return;
            }

            var set = StringToSet(setBox);
            var t = StringToSetOfSets(tBox);

            var powerSet = PowerSet(set);

            try
            {
                var i = 0;
                foreach (var subset in powerSet)
                {
                    SubsetPointsDataGrid.Items.Add(new SubsetPointsItemData
                    {
                        Index = ++i,
                        Subset = SetToString(subset),
                        Limit = SetToString(LimitPoints(set, subset, t)),
                        Closure = SetToString(ClosurePoints(set, subset, t)),
                        Interior = SetToString(InteriorPoints(set, subset, t)),
                        Exterior = SetToString(ExteriorPoints(set, subset, t)),
                        Boundary = SetToString(BoundaryPoints(set, subset, t))
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error!");
            }
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

        #region Close

        private static void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // See if the user really wants to shut down this window.
            const string msg = "Do you want to close without saving?";
            var result = MessageBox.Show(msg, "Are You Sure?", 
                MessageBoxButton.YesNo, MessageBoxImage.Warning);
	
            if (result == MessageBoxResult.No)
                // If user doesn't want to close, cancel closure.
                e.Cancel = true;
        }

        #endregion

        #region Handle Keys

        // Handle Mouse previous button and next button
        private void MainWindows_ButtonX(object sender, MouseEventArgs e)
        {
            if (e.XButton1 == MouseButtonState.Pressed)
            {
                TabsControl.SelectedItem = 
                    TabsControl.Items[(TabsControl.SelectedIndex + 1) % TabsControl.Items.Count];
            }

            if (e.XButton2 == MouseButtonState.Pressed)
            {
                var totalTabs = TabsControl.Items.Count;
                TabsControl.SelectedItem = 
                    TabsControl.Items[(TabsControl.SelectedIndex - 1 + totalTabs ) % totalTabs];
            }
        }

        #endregion
    }

    internal class TopologyItemData
    {
        public int Index { get; set; }
        public string Topology { get; set; }
    }

    internal class SubsetPointsItemData
    {
        public int Index { get; set; }
        public string Subset { get; set; }
        public string Limit { get; set; }
        public string Closure { get; set; }
        public string Interior { get; set; }
        public string Exterior { get; set; }
        public string Boundary { get; set; }
    }
}