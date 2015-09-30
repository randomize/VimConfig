namespace System.Globalization
{
    using System;

    internal abstract class BaseInfoTable
    {
        internal string fileName;
        internal bool fromAssembly;
        internal uint m_itemSize;
        internal uint m_numItem;
        protected unsafe CultureTableHeader* m_pCultureHeader;
        internal unsafe byte* m_pDataFileStart;
        internal unsafe ushort* m_pDataPool;
        internal unsafe byte* m_pItemData;
        protected bool m_valid = true;
        protected AgileSafeNativeMemoryHandle memoryMapFile;

        internal BaseInfoTable(string fileName, bool fromAssembly)
        {
            this.fileName = fileName;
            this.fromAssembly = fromAssembly;
            this.InitializeBaseInfoTablePointers(fileName, fromAssembly);
        }

        internal unsafe int CompareStringToStringPoolStringBinary(string name, int offset)
        {
            int num = 0;
            char* chPtr = (char*) (this.m_pDataPool + offset);
            if (chPtr[1] == '\0')
            {
                if (name.Length == 0)
                {
                    return 0;
                }
                return 1;
            }
            for (int i = 0; (i < chPtr[0]) && (i < name.Length); i++)
            {
                num = name[i] - (((chPtr[i + 1] <= 'Z') && (chPtr[i + 1] >= 'A')) ? ((chPtr[i + 1] + 'a') - 0x41) : chPtr[i + 1]);
                if (num != 0)
                {
                    break;
                }
            }
            if (num != 0)
            {
                return num;
            }
            return (name.Length - chPtr[0]);
        }

        public override bool Equals(object value)
        {
            BaseInfoTable table = value as BaseInfoTable;
            if (table == null)
            {
                return false;
            }
            return ((this.fromAssembly == table.fromAssembly) && (CultureInfo.InvariantCulture.CompareInfo.Compare(this.fileName, table.fileName, CompareOptions.IgnoreCase) == 0));
        }

        public override int GetHashCode()
        {
            return this.fileName.GetHashCode();
        }

        internal unsafe string[] GetStringArray(uint iOffset)
        {
            if (iOffset == 0)
            {
                return new string[0];
            }
            ushort* numPtr = this.m_pDataPool + ((ushort*) iOffset);
            int num = numPtr[0];
            string[] strArray = new string[num];
            uint* numPtr2 = (uint*) (numPtr + 1);
            for (int i = 0; i < num; i++)
            {
                strArray[i] = this.GetStringPoolString(numPtr2[i]);
            }
            return strArray;
        }

        internal unsafe string GetStringPoolString(uint offset)
        {
            char* chPtr = (char*) (this.m_pDataPool + offset);
            if (chPtr[1] == '\0')
            {
                return string.Empty;
            }
            return new string(chPtr + 1, 0, chPtr[0]);
        }

        internal unsafe int[][] GetWordArrayArray(uint iOffset)
        {
            if (iOffset == 0)
            {
                return new int[0][];
            }
            short* numPtr = (short*) (this.m_pDataPool + iOffset);
            int num = numPtr[0];
            int[][] numArray = new int[num][];
            uint* numPtr2 = (uint*) (numPtr + 1);
            for (int i = 0; i < num; i++)
            {
                numPtr = (short*) (this.m_pDataPool + numPtr2[i]);
                int num3 = numPtr[0];
                numPtr++;
                numArray[i] = new int[num3];
                for (int j = 0; j < num3; j++)
                {
                    numArray[i][j] = numPtr[j];
                }
            }
            return numArray;
        }

        internal unsafe void InitializeBaseInfoTablePointers(string fileName, bool fromAssembly)
        {
            if (fromAssembly)
            {
                this.m_pDataFileStart = GlobalizationAssembly.GetGlobalizationResourceBytePtr(typeof(BaseInfoTable).Assembly, fileName);
            }
            else
            {
                this.memoryMapFile = new AgileSafeNativeMemoryHandle(fileName);
                if (this.memoryMapFile.FileSize == 0L)
                {
                    this.m_valid = false;
                    return;
                }
                this.m_pDataFileStart = this.memoryMapFile.GetBytePtr();
            }
            EndianessHeader* pDataFileStart = (EndianessHeader*) this.m_pDataFileStart;
            this.m_pCultureHeader = (CultureTableHeader*) (this.m_pDataFileStart + pDataFileStart->leOffset);
            this.SetDataItemPointers();
        }

        internal abstract void SetDataItemPointers();

        internal bool IsValid
        {
            get
            {
                return this.m_valid;
            }
        }
    }
}

