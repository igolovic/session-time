using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SessionTime.SessionTimeCommon
{
    public class PeriodInSessionByLockStatus
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public bool IsLocked { get; set; }

        public TimeSpan TimeBetweenStartAndEnd
        {
            get
            {
                return EndDateTime - StartDateTime;
            }
        }
    }
}