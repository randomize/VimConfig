namespace System
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    internal struct AssemblyHandle
    {
        private IntPtr m_ptr;
        internal void* Value
        {
            get
            {
                return this.m_ptr.ToPointer();
            }
        }
        internal unsafe AssemblyHandle(void* pAssembly)
        {
            this.m_ptr = new IntPtr(pAssembly);
        }

        public override int GetHashCode()
        {
            return ValueType.GetHashCodeOfPtr(this.m_ptr);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is AssemblyHandle))
            {
                return false;
            }
            AssemblyHandle handle = (AssemblyHandle) obj;
            return (handle.m_ptr == this.m_ptr);
        }

        public bool Equals(AssemblyHandle handle)
        {
            return (handle.m_ptr == this.m_ptr);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern Assembly GetAssembly();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetManifestModule();
        internal ModuleHandle GetManifestModule()
        {
            return new ModuleHandle(this._GetManifestModule());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern bool _AptcaCheck(IntPtr sourceAssembly);
        internal unsafe bool AptcaCheck(AssemblyHandle sourceAssembly)
        {
            return this._AptcaCheck((IntPtr) sourceAssembly.Value);
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        internal extern int GetToken();
    }
}

