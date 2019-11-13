using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SessionTime.SessionTimeCommon;
using System.Xml.Linq;
using Cassia;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace SessionTime.SessionTimeCommon
{
    public static class SessionManager
    {
        #region Public methods

        /// <summary>
        /// Method to parse sessions from recorded session events
        /// </summary>
        /// <returns></returns>
        public static List<SessionInfo> GetSessions()
        {
            SessionTracking sessionTracking = Utility.DeserializeObjectFromFile<SessionTracking>(GlobalSettings.DataFilePath);

            // Sessions are defined by ServiceRunGuid and SessionId.
            // SessionId can repeat itself within ServiceRunGuid - such is the Windows implementation
            // (tested on Windows 7, 8, 10)
            var sessionTrackingParamsPerServiceRunAndSessionId =
                from o in sessionTracking.SessionTrackingParamsList
                group o by new { o.ServiceRunGuid, o.SessionId } into g
                select g;

            var onStartEventName = Enum.GetName(typeof(SessionTrackingEvents), SessionTrackingEvents.OnStart);
            var onStopEventName = Enum.GetName(typeof(SessionTrackingEvents), SessionTrackingEvents.OnStop);
            var sessionLogonReasonName = Enum.GetName(typeof(SessionTrackingSupportedReasons), SessionTrackingSupportedReasons.SessionLogon);
            var sessionLogoffReasonName = Enum.GetName(typeof(SessionTrackingSupportedReasons), SessionTrackingSupportedReasons.SessionLogoff);
            var sessionLockReasonName = Enum.GetName(typeof(SessionTrackingSupportedReasons), SessionTrackingSupportedReasons.SessionLock);

            List<SessionInfo> sessionInfos = new List<SessionInfo>();
            foreach (var sessionTrackingParams in sessionTrackingParamsPerServiceRunAndSessionId)
            {
                var orderedSessionTrackingParams = sessionTrackingParams.OrderBy(o => o.EventDT).ToList();
                //if (orderedSessionTrackingParams.Count() > 1)
                {
                    // Get beginning of the session
                    var firstSessionTrackingParams = orderedSessionTrackingParams.First();
                    if (firstSessionTrackingParams.Event == onStartEventName || firstSessionTrackingParams.Reason == sessionLogonReasonName)
                    {
                        var lastSessionTrackingParams = orderedSessionTrackingParams.Last();

                        // Get end of the session (if session is finished)
                        if (lastSessionTrackingParams.Event == onStopEventName || lastSessionTrackingParams.Reason == sessionLogoffReasonName)
                        {
                            var sessionInfo = new SessionInfo()
                            {
                                SessionId = firstSessionTrackingParams.SessionId,
                                SessionLogonDateTime = firstSessionTrackingParams.LogonDT,
                                SessionLogoffDateTime = lastSessionTrackingParams.EventDT,
                                Account = firstSessionTrackingParams.UserAccount
                            };

                            // Create list of unlocked/locked periods within session
                            sessionInfo.PeriodsInSessionByLockStatus = new List<PeriodInSessionByLockStatus>();

                            // Start time of first locked/unlocked period is equal to logon time of session
                            var firstPeriodInSessionByLockStatus = new PeriodInSessionByLockStatus()
                            {
                                StartDateTime = firstSessionTrackingParams.LogonDT,
                            };
                            sessionInfo.PeriodsInSessionByLockStatus.Add(firstPeriodInSessionByLockStatus);

                            // Process start/end times between start time of first period and end time of last period
                            for (int i = 1; i < orderedSessionTrackingParams.Count() - 1; i++)
                            {
                                sessionInfo.PeriodsInSessionByLockStatus[i - 1].EndDateTime = orderedSessionTrackingParams[i].EventDT;
                                sessionInfo.PeriodsInSessionByLockStatus.Add(
                                    new PeriodInSessionByLockStatus()
                                    {
                                        StartDateTime = orderedSessionTrackingParams[i].EventDT,
                                        IsLocked = (orderedSessionTrackingParams[i].Reason == sessionLockReasonName)
                                    });
                            }

                            // End time of last locked/unlocked period is equal to the time of last event of session
                            sessionInfo.PeriodsInSessionByLockStatus.Last().EndDateTime = lastSessionTrackingParams.EventDT;

                            sessionInfo.PeriodsInSessionByLockStatus.Reverse();
                            sessionInfos.Add(sessionInfo);
                        }
                        else
                        {
                            // Current unfinished session
                            var sessionInfo = new SessionInfo()
                            {
                                SessionId = firstSessionTrackingParams.SessionId,
                                SessionLogonDateTime = firstSessionTrackingParams.LogonDT,
                                SessionLogoffDateTime = null,
                                Account = firstSessionTrackingParams.UserAccount
                            };
                            sessionInfos.Add(sessionInfo);
                        }
                    }
                }
            }

            sessionInfos.Reverse();
            return sessionInfos;
        }

        public static void LogSessionData(Guid currentServiceRunGuid, int sessionId, SessionTrackingEvents sessionTrackingEvent, string reason, bool logAllExistingSessions = false)
        {
            using (ITerminalServer server = new TerminalServicesManager().GetLocalServer())
            {
                SessionTracking sessionTracking = null;
                if (!File.Exists(GlobalSettings.DataFilePath))
                {
                    sessionTracking = new SessionTracking();
                    sessionTracking.SessionTrackingParamsList = new List<SessionTrackingParams>();
                    File.Create(GlobalSettings.DataFilePath).Close();
                    Utility.SerializeObjectToFile<SessionTracking>(sessionTracking, GlobalSettings.DataFilePath);
                }

                server.Open();
                foreach (ITerminalServicesSession session in server.GetSessions().Where(o => o.LoginTime.HasValue))
                {
                    if (logAllExistingSessions || session.SessionId == sessionId)
                    {                        
                        SessionTrackingParams sessionTrackingParams = new SessionTrackingParams()
                            {
                               ServiceRunGuid = currentServiceRunGuid,
                               SessionId = session.SessionId,
                               LogonDT = session.LoginTime.HasValue ? session.LoginTime.Value : DateTime.MinValue,
                               EventDT = DateTime.Now,
                               UserAccount = (session.UserAccount != null && session.UserAccount.Value != null ? session.UserAccount.Value : String.Empty),
                               Event = Enum.GetName(typeof(SessionTrackingEvents), sessionTrackingEvent),
                               Reason = reason
                            };
                        string sessionTrackingParamsSerialized = Utility.SerializeObject<SessionTrackingParams>(sessionTrackingParams);

                        // Faster and not-so-clean way to write XML node to the end of the serialized List<SessionTrackingParams>,
                        // cleaner way would be to deserialize, add node and serialize whole list which might contain hundreds of records, but it would 
                        // possibly be much slower in case of large file.
                        string text = File.ReadAllText(GlobalSettings.DataFilePath);
                        if (text.Contains("</SessionTrackingParamsList>"))
                        {
                            text = text.Replace(
                                "</SessionTrackingParamsList>",
                                sessionTrackingParamsSerialized + Environment.NewLine + "</SessionTrackingParamsList>");
                        }
                        else
                        {
                            text = text.Replace(
                                "<SessionTrackingParamsList />",
                                "<SessionTrackingParamsList>" + Environment.NewLine + sessionTrackingParamsSerialized + Environment.NewLine + "</SessionTrackingParamsList>"); 
                        }
                        File.WriteAllText(GlobalSettings.DataFilePath, text);
                        
                        if (!logAllExistingSessions)
                            break;
                    }
                }
                server.Close();
            }
        }

        #endregion
    }
}