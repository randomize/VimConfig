namespace System
{
    using System.Deployment.Internal.Isolation;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;
    using System.Security.Permissions;

    [Serializable, ComVisible(false)]
    public sealed class ApplicationIdentity : ISerializable
    {
        private IDefinitionAppId _appId;

        private ApplicationIdentity()
        {
        }

        internal ApplicationIdentity(IDefinitionAppId applicationIdentity)
        {
            this._appId = applicationIdentity;
        }

        public ApplicationIdentity(string applicationIdentityFullName)
        {
            if (applicationIdentityFullName == null)
            {
                throw new ArgumentNullException("applicationIdentityFullName");
            }
            this._appId = IsolationInterop.AppIdAuthority.TextToDefinition(0, applicationIdentityFullName);
        }

        private ApplicationIdentity(SerializationInfo info, StreamingContext context)
        {
            string identity = (string) info.GetValue("FullName", typeof(string));
            if (identity == null)
            {
                throw new ArgumentNullException("fullName");
            }
            this._appId = IsolationInterop.AppIdAuthority.TextToDefinition(0, identity);
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("FullName", this.FullName, typeof(string));
        }

        public override string ToString()
        {
            return this.FullName;
        }

        public string CodeBase
        {
            get
            {
                return this._appId.get_Codebase();
            }
        }

        public string FullName
        {
            get
            {
                return IsolationInterop.AppIdAuthority.DefinitionToText(0, this._appId);
            }
        }

        internal IDefinitionAppId Identity
        {
            get
            {
                return this._appId;
            }
        }
    }
}

