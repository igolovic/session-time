using SessionTime.SessionTimeCommon;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace SessionTime.SessionTimeViewer
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
            this.PreviewKeyDown += new KeyEventHandler(Key_PreviewKeyDown);
        }

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                string pathSessionTimeViewer = System.Reflection.Assembly.GetExecutingAssembly().Location;
                lblVersionSessionTimeViewer.Content = "SessionTimeViewer " + FileVersionInfo.GetVersionInfo(pathSessionTimeViewer).ProductVersion;

                string folderPathSessionTime = Path.GetDirectoryName(pathSessionTimeViewer);

                string pathSessionTimeMonitor = Path.Combine(folderPathSessionTime, "SessionTimeMonitor.exe");
                if (File.Exists(pathSessionTimeMonitor))
                    lblVersionSessionTimeMonitor.Content = "SessionTimeMonitor " + FileVersionInfo.GetVersionInfo(pathSessionTimeMonitor).ProductVersion;

                string pathSessionTimeCommon = Path.Combine(folderPathSessionTime, "SessionTimeCommon.dll");
                if (File.Exists(pathSessionTimeCommon))
                    lblVersionSessionTimeCommon.Content = "SessionTimeCommon " + FileVersionInfo.GetVersionInfo(pathSessionTimeCommon).ProductVersion;
            }
            catch (Exception ex)
            {
                SessionTimeViewerUtility.ShowException(ex);
                Utility.Log(ex.ToString());
            }
        }

        private void LnkRepository_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                SessionTimeViewerUtility.ShowException(ex);
                Utility.Log(ex.ToString());
            }
        }

        private void LnkDocumentation_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder codeBaseUri = new UriBuilder(codeBase);
                string codeBasePath = Uri.UnescapeDataString(codeBaseUri.Path);
                string documentationFilePath = Path.Combine(Path.GetDirectoryName(codeBasePath), "SessionTimeDocumentation.txt");

                if (File.Exists(documentationFilePath))
                {
                    Process.Start(new ProcessStartInfo(documentationFilePath));
                    e.Handled = true;
                }
                else
                    MessageBox.Show("Cannot find SessionTimeDocumentation.txt in application folder.");
            }
            catch (Exception ex)
            {
                SessionTimeViewerUtility.ShowException(ex);
                Utility.Log(ex.ToString());
            }
        }

        private void Key_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception ex)
            {
                SessionTimeViewerUtility.ShowException(ex);
                Utility.Log(ex.ToString());
            }
        }

        #endregion
    }
}