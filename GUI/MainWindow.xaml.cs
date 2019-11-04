using Infra;
using Infra.Models;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
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
using static Infra.ParseUtl;
using static Infra.SetUtl;
using static Infra.TopologyUtl;

namespace GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly HashSet<string> _topologies;

        private CancellationTokenSource _cancelToken;

        private Progress<double> _progress;

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

            if (set.Count == 0)
            {
                MessageBox.Show("Please Enter the set elements!",
                    "Required Fields");
                return;
            }

            var sort = TopologyTabSortCheckBox.IsChecked == true;

            if (sort && set.Count > 4)
            {
                MessageBox.Show("I can not sort topologies defined on a set has element > 4 it cost a lot of time!", "Error");
                return;
            }

            GenerateTopologiesBtn.IsEnabled = false;
            SaveToExcelBtn.IsEnabled = false;
            TopologyTabCancelBtn.IsEnabled = true;

            TopologiesDataGrid.Items.Clear();
            TopologyTabStatusTxt.Text = "Generating...";

            _cancelToken = new CancellationTokenSource();
            _progress = new Progress<double>(
                value => TopologyTabProcessBar.Value = value);
            TopologyTabProcessBar.Visibility = Visibility.Visible;

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
                        TopologiesDataGrid.Items.Add(new TopologyModel
                            { Index = ++i, Topology = topology });
                    }
                }
                else
                {
                    await Task.Run(() => TopologiesToDataGrid(
                        set, _cancelToken.Token, _progress));
                }

                TopologyTabStatusTxt.Text = "Operation Completed.";
            }
            catch (OperationCanceledException ex)
            {
                TopologyTabStatusTxt.Text = "Operation Cancelled | " + ex.Message;
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

        private async void SaveToExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            GenerateTopologiesBtn.IsEnabled = false;
            SaveToExcelBtn.IsEnabled = false;
            TopologyTabCancelBtn.IsEnabled = true;

            // Get path
            var sfd = new SaveFileDialog
            {
                Filter = "Excel Documents (*.xlsx)|*.xlsx",
                FileName = "Topologies.xlsx"
            };
        
            if (sfd.ShowDialog() != true) return;

            _cancelToken = new CancellationTokenSource();
            _progress = new Progress<double>(
                value => TopologyTabProcessBar.Value = value);
            TopologyTabProcessBar.Visibility = Visibility.Visible;
            TopologyTabStatusTxt.Text = "Exporting...";

            try
            {
                await Task.Run(() => ExportTopologiesToExcel(
                    _topologies, sfd.FileName, _cancelToken.Token, _progress));
                
                TopologyTabStatusTxt.Text = "Operation Completed.";
            }
            catch (OperationCanceledException ex)
            {
                TopologyTabStatusTxt.Text = "Operation Cancelled | " + ex.Message;
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

        private void CancelBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _cancelToken.Cancel();
        }

        #region HelperMethods

        private void TopologiesToDataGrid(
            HashSet<string> set,
            CancellationToken ct,
            IProgress<double> progress)
        {
            _topologies.Clear();
            var counter = 0;

            foreach (var t in Topologies(set, progress, ct))
            {
                var s = SetToString(t);
                _topologies.Add(s);
                this.Dispatcher?.Invoke(delegate
                    {
                        TopologiesDataGrid.Items.Add(new TopologyModel
                            { Index = ++counter, Topology = s });
                    }
                );
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

        #endregion

        #region SubsetsPoints

        private void FindSubsetsPointsBtn_Click(object sender, RoutedEventArgs e)
        {
            SubsetPointsDataGrid.Items.Clear();

            var setBox = SubsetPointsTabSetTextBox.Text;
            if (string.IsNullOrEmpty(setBox))
            {
                MessageBox.Show("Please fill the set field.", "Required Field!");
                return;
            }

            var tBox = SubsetPointsTabTopologyTextBox.Text;
            if (string.IsNullOrEmpty(tBox))
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
                    SubsetPointsDataGrid.Items.Add(new SubsetPointsCategories
                    {
                        Index = ++i,
                        Subset = SetToString(subset),
                        Limit = SetToString(LimitPoints(set, subset, t)),
                        Closure = SetToString(ClosurePoints(set, subset, t)),
                        Interior = SetToString(InteriorPoints(set, subset, t)),
                        Exterior = SetToString(ExteriorPoints(set, subset, t)),
                        Boundary = SetToString(BoundaryPoints(set, subset, t)),
                        Accuracy = Accuracy(set, subset, t),
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
            if (string.IsNullOrEmpty(setBox))
            {
                MessageBox.Show("Please fill the set field.", "Required Field!");
                return;
            }

            var subsetBox = PointsTabSubsetTextBox.Text;
            if (string.IsNullOrEmpty(subsetBox))
            {
                MessageBox.Show("Please fill the subset field.", "Required Field!");
                return;
            }

            var tBox = PointsTabTopologyTextBox.Text;
            if (string.IsNullOrEmpty(tBox))
            {
                MessageBox.Show("Please fill the topology field.", "Required Field!");
                return;
            }

            var set = StringToSet(setBox);
            var subset = StringToSet(subsetBox);
            var t = StringToSetOfSets(tBox);

            var func = PointsTabPointsComboBox.Text;

            if (string.IsNullOrEmpty(func))
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

        #region Neighbourhood

        private void GenerateNeighbourhoodBtn_Click(object sender, RoutedEventArgs e)
        {
            var setBox = NeighbourhoodTabSetTextBox.Text;
            if (string.IsNullOrEmpty(setBox))
            {
                MessageBox.Show("Please fill the set field.", "Required Field!");
                return;
            }

            var pointBox = NeighbourhoodTabPointTextBox.Text;
            if (string.IsNullOrEmpty(pointBox))
            {
                MessageBox.Show("Please fill the point field.", "Required Field!");
                return;
            }

            var topologyBox = NeighbourhoodTabTopologyTextBox.Text;
            if (string.IsNullOrEmpty(topologyBox))
            {
                MessageBox.Show("Please fill the topology field.", "Required Field!");
                return;
            }

            NeighbourhoodDataGrid.Items.Clear();
            var counter = 0;

            try
            {
                foreach (var s in NeighbourhoodSystem(StringToSet(setBox),
                    StringToSetOfSets(topologyBox), pointBox)) 
                    NeighbourhoodDataGrid.Items.Add(new NeighbourhoodModel 
                        { Index = ++counter, Neighbourhood = SetToString(s) });
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
            if (string.IsNullOrEmpty(setBox))
            {
                MessageBox.Show("Please fill the set field.", "Required Field!");
                return;
            }

            PowerSetDataGrid.Items.Clear();
            var counter = 0;

            try
            {
                foreach (var subset in PowerSet(StringToSet(setBox)))
                    PowerSetDataGrid.Items.Add(new SetModel 
                        { Index = ++counter, Set = SetToString(subset) });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error!");
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
                    TabsControl.Items[(TabsControl.SelectedIndex - 1 + totalTabs) % totalTabs];
            }
        }

        #endregion
    }


}