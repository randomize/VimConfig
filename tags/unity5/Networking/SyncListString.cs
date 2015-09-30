namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;

    public sealed class SyncListString : SyncList<string>
    {
        protected override string DeserializeItem(NetworkReader reader)
        {
            return reader.ReadString();
        }

        public static SyncListString ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListString str = new SyncListString();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                str.AddInternal(reader.ReadString());
            }
            return str;
        }

        protected override void SerializeItem(NetworkWriter writer, string item)
        {
            writer.Write(item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListString items)
        {
            writer.Write((ushort) items.Count);
            IEnumerator<string> enumerator = items.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    string current = enumerator.Current;
                    writer.Write(current);
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
    }
}

