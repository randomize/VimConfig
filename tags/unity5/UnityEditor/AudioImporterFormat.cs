namespace UnityEditor
{
    using System;
    using System.ComponentModel;

    [Obsolete("UnityEditor.AudioImporterFormat has been deprecated. Use UnityEngine.AudioCompressionFormat instead."), EditorBrowsable(EditorBrowsableState.Never)]
    public enum AudioImporterFormat
    {
        Compressed = 0,
        Native = -1
    }
}

