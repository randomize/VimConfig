namespace UnityEngine.Networking
{
    using System;
    using System.Collections.Generic;

    public sealed class SyncListFloat : SyncList<float>
    {
        protected override float DeserializeItem(NetworkReader reader)
        {
            return reader.ReadSingle();
        }

        public static SyncListFloat ReadInstance(NetworkReader reader)
        {
            ushort num = reader.ReadUInt16();
            SyncListFloat num2 = new SyncListFloat();
            for (ushort i = 0; i < num; i = (ushort) (i + 1))
            {
                num2.AddInternal(reader.ReadSingle());
            }
            return num2;
        }

        protected override void SerializeItem(NetworkWriter writer, float item)
        {
            writer.Write(item);
        }

        public static void WriteInstance(NetworkWriter writer, SyncListFloat items)
        {
            writer.Write((ushort) items.Count);
            IEnumerator<float> enumerator = items.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    float current = enumerator.Current;
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

