namespace System
{
    using System.Runtime.CompilerServices;

    public struct ArgIterator
    {
        private IntPtr ArgCookie;
        private IntPtr ArgPtr;
        private int RemainingArgs;
        private IntPtr sigPtr;
        private IntPtr sigPtrLen;

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern ArgIterator(IntPtr arglist);
        public ArgIterator(RuntimeArgumentHandle arglist) : this(arglist.Value)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe ArgIterator(IntPtr arglist, void* ptr);
        [CLSCompliant(false)]
        public unsafe ArgIterator(RuntimeArgumentHandle arglist, void* ptr) : this(arglist.Value, ptr)
        {
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void* _GetNextArgType();
        public void End()
        {
        }

        public override bool Equals(object o)
        {
            throw new NotSupportedException(Environment.GetResourceString("NotSupported_NYI"));
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void FCallGetNextArg(void* result);
        public override int GetHashCode()
        {
            return ValueType.GetHashCodeOfPtr(this.ArgCookie);
        }

        [CLSCompliant(false)]
        public unsafe TypedReference GetNextArg()
        {
            TypedReference reference = new TypedReference();
            this.FCallGetNextArg((void*) &reference);
            return reference;
        }

        [CLSCompliant(false)]
        public unsafe TypedReference GetNextArg(RuntimeTypeHandle rth)
        {
            if (this.sigPtr != IntPtr.Zero)
            {
                return this.GetNextArg();
            }
            if (this.ArgPtr == IntPtr.Zero)
            {
                throw new ArgumentNullException();
            }
            TypedReference reference = new TypedReference();
            this.InternalGetNextArg((void*) &reference, rth);
            return reference;
        }

        public RuntimeTypeHandle GetNextArgType()
        {
            return new RuntimeTypeHandle(this._GetNextArgType());
        }

        [MethodImpl(MethodImplOptions.InternalCall)]
        public extern int GetRemainingCount();
        [MethodImpl(MethodImplOptions.InternalCall)]
        private extern unsafe void InternalGetNextArg(void* result, RuntimeTypeHandle rth);
    }
}

