namespace System.Globalization
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;
    using System.Threading;

    internal sealed class GlobalizationAssembly
    {
        internal Hashtable compareInfoCache = new Hashtable(4);
        private static Hashtable m_assemblyHash = Hashtable.Synchronized(new Hashtable(4));
        internal static GlobalizationAssembly m_defaultInstance = GetGlobalizationAssembly(Assembly.GetAssembly(typeof(GlobalizationAssembly)));
        internal unsafe void* pNativeGlobalizationAssembly;

        internal GlobalizationAssembly()
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private static extern unsafe void* _nativeCreateGlobalizationAssembly(Assembly assembly);
        [PrePrepareMethod]
        private static void CreateGlobalizationAssembly(object assem)
        {
            Assembly assembly2 = (Assembly) assem;
            GlobalizationAssembly assembly = (GlobalizationAssembly) m_assemblyHash[assembly2];
            if (assembly == null)
            {
                assembly = new GlobalizationAssembly {
                    pNativeGlobalizationAssembly = nativeCreateGlobalizationAssembly(assembly2)
                };
                Thread.MemoryBarrier();
                m_assemblyHash[assembly2] = assembly;
            }
        }

        internal static GlobalizationAssembly GetGlobalizationAssembly(Assembly assembly)
        {
            GlobalizationAssembly assembly2 = (GlobalizationAssembly) m_assemblyHash[assembly];
            if (assembly2 == null)
            {
                RuntimeHelpers.TryCode code = new RuntimeHelpers.TryCode(GlobalizationAssembly.CreateGlobalizationAssembly);
                RuntimeHelpers.ExecuteCodeWithLock(typeof(CultureTableRecord), code, assembly);
                assembly2 = (GlobalizationAssembly) m_assemblyHash[assembly];
            }
            return assembly2;
        }

        internal static unsafe byte* GetGlobalizationResourceBytePtr(Assembly assembly, string tableName)
        {
            UnmanagedMemoryStream manifestResourceStream = assembly.GetManifestResourceStream(tableName) as UnmanagedMemoryStream;
            if (manifestResourceStream != null)
            {
                byte* positionPointer = manifestResourceStream.PositionPointer;
                if (positionPointer != null)
                {
                    return positionPointer;
                }
            }
            throw new ExecutionEngineException();
        }

        private static unsafe void* nativeCreateGlobalizationAssembly(Assembly assembly)
        {
            return _nativeCreateGlobalizationAssembly(assembly.InternalAssembly);
        }

        internal static GlobalizationAssembly DefaultInstance
        {
            get
            {
                if (m_defaultInstance == null)
                {
                    throw new TypeLoadException("Failure has occurred while loading a type.");
                }
                return m_defaultInstance;
            }
        }
    }
}

