using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SessionTime.SessionTimeCommon
{
    public enum SessionTrackingSupportedReasons
    {
        SessionLogon = 5,
        SessionLogoff = 6,
        SessionLock = 7,
        SessionUnlock = 8
    }
}