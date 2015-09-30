namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("UI/Effects/Outline", 15)]
    public class Outline : Shadow
    {
        protected Outline()
        {
        }

        public override void ModifyMesh(Mesh mesh)
        {
            if (this.IsActive())
            {
                List<UIVertex> stream = new List<UIVertex>();
                using (VertexHelper helper = new VertexHelper(mesh))
                {
                    helper.GetUIVertexStream(stream);
                }
                int num = stream.Count * 5;
                if (stream.Capacity < num)
                {
                    stream.Capacity = num;
                }
                int start = 0;
                int count = stream.Count;
                base.ApplyShadowZeroAlloc(stream, base.effectColor, start, stream.Count, base.effectDistance.x, base.effectDistance.y);
                start = count;
                count = stream.Count;
                base.ApplyShadowZeroAlloc(stream, base.effectColor, start, stream.Count, base.effectDistance.x, -base.effectDistance.y);
                start = count;
                count = stream.Count;
                base.ApplyShadowZeroAlloc(stream, base.effectColor, start, stream.Count, -base.effectDistance.x, base.effectDistance.y);
                start = count;
                count = stream.Count;
                base.ApplyShadowZeroAlloc(stream, base.effectColor, start, stream.Count, -base.effectDistance.x, -base.effectDistance.y);
                using (VertexHelper helper2 = new VertexHelper())
                {
                    helper2.AddUIVertexTriangleStream(stream);
                    helper2.FillMesh(mesh);
                }
            }
        }
    }
}

