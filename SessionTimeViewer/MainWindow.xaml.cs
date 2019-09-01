using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using System.Diagnostics;
using SessionTime.SessionTimeCommon;

namespace SessionTime.SessionTimeViewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string dataFilePath = ConfigurationManager.AppSettings["DataFilePath"];
        private string logFilePath = ConfigurationManager.AppSettings["LogFilePath"];

        public MainWindow()
        {
            try
            {
                InitializeComponent();
                SessionManager.Initialize(dataFilePath, logFilePath);
                this.Loaded += MainWindow_Loaded;
            }
            catch (Exception ex)
            {
                Utility.Log(logFilePath, ex.ToString());
            }
        }

        #region Event handlers

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                Utility.Log(logFilePath, ex.ToString());
            }
        }

        #endregion

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                Utility.Log(logFilePath, ex.ToString());
            }
        }
        
        #region Private methods

        private void LoadData()
        {
            if (File.Exists(dataFilePath))
            {
                var sessions = SessionManager.GetSessions();
                if (sessions.Count() > 0)
                {
                    var users = (from session in sessions
                                 select session.Account).Distinct();

                    var selectedUser = cbUser.SelectedValue;
                    cbUser.ItemsSource = users.OrderBy(user => user);
                    if (selectedUser == null)
                    {
                        selectedUser = users.FirstOrDefault();
                        cbUser.SelectedValue = selectedUser;
                    }

                    sessions.RemoveAll(info => info.Account != selectedUser.ToString());
                    lvSessionMain.ItemsSource = sessions;
                }
            }
        }

        #endregion
    }
}