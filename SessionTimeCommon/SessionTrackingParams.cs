using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SessionTime.SessionTimeCommon
{
    public class SessionTrackingParams
    {
        [XmlAttribute]
        public Guid ServiceRunGuid { get; set; }
        [XmlAttribute]
        public int SessionId { get; set; }
        [XmlAttribute]
        public DateTime LogonDT { get; set; }
        [XmlAttribute]
        public DateTime EventDT { get; set; }
        [XmlAttribute]
        public string UserAccount { get; set; }
        [XmlAttribute]
        public string Event { get; set; }
        [XmlAttribute]
        public string Reason { get; set; }
    }
}