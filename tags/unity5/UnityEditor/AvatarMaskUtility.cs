namespace UnityEditor
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEditor.Animations;

    internal class AvatarMaskUtility
    {
        private static string sBoneName = "m_BoneName";
        private static string sHuman = "m_HumanDescription.m_Human";

        public static string[] GetAvatarHumanTransform(SerializedObject so, string[] refTransformsPath)
        {
            SerializedProperty property = so.FindProperty(sHuman);
            if ((property == null) || !property.isArray)
            {
                return null;
            }
            string[] array = new string[0];
            for (int i = 0; i < property.arraySize; i++)
            {
                SerializedProperty property2 = property.GetArrayElementAtIndex(i).FindPropertyRelative(sBoneName);
                ArrayUtility.Add<string>(ref array, property2.stringValue);
            }
            return TokeniseHumanTransformsPath(refTransformsPath, array);
        }

        public static void SetActiveHumanTransforms(AvatarMask mask, string[] humanTransforms)
        {
            for (int i = 0; i < mask.transformCount; i++)
            {
                <SetActiveHumanTransforms>c__AnonStorey7A storeya = new <SetActiveHumanTransforms>c__AnonStorey7A {
                    path = mask.GetTransformPath(i)
                };
                if (ArrayUtility.FindIndex<string>(humanTransforms, new Predicate<string>(storeya.<>m__124)) != -1)
                {
                    mask.SetTransformActive(i, true);
                }
            }
        }

        private static string[] TokeniseHumanTransformsPath(string[] refTransformsPath, string[] humanTransforms)
        {
            <TokeniseHumanTransformsPath>c__AnonStorey7B storeyb = new <TokeniseHumanTransformsPath>c__AnonStorey7B {
                humanTransforms = humanTransforms
            };
            if (storeyb.humanTransforms == null)
            {
                return null;
            }
            string[] array = new string[] { string.Empty };
            <TokeniseHumanTransformsPath>c__AnonStorey7C storeyc = new <TokeniseHumanTransformsPath>c__AnonStorey7C {
                <>f__ref$123 = storeyb,
                i = 0
            };
            while (storeyc.i < storeyb.humanTransforms.Length)
            {
                int index = ArrayUtility.FindIndex<string>(refTransformsPath, new Predicate<string>(storeyc.<>m__125));
                if (index != -1)
                {
                    <TokeniseHumanTransformsPath>c__AnonStorey7D storeyd = new <TokeniseHumanTransformsPath>c__AnonStorey7D();
                    int length = array.Length;
                    storeyd.path = refTransformsPath[index];
                    while (storeyd.path.Length > 0)
                    {
                        if (ArrayUtility.FindIndex<string>(array, new Predicate<string>(storeyd.<>m__126)) == -1)
                        {
                            ArrayUtility.Insert<string>(ref array, length, storeyd.path);
                        }
                        int num4 = storeyd.path.LastIndexOf('/');
                        storeyd.path = storeyd.path.Substring(0, (num4 == -1) ? 0 : num4);
                    }
                }
                storeyc.i++;
            }
            return array;
        }

        public static void UpdateTransformMask(AvatarMask mask, string[] refTransformsPath, string[] humanTransforms)
        {
            <UpdateTransformMask>c__AnonStorey78 storey = new <UpdateTransformMask>c__AnonStorey78 {
                refTransformsPath = refTransformsPath
            };
            mask.transformCount = storey.refTransformsPath.Length;
            <UpdateTransformMask>c__AnonStorey79 storey2 = new <UpdateTransformMask>c__AnonStorey79 {
                <>f__ref$120 = storey,
                i = 0
            };
            while (storey2.i < storey.refTransformsPath.Length)
            {
                mask.SetTransformPath(storey2.i, storey.refTransformsPath[storey2.i]);
                bool flag = (humanTransforms != null) ? (ArrayUtility.FindIndex<string>(humanTransforms, new Predicate<string>(storey2.<>m__123)) != -1) : true;
                mask.SetTransformActive(storey2.i, flag);
                storey2.i++;
            }
        }

        [CompilerGenerated]
        private sealed class <SetActiveHumanTransforms>c__AnonStorey7A
        {
            internal string path;

            internal bool <>m__124(string s)
            {
                return (this.path == s);
            }
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey7B
        {
            internal string[] humanTransforms;
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey7C
        {
            internal AvatarMaskUtility.<TokeniseHumanTransformsPath>c__AnonStorey7B <>f__ref$123;
            internal int i;

            internal bool <>m__125(string s)
            {
                return (this.<>f__ref$123.humanTransforms[this.i] == FileUtil.GetLastPathNameComponent(s));
            }
        }

        [CompilerGenerated]
        private sealed class <TokeniseHumanTransformsPath>c__AnonStorey7D
        {
            internal string path;

            internal bool <>m__126(string s)
            {
                return (this.path == s);
            }
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey78
        {
            internal string[] refTransformsPath;
        }

        [CompilerGenerated]
        private sealed class <UpdateTransformMask>c__AnonStorey79
        {
            internal AvatarMaskUtility.<UpdateTransformMask>c__AnonStorey78 <>f__ref$120;
            internal int i;

            internal bool <>m__123(string s)
            {
                return (this.<>f__ref$120.refTransformsPath[this.i] == s);
            }
        }
    }
}

