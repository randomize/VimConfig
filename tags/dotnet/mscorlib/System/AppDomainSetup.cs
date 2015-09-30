namespace System
{
    using System.Deployment.Internal.Isolation.Manifest;
    using System.IO;
    using System.Runtime.CompilerServices;
    using System.Runtime.Hosting;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Security.Util;
    using System.Text;

    [Serializable, ComVisible(true), ClassInterface(ClassInterfaceType.None)]
    public sealed class AppDomainSetup : IAppDomainSetup
    {
        [OptionalField(VersionAdded=2)]
        private System.Runtime.Hosting.ActivationArguments _ActivationArguments;
        private string _AppBase;
        [OptionalField(VersionAdded=2)]
        private System.AppDomainInitializer _AppDomainInitializer;
        [OptionalField(VersionAdded=2)]
        private string[] _AppDomainInitializerArguments;
        [OptionalField(VersionAdded=2)]
        internal string _ApplicationTrust;
        [OptionalField(VersionAdded=2)]
        private byte[] _ConfigurationBytes;
        [OptionalField(VersionAdded=3)]
        private bool _DisableInterfaceCache;
        private string[] _Entries;
        private System.LoaderOptimization _LoaderOptimization;

        public AppDomainSetup()
        {
            this._LoaderOptimization = System.LoaderOptimization.NotSpecified;
        }

        public AppDomainSetup(ActivationContext activationContext) : this(new System.Runtime.Hosting.ActivationArguments(activationContext))
        {
        }

        public AppDomainSetup(System.Runtime.Hosting.ActivationArguments activationArguments)
        {
            if (activationArguments == null)
            {
                throw new ArgumentNullException("activationArguments");
            }
            this._LoaderOptimization = System.LoaderOptimization.NotSpecified;
            this.ActivationArguments = activationArguments;
            string entryPointFullPath = CmsUtils.GetEntryPointFullPath(activationArguments);
            if (!string.IsNullOrEmpty(entryPointFullPath))
            {
                this.SetupDefaultApplicationBase(entryPointFullPath);
            }
            else
            {
                this.ApplicationBase = activationArguments.ActivationContext.ApplicationDirectory;
            }
        }

        internal AppDomainSetup(AppDomainSetup copy, bool copyDomainBoundData)
        {
            string[] strArray = this.Value;
            if (copy != null)
            {
                string[] strArray2 = copy.Value;
                int length = this._Entries.Length;
                int num2 = strArray2.Length;
                int num3 = (num2 < length) ? num2 : length;
                for (int i = 0; i < num3; i++)
                {
                    strArray[i] = strArray2[i];
                }
                if (num3 < length)
                {
                    for (int j = num3; j < length; j++)
                    {
                        strArray[j] = null;
                    }
                }
                this._LoaderOptimization = copy._LoaderOptimization;
                this._AppDomainInitializerArguments = copy.AppDomainInitializerArguments;
                this._ActivationArguments = copy.ActivationArguments;
                this._ApplicationTrust = copy._ApplicationTrust;
                if (copyDomainBoundData)
                {
                    this._AppDomainInitializer = copy.AppDomainInitializer;
                }
                else
                {
                    this._AppDomainInitializer = null;
                }
                this._ConfigurationBytes = copy.GetConfigurationBytes();
                this._DisableInterfaceCache = copy._DisableInterfaceCache;
            }
            else
            {
                this._LoaderOptimization = System.LoaderOptimization.NotSpecified;
            }
        }

        private string BuildShadowCopyDirectories()
        {
            string str = this.Value[5];
            if (str == null)
            {
                return null;
            }
            StringBuilder builder = new StringBuilder();
            string str2 = this.Value[0];
            if (str2 != null)
            {
                char[] separator = new char[] { ';' };
                string[] strArray = str.Split(separator);
                int length = strArray.Length;
                bool flag = (str2[str2.Length - 1] != '/') && (str2[str2.Length - 1] != '\\');
                if (length == 0)
                {
                    builder.Append(str2);
                    if (flag)
                    {
                        builder.Append('\\');
                    }
                    builder.Append(str);
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        builder.Append(str2);
                        if (flag)
                        {
                            builder.Append('\\');
                        }
                        builder.Append(strArray[i]);
                        if (i < (length - 1))
                        {
                            builder.Append(';');
                        }
                    }
                }
            }
            return builder.ToString();
        }

        public byte[] GetConfigurationBytes()
        {
            if (this._ConfigurationBytes == null)
            {
                return null;
            }
            return (byte[]) this._ConfigurationBytes.Clone();
        }

        private bool IsFilePath(string path)
        {
            return ((path[1] == ':') || ((path[0] == '\\') && (path[1] == '\\')));
        }

        internal static int Locate(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                switch (s[0])
                {
                    case 'P':
                        if (!(s == "PRIVATE_BINPATH"))
                        {
                            break;
                        }
                        return 5;

                    case 'S':
                        if (s == "SHADOW_COPY_DIRS")
                        {
                            return 7;
                        }
                        break;

                    case 'A':
                        if (!(s == "APP_CONFIG_FILE"))
                        {
                            if (s == "APP_NAME")
                            {
                                return 4;
                            }
                            if (s == "APPBASE")
                            {
                                return 0;
                            }
                            if (!(s == "APP_CONFIG_BLOB"))
                            {
                                break;
                            }
                            return 15;
                        }
                        return 1;

                    case 'B':
                        if (!(s == "BINPATH_PROBE_ONLY"))
                        {
                            break;
                        }
                        return 6;

                    case 'C':
                        if (!(s == "CACHE_BASE"))
                        {
                            if (!(s == "CODE_DOWNLOAD_DISABLED"))
                            {
                                break;
                            }
                            return 12;
                        }
                        return 9;

                    case 'D':
                        if (!(s == "DEV_PATH"))
                        {
                            if (s == "DYNAMIC_BASE")
                            {
                                return 2;
                            }
                            if (s == "DISALLOW_APP")
                            {
                                return 11;
                            }
                            if (s == "DISALLOW_APP_REDIRECTS")
                            {
                                return 13;
                            }
                            if (s == "DISALLOW_APP_BASE_PROBING")
                            {
                                return 14;
                            }
                            break;
                        }
                        return 3;

                    case 'F':
                        if (!(s == "FORCE_CACHE_INSTALL"))
                        {
                            break;
                        }
                        return 8;

                    case 'L':
                        if (s == "LICENSE_FILE")
                        {
                            return 10;
                        }
                        break;
                }
            }
            return -1;
        }

        private string NormalizePath(string path, bool useAppBase)
        {
            bool flag2;
            if (path == null)
            {
                return null;
            }
            if (!useAppBase)
            {
                path = URLString.PreProcessForExtendedPathRemoval(path, false);
            }
            int length = path.Length;
            if (length == 0)
            {
                return null;
            }
            bool flag = false;
            if ((length > 7) && (string.Compare(path, 0, "file:", 0, 5, StringComparison.OrdinalIgnoreCase) == 0))
            {
                int num2;
                if (path[6] == '\\')
                {
                    if ((path[7] == '\\') || (path[7] == '/'))
                    {
                        if ((length > 8) && ((path[8] == '\\') || (path[8] == '/')))
                        {
                            throw new ArgumentException(Environment.GetResourceString("Argument_InvalidPathChars"));
                        }
                        num2 = 8;
                    }
                    else
                    {
                        num2 = 5;
                        flag = true;
                    }
                }
                else if (path[7] == '/')
                {
                    num2 = 8;
                }
                else
                {
                    if (((length > 8) && (path[7] == '\\')) && (path[8] == '\\'))
                    {
                        num2 = 7;
                    }
                    else
                    {
                        num2 = 5;
                        StringBuilder builder = new StringBuilder(length);
                        for (int i = 0; i < length; i++)
                        {
                            char ch = path[i];
                            if (ch == '/')
                            {
                                builder.Append('\\');
                            }
                            else
                            {
                                builder.Append(ch);
                            }
                        }
                        path = builder.ToString();
                    }
                    flag = true;
                }
                path = path.Substring(num2);
                length -= num2;
            }
            if (flag || (((length > 1) && ((path[0] == '/') || (path[0] == '\\'))) && ((path[1] == '/') || (path[1] == '\\'))))
            {
                flag2 = false;
            }
            else
            {
                int num4 = path.IndexOf(':') + 1;
                if ((((num4 != 0) && (length > (num4 + 1))) && ((path[num4] == '/') || (path[num4] == '\\'))) && ((path[num4 + 1] == '/') || (path[num4 + 1] == '\\')))
                {
                    flag2 = false;
                }
                else
                {
                    flag2 = true;
                }
            }
            if (flag2)
            {
                if (useAppBase && ((length == 1) || (path[1] != ':')))
                {
                    string str = this.Value[0];
                    if ((str == null) || (str.Length == 0))
                    {
                        throw new MemberAccessException(Environment.GetResourceString("AppDomain_AppBaseNotSet"));
                    }
                    StringBuilder builder2 = new StringBuilder();
                    bool flag3 = false;
                    if ((path[0] == '/') || (path[0] == '\\'))
                    {
                        string pathRoot = Path.GetPathRoot(str);
                        if (pathRoot.Length == 0)
                        {
                            int index = str.IndexOf(":/", StringComparison.Ordinal);
                            if (index == -1)
                            {
                                index = str.IndexOf(@":\", StringComparison.Ordinal);
                            }
                            int num6 = str.Length;
                            index++;
                            while ((index < num6) && ((str[index] == '/') || (str[index] == '\\')))
                            {
                                index++;
                            }
                            while (((index < num6) && (str[index] != '/')) && (str[index] != '\\'))
                            {
                                index++;
                            }
                            pathRoot = str.Substring(0, index);
                        }
                        builder2.Append(pathRoot);
                        flag3 = true;
                    }
                    else
                    {
                        builder2.Append(str);
                    }
                    int startIndex = builder2.Length - 1;
                    if ((builder2[startIndex] != '/') && (builder2[startIndex] != '\\'))
                    {
                        if (!flag3)
                        {
                            if (str.IndexOf(":/", StringComparison.Ordinal) == -1)
                            {
                                builder2.Append('\\');
                            }
                            else
                            {
                                builder2.Append('/');
                            }
                        }
                    }
                    else if (flag3)
                    {
                        builder2.Remove(startIndex, 1);
                    }
                    builder2.Append(path);
                    path = builder2.ToString();
                    return path;
                }
                path = Path.GetFullPathInternal(path);
            }
            return path;
        }

        public void SetConfigurationBytes(byte[] value)
        {
            this._ConfigurationBytes = value;
        }

        internal void SetupDefaultApplicationBase(string imageLocation)
        {
            StringBuilder builder = null;
            string str2;
            char[] anyOf = new char[] { '\\', '/' };
            int num = imageLocation.LastIndexOfAny(anyOf);
            string str = null;
            if (num == -1)
            {
                builder = new StringBuilder(imageLocation);
            }
            else
            {
                str = imageLocation.Substring(0, num + 1);
                builder = new StringBuilder(imageLocation.Substring(num + 1));
            }
            if (num == -1)
            {
                str2 = imageLocation;
            }
            else
            {
                str2 = imageLocation.Substring(num + 1);
            }
            builder.Append(ConfigurationExtension);
            if (builder != null)
            {
                this.ConfigurationFile = builder.ToString();
            }
            if (str != null)
            {
                this.ApplicationBase = str;
            }
            if (str2 != null)
            {
                this.ApplicationName = str2;
            }
        }

        internal void SetupFusionContext(IntPtr fusionContext)
        {
            string str = this.Value[0];
            if (str != null)
            {
                UpdateContextProperty(fusionContext, ApplicationBaseKey, str);
            }
            string str2 = this.Value[5];
            if (str2 != null)
            {
                UpdateContextProperty(fusionContext, PrivateBinPathKey, str2);
            }
            string str3 = this.Value[3];
            if (str3 != null)
            {
                UpdateContextProperty(fusionContext, DeveloperPathKey, str3);
            }
            if (this.DisallowPublisherPolicy)
            {
                UpdateContextProperty(fusionContext, DisallowPublisherPolicyKey, "true");
            }
            if (this.DisallowCodeDownload)
            {
                UpdateContextProperty(fusionContext, DisallowCodeDownloadKey, "true");
            }
            if (this.DisallowBindingRedirects)
            {
                UpdateContextProperty(fusionContext, DisallowBindingRedirectsKey, "true");
            }
            if (this.DisallowApplicationBaseProbing)
            {
                UpdateContextProperty(fusionContext, DisallowAppBaseProbingKey, "true");
            }
            if (this.ShadowCopyFiles != null)
            {
                UpdateContextProperty(fusionContext, ShadowCopyFilesKey, this.ShadowCopyFiles);
                if (this.Value[7] == null)
                {
                    this.ShadowCopyDirectories = this.BuildShadowCopyDirectories();
                }
                string str4 = this.Value[7];
                if (str4 != null)
                {
                    UpdateContextProperty(fusionContext, ShadowCopyDirectoriesKey, str4);
                }
            }
            string str5 = this.Value[9];
            if (str5 != null)
            {
                UpdateContextProperty(fusionContext, CachePathKey, str5);
            }
            if (this.PrivateBinPathProbe != null)
            {
                UpdateContextProperty(fusionContext, PrivateBinPathProbeKey, this.PrivateBinPathProbe);
            }
            string str6 = this.Value[1];
            if (str6 != null)
            {
                UpdateContextProperty(fusionContext, ConfigurationFileKey, str6);
            }
            if (this._ConfigurationBytes != null)
            {
                UpdateContextProperty(fusionContext, ConfigurationBytesKey, this._ConfigurationBytes);
            }
            if (this.ApplicationName != null)
            {
                UpdateContextProperty(fusionContext, ApplicationNameKey, this.ApplicationName);
            }
            string str7 = this.Value[2];
            if (str7 != null)
            {
                UpdateContextProperty(fusionContext, DynamicBaseKey, str7);
            }
            StringBuilder builder = new StringBuilder();
            builder.Append(RuntimeEnvironment.GetRuntimeDirectoryImpl());
            builder.Append(RuntimeConfigurationFile);
            UpdateContextProperty(fusionContext, MachineConfigKey, builder.ToString());
            string hostBindingFile = RuntimeEnvironment.GetHostBindingFile();
            if (hostBindingFile != null)
            {
                UpdateContextProperty(fusionContext, HostBindingKey, hostBindingFile);
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void UpdateContextProperty(IntPtr fusionContext, string key, object value);
        private string VerifyDir(string dir, bool normalize)
        {
            if (dir != null)
            {
                if (dir.Length == 0)
                {
                    dir = null;
                    return dir;
                }
                if (normalize)
                {
                    dir = this.NormalizePath(dir, true);
                }
                if (this.IsFilePath(dir))
                {
                    new FileIOPermission(FileIOPermissionAccess.PathDiscovery, dir).Demand();
                }
            }
            return dir;
        }

        private void VerifyDirList(string dirs)
        {
            if (dirs != null)
            {
                string[] strArray = dirs.Split(new char[] { ';' });
                int length = strArray.Length;
                for (int i = 0; i < length; i++)
                {
                    this.VerifyDir(strArray[i], true);
                }
            }
        }

        [XmlIgnoreMember]
        public System.Runtime.Hosting.ActivationArguments ActivationArguments
        {
            get
            {
                return this._ActivationArguments;
            }
            set
            {
                lock (this)
                {
                    if (((value != null) && (this._ApplicationTrust != null)) && !CmsUtils.CompareIdentities(this.ApplicationTrust.ApplicationIdentity, value.ApplicationIdentity, ApplicationVersionMatch.MatchExactVersion))
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ActivationArgsAppTrustMismatch"));
                    }
                    this._ActivationArguments = value;
                }
            }
        }

        [XmlIgnoreMember]
        public System.AppDomainInitializer AppDomainInitializer
        {
            get
            {
                return this._AppDomainInitializer;
            }
            set
            {
                this._AppDomainInitializer = value;
            }
        }

        public string[] AppDomainInitializerArguments
        {
            get
            {
                return this._AppDomainInitializerArguments;
            }
            set
            {
                this._AppDomainInitializerArguments = value;
            }
        }

        public string ApplicationBase
        {
            get
            {
                return this.VerifyDir(this.Value[0], false);
            }
            set
            {
                this.Value[0] = this.NormalizePath(value, false);
            }
        }

        internal static string ApplicationBaseKey
        {
            get
            {
                return "APPBASE";
            }
        }

        public string ApplicationName
        {
            get
            {
                return this.Value[4];
            }
            set
            {
                this.Value[4] = value;
            }
        }

        internal static string ApplicationNameKey
        {
            get
            {
                return "APP_NAME";
            }
        }

        [XmlIgnoreMember]
        public System.Security.Policy.ApplicationTrust ApplicationTrust
        {
            get
            {
                if (this._ApplicationTrust == null)
                {
                    return null;
                }
                SecurityElement element = SecurityElement.FromString(this._ApplicationTrust);
                System.Security.Policy.ApplicationTrust trust = new System.Security.Policy.ApplicationTrust();
                trust.FromXml(element);
                return trust;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("value", Environment.GetResourceString("ArgumentNull_ApplicationTrust"));
                }
                lock (this)
                {
                    if ((this._ActivationArguments != null) && !CmsUtils.CompareIdentities(value.ApplicationIdentity, this._ActivationArguments.ApplicationIdentity, ApplicationVersionMatch.MatchExactVersion))
                    {
                        throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_ActivationArgsAppTrustMismatch"));
                    }
                    this._ApplicationTrust = value.ToXml().ToString();
                }
            }
        }

        public string CachePath
        {
            get
            {
                return this.VerifyDir(this.Value[9], false);
            }
            set
            {
                this.Value[9] = this.NormalizePath(value, false);
            }
        }

        internal static string CachePathKey
        {
            get
            {
                return "CACHE_BASE";
            }
        }

        private static string ConfigurationBytesKey
        {
            get
            {
                return "APP_CONFIG_BLOB";
            }
        }

        internal static string ConfigurationExtension
        {
            get
            {
                return ".config";
            }
        }

        public string ConfigurationFile
        {
            get
            {
                return this.VerifyDir(this.Value[1], true);
            }
            set
            {
                this.Value[1] = value;
            }
        }

        internal string ConfigurationFileInternal
        {
            get
            {
                return this.NormalizePath(this.Value[1], true);
            }
        }

        internal static string ConfigurationFileKey
        {
            get
            {
                return "APP_CONFIG_FILE";
            }
        }

        internal string DeveloperPath
        {
            get
            {
                string dirs = this.Value[3];
                this.VerifyDirList(dirs);
                return dirs;
            }
            set
            {
                if (value == null)
                {
                    this.Value[3] = null;
                }
                else
                {
                    string[] strArray = value.Split(new char[] { ';' });
                    int length = strArray.Length;
                    StringBuilder builder = new StringBuilder();
                    bool flag = false;
                    for (int i = 0; i < length; i++)
                    {
                        if (strArray[i].Length != 0)
                        {
                            if (flag)
                            {
                                builder.Append(";");
                            }
                            else
                            {
                                flag = true;
                            }
                            builder.Append(Path.GetFullPathInternal(strArray[i]));
                        }
                    }
                    if (builder.ToString().Length == 0)
                    {
                        this.Value[3] = null;
                    }
                    else
                    {
                        this.Value[3] = builder.ToString();
                    }
                }
            }
        }

        internal static string DeveloperPathKey
        {
            get
            {
                return "DEV_PATH";
            }
        }

        internal static string DisallowAppBaseProbingKey
        {
            get
            {
                return "DISALLOW_APP_BASE_PROBING";
            }
        }

        public bool DisallowApplicationBaseProbing
        {
            get
            {
                return (this.Value[14] != null);
            }
            set
            {
                if (value)
                {
                    this.Value[14] = "true";
                }
                else
                {
                    this.Value[14] = null;
                }
            }
        }

        public bool DisallowBindingRedirects
        {
            get
            {
                return (this.Value[13] != null);
            }
            set
            {
                if (value)
                {
                    this.Value[13] = "true";
                }
                else
                {
                    this.Value[13] = null;
                }
            }
        }

        internal static string DisallowBindingRedirectsKey
        {
            get
            {
                return "DISALLOW_APP_REDIRECTS";
            }
        }

        public bool DisallowCodeDownload
        {
            get
            {
                return (this.Value[12] != null);
            }
            set
            {
                if (value)
                {
                    this.Value[12] = "true";
                }
                else
                {
                    this.Value[12] = null;
                }
            }
        }

        internal static string DisallowCodeDownloadKey
        {
            get
            {
                return "CODE_DOWNLOAD_DISABLED";
            }
        }

        public bool DisallowPublisherPolicy
        {
            get
            {
                return (this.Value[11] != null);
            }
            set
            {
                if (value)
                {
                    this.Value[11] = "true";
                }
                else
                {
                    this.Value[11] = null;
                }
            }
        }

        internal static string DisallowPublisherPolicyKey
        {
            get
            {
                return "DISALLOW_APP";
            }
        }

        public string DynamicBase
        {
            get
            {
                return this.VerifyDir(this.Value[2], true);
            }
            set
            {
                if (value == null)
                {
                    this.Value[2] = null;
                }
                else
                {
                    if (this.ApplicationName == null)
                    {
                        throw new MemberAccessException(Environment.GetResourceString("AppDomain_RequireApplicationName"));
                    }
                    StringBuilder builder = new StringBuilder(this.NormalizePath(value, false));
                    builder.Append('\\');
                    string str = ParseNumbers.IntToString(this.ApplicationName.GetHashCode(), 0x10, 8, '0', 0x100);
                    builder.Append(str);
                    this.Value[2] = builder.ToString();
                }
            }
        }

        internal static string DynamicBaseKey
        {
            get
            {
                return "DYNAMIC_BASE";
            }
        }

        internal static string HostBindingKey
        {
            get
            {
                return "HOST_CONFIG";
            }
        }

        public string LicenseFile
        {
            get
            {
                return this.VerifyDir(this.Value[10], true);
            }
            set
            {
                this.Value[10] = value;
            }
        }

        public System.LoaderOptimization LoaderOptimization
        {
            get
            {
                return this._LoaderOptimization;
            }
            set
            {
                this._LoaderOptimization = value;
            }
        }

        internal static string LoaderOptimizationKey
        {
            get
            {
                return "LOADER_OPTIMIZATION";
            }
        }

        internal static string MachineConfigKey
        {
            get
            {
                return "MACHINE_CONFIG";
            }
        }

        public string PrivateBinPath
        {
            get
            {
                string dirs = this.Value[5];
                this.VerifyDirList(dirs);
                return dirs;
            }
            set
            {
                this.Value[5] = value;
            }
        }

        internal static string PrivateBinPathEnvironmentVariable
        {
            get
            {
                return "RELPATH";
            }
        }

        internal static string PrivateBinPathKey
        {
            get
            {
                return "PRIVATE_BINPATH";
            }
        }

        public string PrivateBinPathProbe
        {
            get
            {
                return this.Value[6];
            }
            set
            {
                this.Value[6] = value;
            }
        }

        internal static string PrivateBinPathProbeKey
        {
            get
            {
                return "BINPATH_PROBE_ONLY";
            }
        }

        internal static string RuntimeConfigurationFile
        {
            get
            {
                return @"config\machine.config";
            }
        }

        public bool SandboxInterop
        {
            get
            {
                return this._DisableInterfaceCache;
            }
            set
            {
                this._DisableInterfaceCache = value;
            }
        }

        public string ShadowCopyDirectories
        {
            get
            {
                string dirs = this.Value[7];
                this.VerifyDirList(dirs);
                return dirs;
            }
            set
            {
                this.Value[7] = value;
            }
        }

        internal static string ShadowCopyDirectoriesKey
        {
            get
            {
                return "SHADOW_COPY_DIRS";
            }
        }

        public string ShadowCopyFiles
        {
            get
            {
                return this.Value[8];
            }
            set
            {
                if ((value != null) && (string.Compare(value, "true", StringComparison.OrdinalIgnoreCase) == 0))
                {
                    this.Value[8] = value;
                }
                else
                {
                    this.Value[8] = null;
                }
            }
        }

        internal static string ShadowCopyFilesKey
        {
            get
            {
                return "FORCE_CACHE_INSTALL";
            }
        }

        internal string[] Value
        {
            get
            {
                if (this._Entries == null)
                {
                    this._Entries = new string[0x10];
                }
                return this._Entries;
            }
        }

        [Serializable]
        internal enum LoaderInformation
        {
            ApplicationBaseValue,
            ConfigurationFileValue,
            DynamicBaseValue,
            DevPathValue,
            ApplicationNameValue,
            PrivateBinPathValue,
            PrivateBinPathProbeValue,
            ShadowCopyDirectoriesValue,
            ShadowCopyFilesValue,
            CachePathValue,
            LicenseFileValue,
            DisallowPublisherPolicyValue,
            DisallowCodeDownloadValue,
            DisallowBindingRedirectsValue,
            DisallowAppBaseProbingValue,
            ConfigurationBytesValue,
            LoaderMaximum
        }
    }
}

