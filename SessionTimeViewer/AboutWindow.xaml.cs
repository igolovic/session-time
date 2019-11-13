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
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                lblVersion.Content = "SessionTimeViewer " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
            catch (Exception ex)
            {
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
                Utility.Log(ex.ToString());
            }
        }

        private void LnkDocumentation_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);


                string documentationFilePath = Path.Combine(Path.GetDirectoryName(path), "SessionTimeDocumentation.txt");
                Process.Start(new ProcessStartInfo(documentationFilePath));
                e.Handled = true;
                int a = 0;
                int z = 3 / a;
            }
            catch (Exception ex)
            {
                Utility.Log(ex.ToString());
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DialogResult = true;
            }
            catch (Exception ex)
            {
                Utility.Log(ex.ToString());
            }
        }
    }
}