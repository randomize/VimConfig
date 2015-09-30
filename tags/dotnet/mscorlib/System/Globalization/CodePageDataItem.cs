namespace System.Globalization
{
    using System;

    [Serializable]
    internal class CodePageDataItem
    {
        internal string m_bodyName;
        internal int m_codePage;
        internal int m_dataIndex;
        internal string m_description;
        internal uint m_flags;
        internal string m_headerName;
        internal int m_uiFamilyCodePage;
        internal string m_webName;

        internal unsafe CodePageDataItem(int dataIndex)
        {
            this.m_dataIndex = dataIndex;
            this.m_codePage = 0;
            this.m_uiFamilyCodePage = EncodingTable.codePageDataPtr[dataIndex].uiFamilyCodePage;
            this.m_webName = null;
            this.m_headerName = null;
            this.m_bodyName = null;
            this.m_description = null;
            this.m_flags = EncodingTable.codePageDataPtr[dataIndex].flags;
        }

        public virtual string BodyName
        {
            get
            {
                if (this.m_bodyName == null)
                {
                    this.m_bodyName = new string(EncodingTable.codePageDataPtr[this.m_dataIndex].bodyName);
                }
                return this.m_bodyName;
            }
        }

        public virtual uint Flags
        {
            get
            {
                return this.m_flags;
            }
        }

        public virtual string HeaderName
        {
            get
            {
                if (this.m_headerName == null)
                {
                    this.m_headerName = new string(EncodingTable.codePageDataPtr[this.m_dataIndex].headerName);
                }
                return this.m_headerName;
            }
        }

        public virtual int UIFamilyCodePage
        {
            get
            {
                return this.m_uiFamilyCodePage;
            }
        }

        public virtual string WebName
        {
            get
            {
                if (this.m_webName == null)
                {
                    this.m_webName = new string(EncodingTable.codePageDataPtr[this.m_dataIndex].webName);
                }
                return this.m_webName;
            }
        }
    }
}

