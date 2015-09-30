namespace UnityEngine.Networking
{
    using System;

    public class SyncListStruct<T> : SyncList<T> where T: struct
    {
        public void AddInternal(T item)
        {
            base.AddInternal(item);
        }

        protected override T DeserializeItem(NetworkReader reader)
        {
            return default(T);
        }

        public T GetItem(int i)
        {
            return base[i];
        }

        protected override void SerializeItem(NetworkWriter writer, T item)
        {
        }

        public ushort Count
        {
            get
            {
                return (ushort) base.Count;
            }
        }
    }
}

