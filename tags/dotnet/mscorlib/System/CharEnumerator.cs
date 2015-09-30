namespace System
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    [Serializable, ComVisible(true)]
    public sealed class CharEnumerator : ICloneable, IEnumerator<char>, IDisposable, IEnumerator
    {
        private char currentElement;
        private int index;
        private string str;

        internal CharEnumerator(string str)
        {
            this.str = str;
            this.index = -1;
        }

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        public bool MoveNext()
        {
            if (this.index < (this.str.Length - 1))
            {
                this.index++;
                this.currentElement = this.str[this.index];
                return true;
            }
            this.index = this.str.Length;
            return false;
        }

        public void Reset()
        {
            this.currentElement = '\0';
            this.index = -1;
        }

        void IDisposable.Dispose()
        {
        }

        public char Current
        {
            get
            {
                if (this.index == -1)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumNotStarted"));
                }
                if (this.index >= this.str.Length)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumEnded"));
                }
                return this.currentElement;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                if (this.index == -1)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumNotStarted"));
                }
                if (this.index >= this.str.Length)
                {
                    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_EnumEnded"));
                }
                return this.currentElement;
            }
        }
    }
}

