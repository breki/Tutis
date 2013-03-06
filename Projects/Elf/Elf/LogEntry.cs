using System;

namespace Elf
{
    public class LogEntry
    {
        public DateTime Date
        {
            get { return date; }
            set { date = value.Date; }
        }

        public TimeSpan Time
        {
            get { return time; }
            set { time = value; }
        }

        public DateTime DateTime
        {
            get { return date.Add(time); }
        }

        public string ClientIp
        {
            get { return clientIP; }
            set { clientIP = value; }
        }

        public string Method
        {
            get { return method; }
            set { method = value; }
        }

        public string UriStem
        {
            get { return uriStem; }
            set { uriStem = value; }
        }

        public string UriQuery
        {
            get { return uriQuery; }
            set { uriQuery = value; }
        }

        public string ServerStatus
        {
            get { return serverStatus; }
            set { serverStatus = value; }
        }

        public string UserAgent
        {
            get { return userAgent; }
            set { userAgent = value; }
        }

        public string Referrer
        {
            get { return referrer; }
            set { referrer = value; }
        }

        private DateTime date;
        private TimeSpan time;
        private string clientIP;
        private string method;
        private string uriStem;
        private string uriQuery;
        private string serverStatus;
        private string userAgent;
        private string referrer;
    }
}