using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Xml.Linq;
using System.Configuration;
using System.IO;
using System.Management;
using SessionTime.SessionTimeCommon;

namespace SessionTime.SessionTimeMonitor
{
    public partial class SessionTimeMonitor : ServiceBase
    {
        private string dataFilePath = ConfigurationManager.AppSettings["DataFilePath"];
        private string logFilePath = ConfigurationManager.AppSettings["LogFilePath"];
        private Guid currentServiceRunGuid = Guid.NewGuid();

        public SessionTimeMonitor()
        {
            try
            {
                InitializeComponent();
                SessionManager.Initialize(dataFilePath, logFilePath);
                CanPauseAndContinue = false;
                CanHandleSessionChangeEvent = true;
                ServiceName = "SessionTimeMonitor";
            }
            catch (Exception ex)
            {
                Utility.Log(logFilePath, ex.ToString());
            }
        }

        #region Event handlers

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            try
            {
                if (changeDescription.Reason == SessionChangeReason.SessionLogon
                    || changeDescription.Reason == SessionChangeReason.SessionLogoff
                    || changeDescription.Reason == SessionChangeReason.SessionLock
                    || changeDescription.Reason == SessionChangeReason.SessionUnlock
                    )
                {
                    SessionManager.LogSessionData(
                        currentServiceRunGuid, 
                        changeDescription.SessionId, 
                        SessionTrackingEvents.OnSessionChange,
                        changeDescription.Reason.ToString());
                }
            }
            catch (Exception ex)
            {
                Utility.Log(logFilePath, ex.ToString());
            }
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                SessionManager.LogSessionData(
                    currentServiceRunGuid, 
                    -1, 
                    SessionTrackingEvents.OnStart, 
                    String.Empty, 
                    true);

            }
            catch (Exception ex)
            {
                Utility.Log(logFilePath, ex.ToString());
            }
        }

        protected override void OnStop()
        {
            try
            {
                SessionManager.LogSessionData(
                    currentServiceRunGuid,
                    -1,
                    SessionTrackingEvents.OnStop,
                    String.Empty,
                    true);

            }
            catch (Exception ex)
            {
                Utility.Log(logFilePath, ex.ToString());
            }
        }

        #endregion
    }
}