namespace UnityEditor.VersionControl
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    public class AssetList : List<Asset>
    {
        public AssetList()
        {
        }

        public AssetList(AssetList src)
        {
        }

        public AssetList Filter(bool includeFolder, params Asset.States[] states)
        {
            AssetList list = new AssetList();
            if (includeFolder || ((states != null) && (states.Length != 0)))
            {
                foreach (Asset asset in this)
                {
                    if (asset.isFolder)
                    {
                        if (includeFolder)
                        {
                            list.Add(asset);
                        }
                    }
                    else if (asset.IsOneOfStates(states))
                    {
                        list.Add(asset);
                    }
                }
            }
            return list;
        }

        public AssetList FilterChildren()
        {
            AssetList list = new AssetList();
            list.AddRange(this);
            <FilterChildren>c__AnonStoreyA8 ya = new <FilterChildren>c__AnonStoreyA8();
            using (List<Asset>.Enumerator enumerator = base.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    ya.asset = enumerator.Current;
                    list.RemoveAll(new Predicate<Asset>(ya.<>m__1F5));
                }
            }
            return list;
        }

        public int FilterCount(bool includeFolder, params Asset.States[] states)
        {
            int num = 0;
            if (!includeFolder && (states == null))
            {
                return this.Count;
            }
            foreach (Asset asset in this)
            {
                if (asset.isFolder)
                {
                    num++;
                }
                else if (asset.IsOneOfStates(states))
                {
                    num++;
                }
            }
            return num;
        }

        [CompilerGenerated]
        private sealed class <FilterChildren>c__AnonStoreyA8
        {
            internal Asset asset;

            internal bool <>m__1F5(Asset p)
            {
                return p.IsChildOf(this.asset);
            }
        }
    }
}

