namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;

    public class SyncListBool : SyncList<bool>
    {
        protected override bool DeserializeItem(NetworkReader reader)
        {
            return reader.ReadBoolean();
        }

        public static SyncListBool ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListBool @bool = new SyncListBool();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                @bool.AddInternal(reader.ReadBoolean());
            }
            return @bool;
        }

        protected override void SerializeItem(NetworkWriter writer, bool item)
        {
            writer.Write(item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListBool items)
        {
            writer.Write((ushort) items.Count);
            IEnumerator<bool> enumerator = items.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    bool current = enumerator.Current;
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

