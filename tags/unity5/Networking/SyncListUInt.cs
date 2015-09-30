namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;

    public class SyncListUInt : SyncList<uint>
    {
        protected override uint DeserializeItem(NetworkReader reader)
        {
            return reader.ReadPackedUInt32();
        }

        public static SyncListUInt ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListUInt num2 = new SyncListUInt();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                num2.AddInternal(reader.ReadPackedUInt32());
            }
            return num2;
        }

        protected override void SerializeItem(NetworkWriter writer, uint item)
        {
            writer.WritePackedUInt32(item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListUInt items)
        {
            writer.Write((ushort) items.Count);
            IEnumerator<uint> enumerator = items.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    uint current = enumerator.Current;
                    writer.WritePackedUInt32(current);
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

