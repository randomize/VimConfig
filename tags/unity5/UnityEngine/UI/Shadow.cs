namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [AddComponentMenu("UI/Effects/Shadow", 14)]
    public class Shadow : BaseMeshEffect
    {
        [SerializeField]
        private Color m_EffectColor = new Color(0f, 0f, 0f, 0.5f);
        [SerializeField]
        private Vector2 m_EffectDistance = new Vector2(1f, -1f);
        [SerializeField]
        private bool m_UseGraphicAlpha = true;

        protected Shadow()
        {
        }

        protected void ApplyShadow(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            int num = verts.Count * 2;
            if (verts.Capacity < num)
            {
                verts.Capacity = num;
            }
            this.ApplyShadowZeroAlloc(verts, color, start, end, x, y);
        }

        protected void ApplyShadowZeroAlloc(List<UIVertex> verts, Color32 color, int start, int end, float x, float y)
        {
            int num = verts.Count * 2;
            if (verts.Capacity < num)
            {
                verts.Capacity = num;
            }
            for (int i = start; i < end; i++)
            {
                UIVertex item = verts[i];
                verts.Add(item);
                Vector3 position = item.position;
                position.x += x;
                position.y += y;
                item.position = position;
                Color32 color2 = color;
                if (this.m_UseGraphicAlpha)
                {
                    UIVertex vertex2 = verts[i];
                    color2.a = (byte) ((color2.a * vertex2.color.a) / 0xff);
                }
                item.color = color2;
                verts[i] = item;
            }
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
                this.ApplyShadow(stream, this.effectColor, 0, stream.Count, this.effectDistance.x, this.effectDistance.y);
                using (VertexHelper helper2 = new VertexHelper())
                {
                    helper2.AddUIVertexTriangleStream(stream);
                    helper2.FillMesh(mesh);
                }
            }
        }

        protected override void OnValidate()
        {
            this.effectDistance = this.m_EffectDistance;
            base.OnValidate();
        }

        public Color effectColor
        {
            get
            {
                return this.m_EffectColor;
            }
            set
            {
                this.m_EffectColor = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }

        public Vector2 effectDistance
        {
            get
            {
                return this.m_EffectDistance;
            }
            set
            {
                if (value.x > 600f)
                {
                    value.x = 600f;
                }
                if (value.x < -600f)
                {
                    value.x = -600f;
                }
                if (value.y > 600f)
                {
                    value.y = 600f;
                }
                if (value.y < -600f)
                {
                    value.y = -600f;
                }
                if (this.m_EffectDistance != value)
                {
                    this.m_EffectDistance = value;
                    if (base.graphic != null)
                    {
                        base.graphic.SetVerticesDirty();
                    }
                }
            }
        }

        public bool useGraphicAlpha
        {
            get
            {
                return this.m_UseGraphicAlpha;
            }
            set
            {
                this.m_UseGraphicAlpha = value;
                if (base.graphic != null)
                {
                    base.graphic.SetVerticesDirty();
                }
            }
        }
    }
}

