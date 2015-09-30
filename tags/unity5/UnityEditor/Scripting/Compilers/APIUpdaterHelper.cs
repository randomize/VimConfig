namespace UnityEditor.Scripting.Compilers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEditor;
    using UnityEditor.Scripting;
    using UnityEditor.Utils;
    using UnityEditor.VersionControl;
    using UnityEditorInternal;
    using UnityEngine;

    internal class APIUpdaterHelper
    {
        [CompilerGenerated]
        private static Func<Asset, bool> <>f__am$cache0;
        [CompilerGenerated]
        private static Func<Asset, string> <>f__am$cache1;
        [CompilerGenerated]
        private static Func<string, string, string> <>f__am$cache2;
        private const string tempOutputPath = "Temp/ScriptUpdater/";

        private static void ReportAPIUpdaterFailure(IEnumerable<string> errorOutput)
        {
            Console.WriteLine("Failed to run script updater.\n ");
            IEnumerator<string> enumerator = errorOutput.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    if (current.StartsWith("unity.console:"))
                    {
                        Debug.LogError(current.Substring("unity.console:".Length));
                    }
                    else
                    {
                        Console.WriteLine(current);
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
            ScriptUpdatingManager.ReportExpectedUpdateFailure();
        }

        private static void RunUpdatingProgram(string executable, string arguments)
        {
            string str = EditorApplication.applicationContentsPath + "/Tools/ScriptUpdater/" + executable;
            ManagedProgram program = new ManagedProgram(MonoInstallationFinder.GetMonoInstallation("MonoBleedingEdge"), "4.5", str, arguments);
            program.LogProcessStartInfo();
            program.Start();
            program.WaitForExit();
            Console.WriteLine(string.Join(Environment.NewLine, program.GetStandardOutput()));
            if (program.ExitCode == 0)
            {
                UpdateFilesInVCIfNeeded();
            }
            else
            {
                ReportAPIUpdaterFailure(program.GetErrorOutput());
            }
        }

        private static void UpdateFilesInVCIfNeeded()
        {
            if (Provider.enabled)
            {
                string[] strArray = Directory.GetFiles("Temp/ScriptUpdater/", "*.*", SearchOption.AllDirectories);
                AssetList assets = new AssetList();
                foreach (string str in strArray)
                {
                    assets.Add(Provider.GetAssetByPath(str.Replace("Temp/ScriptUpdater/", string.Empty)));
                }
                Task task = Provider.Checkout(assets, CheckoutMode.Exact);
                task.Wait();
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = a => (a.state & Asset.States.ReadOnly) == Asset.States.ReadOnly;
                }
                IEnumerable<Asset> source = task.assetList.Where<Asset>(<>f__am$cache0);
                if (!task.success || source.Any<Asset>())
                {
                    object[] args = new object[1];
                    if (<>f__am$cache1 == null)
                    {
                        <>f__am$cache1 = a => string.Concat(new object[] { a.fullName, " (", a.state, ")" });
                    }
                    if (<>f__am$cache2 == null)
                    {
                        <>f__am$cache2 = (acc, curr) => acc + Environment.NewLine + "\t" + curr;
                    }
                    args[0] = source.Select<Asset, string>(<>f__am$cache1).Aggregate<string>(<>f__am$cache2);
                    Debug.LogErrorFormat("[API Updater] Files cannot be updated (failed to check out): {0}", args);
                    ScriptUpdatingManager.ReportExpectedUpdateFailure();
                }
                else
                {
                    FileUtil.CopyDirectoryRecursive("Temp/ScriptUpdater/", ".", true);
                    FileUtil.DeleteFileOrDirectory("Temp/ScriptUpdater/");
                }
            }
        }

        public static void UpdateScripts(string responseFile, string sourceExtension)
        {
            if (ScriptUpdatingManager.WaitForVCSServerConnection(true))
            {
                string str = !Provider.enabled ? "." : "Temp/ScriptUpdater/";
                RunUpdatingProgram("ScriptUpdater.exe", sourceExtension + " " + CommandLineFormatter.PrepareFileName(MonoInstallationFinder.GetFrameWorksFolder()) + " " + str + " " + responseFile);
            }
        }
    }
}

