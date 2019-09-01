using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SessionTime.SessionTimeCommon
{
    public enum SessionTrackingEvents
    {
        OnStart = 1,
        OnSessionChange = 2,
        OnStop = 3
    }
}