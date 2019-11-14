using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SessionTime.SessionTimeViewer
{
    public static class SessionTimeViewerUtility
    {
        public static void ShowException(Exception ex)
        {
            MessageBox.Show("Exception message: " + ex.Message, "Exception", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
