namespace System
{
    using System.Globalization;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class Version : ICloneable, IComparable, IComparable<Version>, IEquatable<Version>
    {
        private int _Build;
        private int _Major;
        private int _Minor;
        private int _Revision;

        public Version()
        {
            this._Build = -1;
            this._Revision = -1;
            this._Major = 0;
            this._Minor = 0;
        }

        public Version(string version)
        {
            this._Build = -1;
            this._Revision = -1;
            if (version == null)
            {
                throw new ArgumentNullException("version");
            }
            string[] strArray = version.Split(new char[] { '.' });
            int length = strArray.Length;
            if ((length < 2) || (length > 4))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_VersionString"));
            }
            this._Major = int.Parse(strArray[0], CultureInfo.InvariantCulture);
            if (this._Major < 0)
            {
                throw new ArgumentOutOfRangeException("version", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            this._Minor = int.Parse(strArray[1], CultureInfo.InvariantCulture);
            if (this._Minor < 0)
            {
                throw new ArgumentOutOfRangeException("version", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            length -= 2;
            if (length > 0)
            {
                this._Build = int.Parse(strArray[2], CultureInfo.InvariantCulture);
                if (this._Build < 0)
                {
                    throw new ArgumentOutOfRangeException("build", Environment.GetResourceString("ArgumentOutOfRange_Version"));
                }
                length--;
                if (length > 0)
                {
                    this._Revision = int.Parse(strArray[3], CultureInfo.InvariantCulture);
                    if (this._Revision < 0)
                    {
                        throw new ArgumentOutOfRangeException("revision", Environment.GetResourceString("ArgumentOutOfRange_Version"));
                    }
                }
            }
        }

        public Version(int major, int minor)
        {
            this._Build = -1;
            this._Revision = -1;
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            this._Major = major;
            this._Minor = minor;
        }

        public Version(int major, int minor, int build)
        {
            this._Build = -1;
            this._Revision = -1;
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            if (build < 0)
            {
                throw new ArgumentOutOfRangeException("build", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            this._Major = major;
            this._Minor = minor;
            this._Build = build;
        }

        public Version(int major, int minor, int build, int revision)
        {
            this._Build = -1;
            this._Revision = -1;
            if (major < 0)
            {
                throw new ArgumentOutOfRangeException("major", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            if (minor < 0)
            {
                throw new ArgumentOutOfRangeException("minor", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            if (build < 0)
            {
                throw new ArgumentOutOfRangeException("build", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            if (revision < 0)
            {
                throw new ArgumentOutOfRangeException("revision", Environment.GetResourceString("ArgumentOutOfRange_Version"));
            }
            this._Major = major;
            this._Minor = minor;
            this._Build = build;
            this._Revision = revision;
        }

        public object Clone()
        {
            return new Version { _Major = this._Major, _Minor = this._Minor, _Build = this._Build, _Revision = this._Revision };
        }

        public int CompareTo(object version)
        {
            if (version == null)
            {
                return 1;
            }
            Version version2 = version as Version;
            if (version2 == null)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeVersion"));
            }
            if (this._Major != version2._Major)
            {
                if (this._Major > version2._Major)
                {
                    return 1;
                }
                return -1;
            }
            if (this._Minor != version2._Minor)
            {
                if (this._Minor > version2._Minor)
                {
                    return 1;
                }
                return -1;
            }
            if (this._Build != version2._Build)
            {
                if (this._Build > version2._Build)
                {
                    return 1;
                }
                return -1;
            }
            if (this._Revision == version2._Revision)
            {
                return 0;
            }
            if (this._Revision > version2._Revision)
            {
                return 1;
            }
            return -1;
        }

        public int CompareTo(Version value)
        {
            if (value == null)
            {
                return 1;
            }
            if (this._Major != value._Major)
            {
                if (this._Major > value._Major)
                {
                    return 1;
                }
                return -1;
            }
            if (this._Minor != value._Minor)
            {
                if (this._Minor > value._Minor)
                {
                    return 1;
                }
                return -1;
            }
            if (this._Build != value._Build)
            {
                if (this._Build > value._Build)
                {
                    return 1;
                }
                return -1;
            }
            if (this._Revision == value._Revision)
            {
                return 0;
            }
            if (this._Revision > value._Revision)
            {
                return 1;
            }
            return -1;
        }

        public override bool Equals(object obj)
        {
            Version version = obj as Version;
            if (version == null)
            {
                return false;
            }
            return (((this._Major == version._Major) && (this._Minor == version._Minor)) && ((this._Build == version._Build) && (this._Revision == version._Revision)));
        }

        public bool Equals(Version obj)
        {
            if (obj == null)
            {
                return false;
            }
            return (((this._Major == obj._Major) && (this._Minor == obj._Minor)) && ((this._Build == obj._Build) && (this._Revision == obj._Revision)));
        }

        public override int GetHashCode()
        {
            int num = 0;
            num |= (this._Major & 15) << 0x1c;
            num |= (this._Minor & 0xff) << 20;
            num |= (this._Build & 0xff) << 12;
            return (num | (this._Revision & 0xfff));
        }

        public static bool operator ==(Version v1, Version v2)
        {
            if (object.ReferenceEquals(v1, null))
            {
                return object.ReferenceEquals(v2, null);
            }
            return v1.Equals(v2);
        }

        public static bool operator >(Version v1, Version v2)
        {
            return (v2 < v1);
        }

        public static bool operator >=(Version v1, Version v2)
        {
            return (v2 <= v1);
        }

        public static bool operator !=(Version v1, Version v2)
        {
            return !(v1 == v2);
        }

        public static bool operator <(Version v1, Version v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) < 0);
        }

        public static bool operator <=(Version v1, Version v2)
        {
            if (v1 == null)
            {
                throw new ArgumentNullException("v1");
            }
            return (v1.CompareTo(v2) <= 0);
        }

        public override string ToString()
        {
            if (this._Build == -1)
            {
                return this.ToString(2);
            }
            if (this._Revision == -1)
            {
                return this.ToString(3);
            }
            return this.ToString(4);
        }

        public string ToString(int fieldCount)
        {
            switch (fieldCount)
            {
                case 0:
                    return string.Empty;

                case 1:
                    return (this._Major);

                case 2:
                    return (this._Major + "." + this._Minor);
            }
            if (this._Build == -1)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper"), new object[] { "0", "2" }), "fieldCount");
            }
            if (fieldCount == 3)
            {
                return string.Concat(new object[] { this._Major, ".", this._Minor, ".", this._Build });
            }
            if (this._Revision == -1)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper"), new object[] { "0", "3" }), "fieldCount");
            }
            if (fieldCount != 4)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("ArgumentOutOfRange_Bounds_Lower_Upper"), new object[] { "0", "4" }), "fieldCount");
            }
            return string.Concat(new object[] { this.Major, ".", this._Minor, ".", this._Build, ".", this._Revision });
        }

        public int Build
        {
            get
            {
                return this._Build;
            }
        }

        public int Major
        {
            get
            {
                return this._Major;
            }
        }

        public short MajorRevision
        {
            get
            {
                return (short) (this._Revision >> 0x10);
            }
        }

        public int Minor
        {
            get
            {
                return this._Minor;
            }
        }

        public short MinorRevision
        {
            get
            {
                return (short) (this._Revision & 0xffff);
            }
        }

        public int Revision
        {
            get
            {
                return this._Revision;
            }
        }
    }
}

