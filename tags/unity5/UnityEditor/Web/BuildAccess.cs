namespace UnityEditor.Web
{
    using System;
    using UnityEditor;
    using UnityEditor.Connect;

    [InitializeOnLoad]
    internal class BuildAccess : CloudServiceAccess
    {
        private const string kServiceDisplayName = "Unity Build";
        private const string kServiceName = "Build";
        private const string kServiceUrl = "http://public.cloud.unity3d.com/editor/5.2/production/cloud/build";

        static BuildAccess()
        {
            UnityConnectServiceData cloudService = new UnityConnectServiceData("Build", "http://public.cloud.unity3d.com/editor/5.2/production/cloud/build", new BuildAccess(), "unity/project/cloud/build");
            UnityConnectServiceCollection.instance.AddService(cloudService);
        }

        public override string GetServiceDisplayName()
        {
            return "Unity Build";
        }

        public override string GetServiceName()
        {
            return "Build";
        }
    }
}

