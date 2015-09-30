namespace System.Reflection
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    [Serializable, CLSCompliant(false), ComVisible(true)]
    public sealed class Pointer : ISerializable
    {
        private unsafe void* _ptr;
        private Type _ptrType;

        private Pointer()
        {
        }

        private unsafe Pointer(SerializationInfo info, StreamingContext context)
        {
            this._ptr = ((IntPtr) info.GetValue("_ptr", typeof(IntPtr))).ToPointer();
            this._ptrType = (Type) info.GetValue("_ptrType", typeof(Type));
        }

        public static unsafe object Box(void* ptr, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsPointer)
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBePointer"), "ptr");
            }
            return new Pointer { _ptr = ptr, _ptrType = type };
        }

        internal Type GetPointerType()
        {
            return this._ptrType;
        }

        internal unsafe object GetPointerValue()
        {
            return (IntPtr) this._ptr;
        }

        unsafe void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("_ptr", new IntPtr(this._ptr));
            info.AddValue("_ptrType", this._ptrType);
        }

        public static unsafe void* Unbox(object ptr)
        {
            if (!(ptr is Pointer))
            {
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBePointer"), "ptr");
            }
            return ((Pointer) ptr)._ptr;
        }
    }
}

