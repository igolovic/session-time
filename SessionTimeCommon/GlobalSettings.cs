using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SessionTime.SessionTimeCommon;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Xml.Serialization;
using System.Configuration;

namespace SessionTime.SessionTimeCommon
{
    public static class GlobalSettings
    {
        public static string DataFilePath = ConfigurationManager.AppSettings["DataFilePath"];
    }
}