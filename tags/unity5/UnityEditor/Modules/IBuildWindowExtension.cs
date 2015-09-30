namespace UnityEditor.Modules
{
    using System;

    internal interface IBuildWindowExtension
    {
        bool EnabledBuildAndRunButton();
        bool EnabledBuildButton();
        bool ShouldDrawDevelopmentPlayerCheckbox();
        bool ShouldDrawExplicitNullCheckbox();
        bool ShouldDrawProfilerCheckbox();
        bool ShouldDrawScriptDebuggingCheckbox();
        void ShowInternalPlatformBuildOptions();
        void ShowPlatformBuildOptions();
    }
}

