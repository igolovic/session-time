using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
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
        public MainWindow()
        {
            try
            {
                InitializeComponent();
                this.Loaded += MainWindow_Loaded;
            }
            catch (Exception ex)
            {
                SessionTimeViewerUtility.ShowException(ex);
                Utility.Log(ex.ToString());
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
                SessionTimeViewerUtility.ShowException(ex);
                Utility.Log(ex.ToString());
            }
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
                SessionTimeViewerUtility.ShowException(ex);
                Utility.Log(ex.ToString());
            }
        }
        private void BtnInfo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var aboutWindow = new AboutWindow();
                aboutWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                SessionTimeViewerUtility.ShowException(ex);
                Utility.Log(ex.ToString());
            }
        }

        #endregion

        #region Private methods

        private void LoadData()
        {
            if (File.Exists(GlobalSettings.DataFilePath))
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