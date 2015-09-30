namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;

    [AddComponentMenu("UI/Raw Image", 12)]
    public class RawImage : MaskableGraphic
    {
        [FormerlySerializedAs("m_Tex"), SerializeField]
        private Texture m_Texture;
        [SerializeField]
        private Rect m_UVRect = new Rect(0f, 0f, 1f, 1f);

        protected RawImage()
        {
        }

        protected override void OnPopulateMesh(Mesh toFill)
        {
            Texture mainTexture = this.mainTexture;
            if (mainTexture != null)
            {
                Vector4 zero = Vector4.zero;
                int num = Mathf.RoundToInt(mainTexture.width * this.uvRect.width);
                int num2 = Mathf.RoundToInt(mainTexture.height * this.uvRect.height);
                float num3 = ((num & 1) != 0) ? ((float) (num + 1)) : ((float) num);
                float num4 = ((num2 & 1) != 0) ? ((float) (num2 + 1)) : ((float) num2);
                zero.x = 0f;
                zero.y = 0f;
                zero.z = ((float) num) / num3;
                zero.w = ((float) num2) / num4;
                zero.x -= base.rectTransform.pivot.x;
                zero.y -= base.rectTransform.pivot.y;
                zero.z -= base.rectTransform.pivot.x;
                zero.w -= base.rectTransform.pivot.y;
                zero.x *= base.rectTransform.rect.width;
                zero.y *= base.rectTransform.rect.height;
                zero.z *= base.rectTransform.rect.width;
                zero.w *= base.rectTransform.rect.height;
                using (VertexHelper helper = new VertexHelper())
                {
                    Color color = base.color;
                    helper.AddVert(new Vector3(zero.x, zero.y), color, new Vector2(this.m_UVRect.xMin, this.m_UVRect.yMin));
                    helper.AddVert(new Vector3(zero.x, zero.w), color, new Vector2(this.m_UVRect.xMin, this.m_UVRect.yMax));
                    helper.AddVert(new Vector3(zero.z, zero.w), color, new Vector2(this.m_UVRect.xMax, this.m_UVRect.yMax));
                    helper.AddVert(new Vector3(zero.z, zero.y), color, new Vector2(this.m_UVRect.xMax, this.m_UVRect.yMin));
                    helper.AddTriangle(0, 1, 2);
                    helper.AddTriangle(2, 3, 0);
                    helper.FillMesh(toFill);
                }
            }
        }

        public override void SetNativeSize()
        {
            Texture mainTexture = this.mainTexture;
            if (mainTexture != null)
            {
                int num = Mathf.RoundToInt(mainTexture.width * this.uvRect.width);
                int num2 = Mathf.RoundToInt(mainTexture.height * this.uvRect.height);
                base.rectTransform.anchorMax = base.rectTransform.anchorMin;
                base.rectTransform.sizeDelta = new Vector2((float) num, (float) num2);
            }
        }

        public override Texture mainTexture
        {
            get
            {
                return ((this.m_Texture != null) ? this.m_Texture : Graphic.s_WhiteTexture);
            }
        }

        public Texture texture
        {
            get
            {
                return this.m_Texture;
            }
            set
            {
                if (this.m_Texture != value)
                {
                    this.m_Texture = value;
                    this.SetVerticesDirty();
                    this.SetMaterialDirty();
                }
            }
        }

        public Rect uvRect
        {
            get
            {
                return this.m_UVRect;
            }
            set
            {
                if (this.m_UVRect != value)
                {
                    this.m_UVRect = value;
                    this.SetVerticesDirty();
                }
            }
        }
    }
}

