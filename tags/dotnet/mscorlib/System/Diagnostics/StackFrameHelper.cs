namespace System.Diagnostics
{
    using System;
    using System.Reflection;
    using System.Runtime.Serialization;
    using System.Threading;

    [Serializable]
    internal class StackFrameHelper
    {
        private object dynamicMethods;
        private bool fNeedFileInfo;
        private int iFrameCount;
        private string[] rgFilename;
        private int[] rgiColumnNumber;
        private int[] rgiILOffset;
        private int[] rgiLineNumber;
        private int[] rgiOffset;
        private MethodBase[] rgMethodBase;
        [NonSerialized]
        private RuntimeMethodHandle[] rgMethodHandle;
        [NonSerialized]
        private Thread targetThread;

        public StackFrameHelper(bool fNeedFileLineColInfo, Thread target)
        {
            this.targetThread = target;
            this.rgMethodBase = null;
            this.rgMethodHandle = null;
            this.rgiOffset = null;
            this.rgiILOffset = null;
            this.rgFilename = null;
            this.rgiLineNumber = null;
            this.rgiColumnNumber = null;
            this.dynamicMethods = null;
            this.iFrameCount = 0x200;
            this.fNeedFileInfo = fNeedFileLineColInfo;
        }

        public virtual int GetColumnNumber(int i)
        {
            return this.rgiColumnNumber[i];
        }

        public virtual string GetFilename(int i)
        {
            return this.rgFilename[i];
        }

        public virtual int GetILOffset(int i)
        {
            return this.rgiILOffset[i];
        }

        public virtual int GetLineNumber(int i)
        {
            return this.rgiLineNumber[i];
        }

        public virtual MethodBase GetMethodBase(int i)
        {
            RuntimeMethodHandle handle = this.rgMethodHandle[i];
            if (handle.IsNullHandle())
            {
                return null;
            }
            return RuntimeType.GetMethodBase(handle.GetTypicalMethodDefinition());
        }

        public virtual int GetNumberOfFrames()
        {
            return this.iFrameCount;
        }

        public virtual int GetOffset(int i)
        {
            return this.rgiOffset[i];
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.rgMethodHandle = (this.rgMethodBase == null) ? null : new RuntimeMethodHandle[this.rgMethodBase.Length];
            if (this.rgMethodBase != null)
            {
                for (int i = 0; i < this.rgMethodBase.Length; i++)
                {
                    if (this.rgMethodBase[i] != null)
                    {
                        this.rgMethodHandle[i] = this.rgMethodBase[i].MethodHandle;
                    }
                }
            }
            this.rgMethodBase = null;
        }

        [OnSerialized]
        private void OnSerialized(StreamingContext context)
        {
            this.rgMethodBase = null;
        }

        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            this.rgMethodBase = (this.rgMethodHandle == null) ? null : new MethodBase[this.rgMethodHandle.Length];
            if (this.rgMethodHandle != null)
            {
                for (int i = 0; i < this.rgMethodHandle.Length; i++)
                {
                    if (!this.rgMethodHandle[i].IsNullHandle())
                    {
                        this.rgMethodBase[i] = RuntimeType.GetMethodBase(this.rgMethodHandle[i]);
                    }
                }
            }
        }

        public virtual void SetNumberOfFrames(int i)
        {
            this.iFrameCount = i;
        }
    }
}

