namespace System.Deployment.Internal.Isolation
{
    using System;
    using System.Collections;

    internal class StoreAssemblyFileEnumeration : IEnumerator
    {
        private STORE_ASSEMBLY_FILE _current;
        private IEnumSTORE_ASSEMBLY_FILE _enum;
        private bool _fValid;

        public StoreAssemblyFileEnumeration(IEnumSTORE_ASSEMBLY_FILE pI)
        {
            this._enum = pI;
        }

        private STORE_ASSEMBLY_FILE GetCurrent()
        {
            if (!this._fValid)
            {
                throw new InvalidOperationException();
            }
            return this._current;
        }

        public IEnumerator GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            STORE_ASSEMBLY_FILE[] rgelt = new STORE_ASSEMBLY_FILE[1];
            uint num = this._enum.Next(1, rgelt);
            if (num == 1)
            {
                this._current = rgelt[0];
            }
            return (this._fValid = num == 1);
        }

        public void Reset()
        {
            this._fValid = false;
            this._enum.Reset();
        }

        public STORE_ASSEMBLY_FILE Current
        {
            get
            {
                return this.GetCurrent();
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.GetCurrent();
            }
        }
    }
}

