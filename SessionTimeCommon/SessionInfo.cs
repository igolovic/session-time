using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SessionTime.SessionTimeCommon
{
    public class SessionInfo
    {
        public int SessionId { get; set; }
        public DateTime SessionLogonDateTime { get; set; }
        public DateTime? SessionLogoffDateTime { get; set; }
        public string Account { get; set; }
        public List<PeriodInSessionByLockStatus> PeriodsInSessionByLockStatus { get; set; }
        public DateTime SessionLogonDate
        {
            get
            {
                return SessionLogonDateTime.Date;
            }
        }
        public DateTime? SessionLogoffDate
        {
            get
            {
                return SessionLogoffDateTime?.Date;
            }
        }
        public TimeSpan? TimeBetweenSessionLogonAndLogoff
        {
            get
            {
                if (SessionLogoffDateTime != null)
                    return SessionLogoffDateTime - SessionLogonDateTime;

                return null;
            }
        }
    }
}