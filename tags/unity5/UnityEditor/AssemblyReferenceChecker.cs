namespace UnityEditor
{
    using Mono.Cecil;
    using Mono.Cecil.Cil;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;

    internal class AssemblyReferenceChecker
    {
        private HashSet<AssemblyDefinition> assemblyDefinitions = new HashSet<AssemblyDefinition>();
        private HashSet<string> assemblyFileNames = new HashSet<string>();
        private HashSet<string> definedMethods = new HashSet<string>();
        private HashSet<string> referencedMethods = new HashSet<string>();
        private HashSet<string> referencedTypes = new HashSet<string>();
        private DateTime startTime = DateTime.MinValue;

        private void CollectReferencedMethods(AssemblyDefinition[] definitions, HashSet<string> referencedMethods, HashSet<string> definedMethods, float progressValue)
        {
            foreach (AssemblyDefinition definition in definitions)
            {
                foreach (TypeDefinition definition2 in definition.MainModule.Types)
                {
                    this.CollectReferencedMethods(definition2, referencedMethods, definedMethods, progressValue);
                }
            }
        }

        private void CollectReferencedMethods(TypeDefinition typ, HashSet<string> referencedMethods, HashSet<string> definedMethods, float progressValue)
        {
            this.DisplayProgress(progressValue);
            foreach (TypeDefinition definition in typ.NestedTypes)
            {
                this.CollectReferencedMethods(definition, referencedMethods, definedMethods, progressValue);
            }
            foreach (MethodDefinition definition2 in typ.Methods)
            {
                if (definition2.HasBody)
                {
                    foreach (Instruction instruction in definition2.Body.Instructions)
                    {
                        if (OpCodes.Call == instruction.OpCode)
                        {
                            referencedMethods.Add(instruction.Operand.ToString());
                        }
                    }
                    definedMethods.Add(definition2.ToString());
                }
            }
        }

        public void CollectReferences(string path, bool withMethods, float progressValue, bool ignoreSystemDlls)
        {
            this.assemblyDefinitions = new HashSet<AssemblyDefinition>();
            string[] strArray = !Directory.Exists(path) ? new string[0] : Directory.GetFiles(path);
            foreach (string str in strArray)
            {
                if (Path.GetExtension(str) == ".dll")
                {
                    AssemblyDefinition item = AssemblyDefinition.ReadAssembly(str);
                    if (!ignoreSystemDlls || !this.IsiPhoneIgnoredSystemDll(item.Name.Name))
                    {
                        this.assemblyFileNames.Add(Path.GetFileName(str));
                        this.assemblyDefinitions.Add(item);
                    }
                }
            }
            AssemblyDefinition[] assemblies = this.assemblyDefinitions.ToArray<AssemblyDefinition>();
            this.referencedTypes = MonoAOTRegistration.BuildReferencedTypeList(assemblies);
            if (withMethods)
            {
                this.CollectReferencedMethods(assemblies, this.referencedMethods, this.definedMethods, progressValue);
            }
        }

        public void CollectReferencesFromRoots(string dir, IEnumerable<string> roots, bool withMethods, float progressValue, bool ignoreSystemDlls)
        {
            this.CollectReferencesFromRootsRecursive(dir, roots, ignoreSystemDlls);
            AssemblyDefinition[] assemblies = this.assemblyDefinitions.ToArray<AssemblyDefinition>();
            this.referencedTypes = MonoAOTRegistration.BuildReferencedTypeList(assemblies);
            if (withMethods)
            {
                this.CollectReferencedMethods(assemblies, this.referencedMethods, this.definedMethods, progressValue);
            }
        }

        private void CollectReferencesFromRootsRecursive(string dir, IEnumerable<string> roots, bool ignoreSystemDlls)
        {
            IEnumerator<string> enumerator = roots.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    string fileName = Path.Combine(dir, current);
                    if (!this.assemblyFileNames.Contains(current))
                    {
                        AssemblyDefinition item = AssemblyDefinition.ReadAssembly(fileName);
                        if (!ignoreSystemDlls || !this.IsiPhoneIgnoredSystemDll(item.Name.Name))
                        {
                            this.assemblyFileNames.Add(current);
                            this.assemblyDefinitions.Add(item);
                            foreach (AssemblyNameReference reference in item.MainModule.AssemblyReferences)
                            {
                                string str3 = reference.Name + ".dll";
                                if (!this.assemblyFileNames.Contains(str3))
                                {
                                    string[] textArray1 = new string[] { str3 };
                                    this.CollectReferencesFromRootsRecursive(dir, textArray1, ignoreSystemDlls);
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (enumerator == null)
                {
                }
                enumerator.Dispose();
            }
        }

        private void DisplayProgress(float progressValue)
        {
            TimeSpan span = (TimeSpan) (DateTime.Now - this.startTime);
            string[] strArray = new string[] { "Fetching assembly references", "Building list of referenced assemblies..." };
            if (span.TotalMilliseconds >= 100.0)
            {
                if (EditorUtility.DisplayCancelableProgressBar(strArray[0], strArray[1], progressValue))
                {
                    throw new OperationCanceledException();
                }
                this.startTime = DateTime.Now;
            }
        }

        public AssemblyDefinition[] GetAssemblyDefinitions()
        {
            return this.assemblyDefinitions.ToArray<AssemblyDefinition>();
        }

        public string[] GetAssemblyFileNames()
        {
            return this.assemblyFileNames.ToArray<string>();
        }

        public static bool GetScriptsHaveMouseEvents(string path)
        {
            AssemblyReferenceChecker checker = new AssemblyReferenceChecker();
            checker.CollectReferences(path, true, 0f, true);
            return checker.HasDefinedMethod("OnMouse");
        }

        public bool HasDefinedMethod(string methodName)
        {
            <HasDefinedMethod>c__AnonStoreyA3 ya = new <HasDefinedMethod>c__AnonStoreyA3 {
                methodName = methodName
            };
            return this.definedMethods.Any<string>(new Func<string, bool>(ya.<>m__1F0));
        }

        public bool HasReferenceToMethod(string methodName)
        {
            <HasReferenceToMethod>c__AnonStoreyA2 ya = new <HasReferenceToMethod>c__AnonStoreyA2 {
                methodName = methodName
            };
            return this.referencedMethods.Any<string>(new Func<string, bool>(ya.<>m__1EF));
        }

        public bool HasReferenceToType(string typeName)
        {
            <HasReferenceToType>c__AnonStoreyA4 ya = new <HasReferenceToType>c__AnonStoreyA4 {
                typeName = typeName
            };
            return this.referencedTypes.Any<string>(new Func<string, bool>(ya.<>m__1F1));
        }

        private bool IsiPhoneIgnoredSystemDll(string name)
        {
            return ((name.StartsWith("System") || name.Equals("UnityEngine")) || name.Equals("Mono.Posix"));
        }

        public string WhoReferencesClass(string klass, bool ignoreSystemDlls)
        {
            <WhoReferencesClass>c__AnonStoreyA5 ya = new <WhoReferencesClass>c__AnonStoreyA5 {
                klass = klass
            };
            foreach (AssemblyDefinition definition in this.assemblyDefinitions)
            {
                if (!ignoreSystemDlls || !this.IsiPhoneIgnoredSystemDll(definition.Name.Name))
                {
                    AssemblyDefinition[] assemblies = new AssemblyDefinition[] { definition };
                    if (MonoAOTRegistration.BuildReferencedTypeList(assemblies).Any<string>(new Func<string, bool>(ya.<>m__1F2)))
                    {
                        return definition.Name.Name;
                    }
                }
            }
            return null;
        }

        [CompilerGenerated]
        private sealed class <HasDefinedMethod>c__AnonStoreyA3
        {
            internal string methodName;

            internal bool <>m__1F0(string item)
            {
                return item.Contains(this.methodName);
            }
        }

        [CompilerGenerated]
        private sealed class <HasReferenceToMethod>c__AnonStoreyA2
        {
            internal string methodName;

            internal bool <>m__1EF(string item)
            {
                return item.Contains(this.methodName);
            }
        }

        [CompilerGenerated]
        private sealed class <HasReferenceToType>c__AnonStoreyA4
        {
            internal string typeName;

            internal bool <>m__1F1(string item)
            {
                return item.StartsWith(this.typeName);
            }
        }

        [CompilerGenerated]
        private sealed class <WhoReferencesClass>c__AnonStoreyA5
        {
            internal string klass;

            internal bool <>m__1F2(string item)
            {
                return item.StartsWith(this.klass);
            }
        }
    }
}

