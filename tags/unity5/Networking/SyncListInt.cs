namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;

    public class SyncListInt : SyncList<int>
    {
        protected override int DeserializeItem(NetworkReader reader)
        {
            return (int) reader.ReadPackedUInt32();
        }

        public static SyncListInt ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListInt num2 = new SyncListInt();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                num2.AddInternal((int) reader.ReadPackedUInt32());
            }
            return num2;
        }

        protected override void SerializeItem(NetworkWriter writer, int item)
        {
            writer.WritePackedUInt32((uint) item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListInt items)
        {
            writer.Write((ushort) items.Count);
            IEnumerator<int> enumerator = items.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    int current = enumerator.Current;
                    writer.WritePackedUInt32((uint) current);
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

