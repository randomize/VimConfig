namespace UnityEngine.Cloud.Service
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal sealed class CloudServiceConfig
    {
        private int m_MaxNumberOfEventInGroup;
        private string m_SessionHeaderName;
        private string m_EventsHeaderName;
        private string m_EventsEndPoint;
        private int[] m_NetworkFailureRetryTimeoutInSec;
        public int maxNumberOfEventInGroup
        {
            get
            {
                return this.m_MaxNumberOfEventInGroup;
            }
            set
            {
                this.m_MaxNumberOfEventInGroup = value;
            }
        }
        public string sessionHeaderName
        {
            get
            {
                return this.m_SessionHeaderName;
            }
            set
            {
                this.m_SessionHeaderName = value;
            }
        }
        public string eventsHeaderName
        {
            get
            {
                return this.m_EventsHeaderName;
            }
            set
            {
                this.m_EventsHeaderName = value;
            }
        }
        public string eventsEndPoint
        {
            get
            {
                return this.m_EventsEndPoint;
            }
            set
            {
                this.m_EventsEndPoint = value;
            }
        }
        public int[] networkFailureRetryTimeoutInSec
        {
            get
            {
                return this.m_NetworkFailureRetryTimeoutInSec;
            }
            set
            {
                this.m_NetworkFailureRetryTimeoutInSec = value;
            }
        }
    }
}

