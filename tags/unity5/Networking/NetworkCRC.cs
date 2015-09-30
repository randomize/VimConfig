namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.Networking.NetworkSystem;

    public class NetworkCRC
    {
        private bool m_ScriptCRCCheck;
        private Dictionary<string, int> m_Scripts = new Dictionary<string, int>();
        internal static NetworkCRC s_Singleton;

        private void Dump(CRCMessageEntry[] remoteScripts)
        {
            foreach (string str in this.m_Scripts.Keys)
            {
                Debug.Log(string.Concat(new object[] { "CRC Local Dump ", str, " : ", this.m_Scripts[str] }));
            }
            foreach (CRCMessageEntry entry in remoteScripts)
            {
                Debug.Log(string.Concat(new object[] { "CRC Remote Dump ", entry.name, " : ", entry.channel }));
            }
        }

        public static void RegisterBehaviour(string name, int channel)
        {
            singleton.m_Scripts[name] = channel;
        }

        public static void ReinitializeScriptCRCs(Assembly callingAssembly)
        {
            singleton.m_Scripts.Clear();
            foreach (System.Type type in callingAssembly.GetTypes())
            {
                if (type.BaseType == typeof(NetworkBehaviour))
                {
                    MethodInfo method = type.GetMethod(".cctor", BindingFlags.Static);
                    if (method != null)
                    {
                        method.Invoke(null, new object[0]);
                    }
                }
            }
        }

        internal static bool Validate(CRCMessageEntry[] scripts, int numChannels)
        {
            return singleton.ValidateInternal(scripts, numChannels);
        }

        private bool ValidateInternal(CRCMessageEntry[] remoteScripts, int numChannels)
        {
            if (this.m_Scripts.Count != remoteScripts.Length)
            {
                if (LogFilter.logError)
                {
                    Debug.LogError(string.Concat(new object[] { "HLAPI CRC channel count error local: ", this.m_Scripts.Count, " remote: ", remoteScripts.Length }));
                }
                this.Dump(remoteScripts);
                return false;
            }
            foreach (CRCMessageEntry entry in remoteScripts)
            {
                if (LogFilter.logDebug)
                {
                    Debug.Log(string.Concat(new object[] { "Script: ", entry.name, " Channel: ", entry.channel }));
                }
                if (this.m_Scripts.ContainsKey(entry.name))
                {
                    int num2 = this.m_Scripts[entry.name];
                    if (num2 != entry.channel)
                    {
                        if (LogFilter.logError)
                        {
                            Debug.LogError(string.Concat(new object[] { "HLAPI CRC Channel Mismatch. Script: ", entry.name, " LocalChannel: ", num2, " RemoteChannel: ", entry.channel }));
                        }
                        this.Dump(remoteScripts);
                        return false;
                    }
                }
                if (entry.channel >= numChannels)
                {
                    if (LogFilter.logError)
                    {
                        Debug.LogError(string.Concat(new object[] { "HLAPI CRC channel out of range! Script: ", entry.name, " Channel: ", entry.channel }));
                    }
                    this.Dump(remoteScripts);
                    return false;
                }
            }
            return true;
        }

        public static bool scriptCRCCheck
        {
            get
            {
                return singleton.m_ScriptCRCCheck;
            }
            set
            {
                singleton.m_ScriptCRCCheck = value;
            }
        }

        public Dictionary<string, int> scripts
        {
            get
            {
                return this.m_Scripts;
            }
        }

        internal static NetworkCRC singleton
        {
            get
            {
                if (s_Singleton == null)
                {
                    s_Singleton = new NetworkCRC();
                }
                return s_Singleton;
            }
        }
    }
}

