using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace APApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CancellationTokenSource cts = new CancellationTokenSource();
        private Parser.Parser parser = new Parser.Parser();
        public MainWindow()
        {
            InitializeComponent();    
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClearOutput();
            Microsoft.Win32.OpenFileDialog fileBrowserDlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> res = fileBrowserDlg.ShowDialog();
            if(res == true)
            {
                PathTextBox.Text = fileBrowserDlg.FileName;
            }
        }

        private async void ParseButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClearOutput();
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            Progress<ProgressReportModel> progress = new Progress<ProgressReportModel>();
            progress.ProgressChanged += ReportProgress;
            try
            {
                if (PathTextBox.Text != string.Empty)
                {
                    Dictionary<string, int> wordCountMap = await this.parser.BuildDictionaryAsync(PathTextBox.Text, progress, cts.Token);
                    wordCountGrid.ItemsSource = wordCountMap;
                }
            }
            catch (OperationCanceledException)
            {
                dashboardProgress.Value = 0;
                dashboardProgress.ToolTip = "Canceled";
            }
        }

        private void ReportProgress(object sender, ProgressReportModel e)
        {
            dashboardProgress.Value = e.PercentageComplete;
            dashboardProgress.ToolTip = e.Phase;
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            cts.Cancel();
            //PathTextBox.Text = string.Empty;
            cts.Dispose(); // Clean up old token source....
            cts = new CancellationTokenSource(); // "Reset" the cancellation token source...
        }

        private void ClearOutput()
        {
            wordCountGrid.ItemsSource = null;
        }

    }
}
