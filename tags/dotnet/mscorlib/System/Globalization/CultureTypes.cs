namespace System.Globalization
{
    using System;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true), Flags]
    public enum CultureTypes
    {
        AllCultures = 7,
        FrameworkCultures = 0x40,
        InstalledWin32Cultures = 4,
        NeutralCultures = 1,
        ReplacementCultures = 0x10,
        SpecificCultures = 2,
        UserCustomCulture = 8,
        WindowsOnlyCultures = 0x20
    }
}

