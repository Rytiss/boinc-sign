using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

namespace boinc_sign {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
        }

        private void browseCodeSignFile(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true) {
                tbCodeSignFile.Text = dlg.FileName;
            }
        }

        private void btnBrowseSignFilename_Click(object sender, RoutedEventArgs e) {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true) {
                tbSignFilename.Text = dlg.FileName;
            }
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e) {
            if (File.Exists(tbSignFilename.Text)) {
                if (
                    (tbSignFilename.Text.Substring(tbSignFilename.Text.Length - 4) != ".sig") ||
                    (MessageBox.Show("You should not need to code sign \".sig\" files. Are you sure you want to add this file?", "Confirm file", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                   ) {
                    lbSelectedFiles.Items.Add(tbSignFilename.Text);
                }
            } else {
                MessageBoxResult result = MessageBox.Show("File does not exist.", null, MessageBoxButton.OK, MessageBoxImage.Error);   
            }
        }

        private void btnRemoveSelected_Click(object sender, RoutedEventArgs e) {
            if (lbSelectedFiles.SelectedIndex >= 0) {
                lbSelectedFiles.Items.RemoveAt(lbSelectedFiles.SelectedIndex);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e) {
            if (lbSelectedFiles.Items.Count == 0) {
                MessageBox.Show("No files have been selected to be signed.", null, MessageBoxButton.OK, MessageBoxImage.Error);   
            }
            if (File.Exists(tbCodeSignFile.Text)) {
                for (int i = 0; i < lbSelectedFiles.Items.Count; i++) {
                    if ((cbOverwriteExisting.IsChecked == true) || (File.Exists(lbSelectedFiles.Items.GetItemAt(i) + ".sig") == false)) {
                        Process p = new Process();
                        p.StartInfo.WorkingDirectory = System.AppDomain.CurrentDomain.BaseDirectory;
                        p.StartInfo.FileName = "crypt_prog\\crypt_prog.exe";
                        p.StartInfo.Arguments = "-sign \"" + lbSelectedFiles.Items.GetItemAt(i) + "\" \"" + tbCodeSignFile.Text + "\"";
                        p.StartInfo.UseShellExecute = false;
                        p.StartInfo.CreateNoWindow = true;
                        p.StartInfo.RedirectStandardOutput = true;
                        p.Start();

                        string output = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();

                        System.IO.File.WriteAllText(lbSelectedFiles.Items.GetItemAt(i) + ".sig", output);
                    } else {
                        MessageBox.Show("Skipping existing file " + lbSelectedFiles.Items.GetItemAt(i) + ".sig", null, MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }

            MessageBox.Show("Done.", null, MessageBoxButton.OK, MessageBoxImage.Information); 
        }
    }
}
