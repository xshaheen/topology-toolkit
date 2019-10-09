using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using static Topology.Infra.TopologyUtl;
using static Topology.CLI.Program;

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

        private void GenerateBtn_Click(object sender, RoutedEventArgs e)
        {
            var set = StringToSet(TSetTextBox.Text);
            
            var counter = 0;
            foreach (var topology in Topologies(set))
            {
                TopologiesGrid.Items.Add(new {Index = ++counter, Topology = SetToString(topology)});
            }
        }

        private void GenerateToExcelBtn_Click(object sender, RoutedEventArgs e)
        {
            var set = StringToSet(TSetTextBox.Text);

            TopologiesToExcel(set, SortCheckBox.IsEnabled);
        }

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
                "Interior Points" => InteriorPoints(subset, t),
                "Exterior Points" => ExteriorPoints(set, subset, t),
                "Boundary Points" => BoundaryPoints(set, subset, t),
                _ => null
            };

            PointsResult.Text = result == null 
                ? "Select the points set." : SetToString(result);
        }

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
    }
}