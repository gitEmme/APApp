using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            if (PathTextBox.Text != string.Empty)
            {
                Dictionary<string, int> wordCountMap = await this.parser.BuildDictionaryAsync(PathTextBox.Text);
                wordCountGrid.ItemsSource = wordCountMap;
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            this.ClearOutput();
            PathTextBox.Text = string.Empty;
        }

        private void ClearOutput()
        {
            wordCountGrid.ItemsSource = null;
        }

    }
}
