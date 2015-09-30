namespace UnityEditor
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    [CanEditMultipleObjects, CustomEditor(typeof(PlatformEffector2D), true)]
    internal class PlatformEffector2DEditor : Effector2DEditor
    {
        [CompilerGenerated]
        private static Func<Collider2D, bool> <>f__am$cache0;

        public void OnSceneGUI()
        {
            PlatformEffector2D target = (PlatformEffector2D) this.target;
            if (target.enabled)
            {
                float f = 0.01745329f * ((target.surfaceArc * 0.5f) + target.transform.eulerAngles.z);
                float angle = Mathf.Clamp(target.surfaceArc, 0.5f, 360f);
                if (<>f__am$cache0 == null)
                {
                    <>f__am$cache0 = collider => collider.enabled && collider.usedByEffector;
                }
                IEnumerator<Collider2D> enumerator = target.gameObject.GetComponents<Collider2D>().Where<Collider2D>(<>f__am$cache0).GetEnumerator();
                try
                {
                    while (enumerator.MoveNext())
                    {
                        Collider2D current = enumerator.Current;
                        Vector3 center = current.bounds.center;
                        float radius = HandleUtility.GetHandleSize(center) * 0.8f;
                        Vector3 from = new Vector3(-Mathf.Sin(f), Mathf.Cos(f), 0f);
                        Handles.color = new Color(0f, 1f, 1f, 0.03f);
                        Handles.DrawSolidArc(center, Vector3.back, from, angle, radius);
                        Handles.color = new Color(0f, 1f, 1f, 0.7f);
                        Handles.DrawWireArc(center, Vector3.back, from, angle, radius);
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
}

