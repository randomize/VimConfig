namespace UnityEngine.UI.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    internal class IndexedSet<T> : IList<T>, ICollection<T>, IEnumerable<T>, IEnumerable
    {
        private Dictionary<T, int> m_Dictionary;
        private readonly List<T> m_List;

        public IndexedSet()
        {
            this.m_List = new List<T>();
            this.m_Dictionary = new Dictionary<T, int>();
        }

        public void Add(T item)
        {
            if (!this.m_Dictionary.ContainsKey(item))
            {
                this.m_List.Add(item);
                this.m_Dictionary.Add(item, this.m_List.Count - 1);
            }
        }

        public void Clear()
        {
            this.m_List.Clear();
            this.m_Dictionary.Clear();
        }

        public bool Contains(T item)
        {
            return this.m_Dictionary.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.m_List.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public int IndexOf(T item)
        {
            int num = -1;
            this.m_Dictionary.TryGetValue(item, out num);
            return num;
        }

        public void Insert(int index, T item)
        {
            throw new NotSupportedException("Random Insertion is semantically invalid, since this structure does not guarantee ordering.");
        }

        public bool Remove(T item)
        {
            int num = -1;
            if (!this.m_Dictionary.TryGetValue(item, out num))
            {
                return false;
            }
            this.RemoveAt(num);
            return true;
        }

        public void RemoveAll(Predicate<T> match)
        {
            int num = 0;
            while (num < this.m_List.Count)
            {
                T local = this.m_List[num];
                if (match(local))
                {
                    this.Remove(local);
                }
                else
                {
                    num++;
                }
            }
        }

        public void RemoveAt(int index)
        {
            T key = this.m_List[index];
            this.m_Dictionary.Remove(key);
            if (index == (this.m_List.Count - 1))
            {
                this.m_List.RemoveAt(index);
            }
            else
            {
                int num = this.m_List.Count - 1;
                T local2 = this.m_List[num];
                this.m_List[index] = local2;
                this.m_Dictionary[local2] = index;
                this.m_List.RemoveAt(num);
            }
        }

        public void Sort(Comparison<T> sortLayoutFunction)
        {
            this.m_List.Sort(sortLayoutFunction);
            for (int i = 0; i < this.m_List.Count; i++)
            {
                T local = this.m_List[i];
                this.m_Dictionary[local] = i;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public int Count
        {
            get
            {
                return this.m_List.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public T this[int index]
        {
            get
            {
                return this.m_List[index];
            }
            set
            {
                T key = this.m_List[index];
                this.m_Dictionary.Remove(key);
                this.m_List[index] = value;
                this.m_Dictionary.Add(key, index);
            }
        }
    }
}

