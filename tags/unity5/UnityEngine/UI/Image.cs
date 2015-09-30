namespace UnityEngine.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Serialization;
    using UnityEngine.Sprites;

    [AddComponentMenu("UI/Image", 11)]
    public class Image : MaskableGraphic, ICanvasRaycastFilter, ISerializationCallbackReceiver, ILayoutElement
    {
        private float m_EventAlphaThreshold = 1f;
        [SerializeField, Range(0f, 1f)]
        private float m_FillAmount = 1f;
        [SerializeField]
        private bool m_FillCenter = true;
        [SerializeField]
        private bool m_FillClockwise = true;
        [SerializeField]
        private FillMethod m_FillMethod = FillMethod.Radial360;
        [SerializeField]
        private int m_FillOrigin;
        [NonSerialized]
        private Sprite m_OverrideSprite;
        [SerializeField]
        private bool m_PreserveAspect;
        [FormerlySerializedAs("m_Frame"), SerializeField]
        private Sprite m_Sprite;
        [SerializeField]
        private Type m_Type;
        private static readonly Vector3[] s_Uv = new Vector3[4];
        private static readonly Vector2[] s_UVScratch = new Vector2[4];
        private static readonly Vector2[] s_VertScratch = new Vector2[4];
        private static readonly Vector3[] s_Xy = new Vector3[4];

        protected Image()
        {
        }

        private static void AddQuad(VertexHelper vh, Vector3[] quadPositions, Color32 color, Vector3[] quadUVs)
        {
            int currentVertCount = vh.currentVertCount;
            for (int i = 0; i < 4; i++)
            {
                vh.AddVert(quadPositions[i], color, quadUVs[i]);
            }
            vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            vh.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }

        private static void AddQuad(VertexHelper vh, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            int currentVertCount = vh.currentVertCount;
            vh.AddVert(new Vector3(posMin.x, posMin.y, 0f), color, new Vector2(uvMin.x, uvMin.y));
            vh.AddVert(new Vector3(posMin.x, posMax.y, 0f), color, new Vector2(uvMin.x, uvMax.y));
            vh.AddVert(new Vector3(posMax.x, posMax.y, 0f), color, new Vector2(uvMax.x, uvMax.y));
            vh.AddVert(new Vector3(posMax.x, posMin.y, 0f), color, new Vector2(uvMax.x, uvMin.y));
            vh.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            vh.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }

        public virtual void CalculateLayoutInputHorizontal()
        {
        }

        public virtual void CalculateLayoutInputVertical()
        {
        }

        private void GenerateFilledSprite(Mesh toFill, bool preserveAspect)
        {
            if (this.m_FillAmount < 0.001f)
            {
                using (VertexHelper helper = new VertexHelper())
                {
                    helper.FillMesh(toFill);
                    return;
                }
            }
            Vector4 drawingDimensions = this.GetDrawingDimensions(preserveAspect);
            Vector4 vector2 = (this.overrideSprite == null) ? Vector4.zero : DataUtility.GetOuterUV(this.overrideSprite);
            UIVertex.simpleVert.color = base.color;
            float x = vector2.x;
            float y = vector2.y;
            float z = vector2.z;
            float w = vector2.w;
            if ((this.m_FillMethod == FillMethod.Horizontal) || (this.m_FillMethod == FillMethod.Vertical))
            {
                if (this.fillMethod == FillMethod.Horizontal)
                {
                    float num5 = (z - x) * this.m_FillAmount;
                    if (this.m_FillOrigin == 1)
                    {
                        drawingDimensions.x = drawingDimensions.z - ((drawingDimensions.z - drawingDimensions.x) * this.m_FillAmount);
                        x = z - num5;
                    }
                    else
                    {
                        drawingDimensions.z = drawingDimensions.x + ((drawingDimensions.z - drawingDimensions.x) * this.m_FillAmount);
                        z = x + num5;
                    }
                }
                else if (this.fillMethod == FillMethod.Vertical)
                {
                    float num6 = (w - y) * this.m_FillAmount;
                    if (this.m_FillOrigin == 1)
                    {
                        drawingDimensions.y = drawingDimensions.w - ((drawingDimensions.w - drawingDimensions.y) * this.m_FillAmount);
                        y = w - num6;
                    }
                    else
                    {
                        drawingDimensions.w = drawingDimensions.y + ((drawingDimensions.w - drawingDimensions.y) * this.m_FillAmount);
                        w = y + num6;
                    }
                }
            }
            s_Xy[0] = (Vector3) new Vector2(drawingDimensions.x, drawingDimensions.y);
            s_Xy[1] = (Vector3) new Vector2(drawingDimensions.x, drawingDimensions.w);
            s_Xy[2] = (Vector3) new Vector2(drawingDimensions.z, drawingDimensions.w);
            s_Xy[3] = (Vector3) new Vector2(drawingDimensions.z, drawingDimensions.y);
            s_Uv[0] = (Vector3) new Vector2(x, y);
            s_Uv[1] = (Vector3) new Vector2(x, w);
            s_Uv[2] = (Vector3) new Vector2(z, w);
            s_Uv[3] = (Vector3) new Vector2(z, y);
            using (VertexHelper helper2 = new VertexHelper())
            {
                if (((this.m_FillAmount < 1f) && (this.m_FillMethod != FillMethod.Horizontal)) && (this.m_FillMethod != FillMethod.Vertical))
                {
                    if (this.fillMethod == FillMethod.Radial90)
                    {
                        if (RadialCut(s_Xy, s_Uv, this.m_FillAmount, this.m_FillClockwise, this.m_FillOrigin))
                        {
                            AddQuad(helper2, s_Xy, base.color, s_Uv);
                        }
                    }
                    else if (this.fillMethod == FillMethod.Radial180)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            float num8;
                            float num9;
                            float num10;
                            float num11;
                            int num12 = (this.m_FillOrigin <= 1) ? 0 : 1;
                            if ((this.m_FillOrigin == 0) || (this.m_FillOrigin == 2))
                            {
                                num10 = 0f;
                                num11 = 1f;
                                if (i == num12)
                                {
                                    num8 = 0f;
                                    num9 = 0.5f;
                                }
                                else
                                {
                                    num8 = 0.5f;
                                    num9 = 1f;
                                }
                            }
                            else
                            {
                                num8 = 0f;
                                num9 = 1f;
                                if (i == num12)
                                {
                                    num10 = 0.5f;
                                    num11 = 1f;
                                }
                                else
                                {
                                    num10 = 0f;
                                    num11 = 0.5f;
                                }
                            }
                            s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num8);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num9);
                            s_Xy[3].x = s_Xy[2].x;
                            s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num10);
                            s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num11);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;
                            s_Uv[0].x = Mathf.Lerp(x, z, num8);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.Lerp(x, z, num9);
                            s_Uv[3].x = s_Uv[2].x;
                            s_Uv[0].y = Mathf.Lerp(y, w, num10);
                            s_Uv[1].y = Mathf.Lerp(y, w, num11);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;
                            float num13 = !this.m_FillClockwise ? ((this.m_FillAmount * 2f) - (1 - i)) : ((this.fillAmount * 2f) - i);
                            if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num13), this.m_FillClockwise, ((i + this.m_FillOrigin) + 3) % 4))
                            {
                                AddQuad(helper2, s_Xy, base.color, s_Uv);
                            }
                        }
                    }
                    else if (this.fillMethod == FillMethod.Radial360)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            float num15;
                            float num16;
                            float num17;
                            float num18;
                            if (j < 2)
                            {
                                num15 = 0f;
                                num16 = 0.5f;
                            }
                            else
                            {
                                num15 = 0.5f;
                                num16 = 1f;
                            }
                            switch (j)
                            {
                                case 0:
                                case 3:
                                    num17 = 0f;
                                    num18 = 0.5f;
                                    break;

                                default:
                                    num17 = 0.5f;
                                    num18 = 1f;
                                    break;
                            }
                            s_Xy[0].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num15);
                            s_Xy[1].x = s_Xy[0].x;
                            s_Xy[2].x = Mathf.Lerp(drawingDimensions.x, drawingDimensions.z, num16);
                            s_Xy[3].x = s_Xy[2].x;
                            s_Xy[0].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num17);
                            s_Xy[1].y = Mathf.Lerp(drawingDimensions.y, drawingDimensions.w, num18);
                            s_Xy[2].y = s_Xy[1].y;
                            s_Xy[3].y = s_Xy[0].y;
                            s_Uv[0].x = Mathf.Lerp(x, z, num15);
                            s_Uv[1].x = s_Uv[0].x;
                            s_Uv[2].x = Mathf.Lerp(x, z, num16);
                            s_Uv[3].x = s_Uv[2].x;
                            s_Uv[0].y = Mathf.Lerp(y, w, num17);
                            s_Uv[1].y = Mathf.Lerp(y, w, num18);
                            s_Uv[2].y = s_Uv[1].y;
                            s_Uv[3].y = s_Uv[0].y;
                            float num19 = !this.m_FillClockwise ? ((this.m_FillAmount * 4f) - (3 - ((j + this.m_FillOrigin) % 4))) : ((this.m_FillAmount * 4f) - ((j + this.m_FillOrigin) % 4));
                            if (RadialCut(s_Xy, s_Uv, Mathf.Clamp01(num19), this.m_FillClockwise, (j + 2) % 4))
                            {
                                AddQuad(helper2, s_Xy, base.color, s_Uv);
                            }
                        }
                    }
                }
                else
                {
                    AddQuad(helper2, s_Xy, base.color, s_Uv);
                }
                helper2.FillMesh(toFill);
            }
        }

        private void GenerateSimpleSprite(Mesh toFill, bool lPreserveAspect)
        {
            Vector4 drawingDimensions = this.GetDrawingDimensions(lPreserveAspect);
            Vector4 vector2 = (this.overrideSprite == null) ? Vector4.zero : DataUtility.GetOuterUV(this.overrideSprite);
            Color color = base.color;
            using (VertexHelper helper = new VertexHelper())
            {
                helper.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.y), color, new Vector2(vector2.x, vector2.y));
                helper.AddVert(new Vector3(drawingDimensions.x, drawingDimensions.w), color, new Vector2(vector2.x, vector2.w));
                helper.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.w), color, new Vector2(vector2.z, vector2.w));
                helper.AddVert(new Vector3(drawingDimensions.z, drawingDimensions.y), color, new Vector2(vector2.z, vector2.y));
                helper.AddTriangle(0, 1, 2);
                helper.AddTriangle(2, 3, 0);
                helper.FillMesh(toFill);
            }
        }

        private void GenerateSlicedSprite(Mesh toFill)
        {
            if (!this.hasBorder)
            {
                this.GenerateSimpleSprite(toFill, false);
            }
            else
            {
                Vector4 outerUV;
                Vector4 innerUV;
                Vector4 padding;
                Vector4 adjustedBorders;
                if (this.overrideSprite != null)
                {
                    outerUV = DataUtility.GetOuterUV(this.overrideSprite);
                    innerUV = DataUtility.GetInnerUV(this.overrideSprite);
                    padding = DataUtility.GetPadding(this.overrideSprite);
                    adjustedBorders = this.overrideSprite.border;
                }
                else
                {
                    outerUV = Vector4.zero;
                    innerUV = Vector4.zero;
                    padding = Vector4.zero;
                    adjustedBorders = Vector4.zero;
                }
                Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
                adjustedBorders = this.GetAdjustedBorders((Vector4) (adjustedBorders / this.pixelsPerUnit), pixelAdjustedRect);
                padding = (Vector4) (padding / this.pixelsPerUnit);
                s_VertScratch[0] = new Vector2(padding.x, padding.y);
                s_VertScratch[3] = new Vector2(pixelAdjustedRect.width - padding.z, pixelAdjustedRect.height - padding.w);
                s_VertScratch[1].x = adjustedBorders.x;
                s_VertScratch[1].y = adjustedBorders.y;
                s_VertScratch[2].x = pixelAdjustedRect.width - adjustedBorders.z;
                s_VertScratch[2].y = pixelAdjustedRect.height - adjustedBorders.w;
                for (int i = 0; i < 4; i++)
                {
                    s_VertScratch[i].x += pixelAdjustedRect.x;
                    s_VertScratch[i].y += pixelAdjustedRect.y;
                }
                s_UVScratch[0] = new Vector2(outerUV.x, outerUV.y);
                s_UVScratch[1] = new Vector2(innerUV.x, innerUV.y);
                s_UVScratch[2] = new Vector2(innerUV.z, innerUV.w);
                s_UVScratch[3] = new Vector2(outerUV.z, outerUV.w);
                using (VertexHelper helper = new VertexHelper())
                {
                    for (int j = 0; j < 3; j++)
                    {
                        int index = j + 1;
                        for (int k = 0; k < 3; k++)
                        {
                            if ((this.m_FillCenter || (j != 1)) || (k != 1))
                            {
                                int num5 = k + 1;
                                AddQuad(helper, new Vector2(s_VertScratch[j].x, s_VertScratch[k].y), new Vector2(s_VertScratch[index].x, s_VertScratch[num5].y), base.color, new Vector2(s_UVScratch[j].x, s_UVScratch[k].y), new Vector2(s_UVScratch[index].x, s_UVScratch[num5].y));
                            }
                        }
                    }
                    helper.FillMesh(toFill);
                }
            }
        }

        private void GenerateTiledSprite(Mesh toFill)
        {
            Vector4 outerUV;
            Vector4 innerUV;
            Vector4 adjustedBorders;
            Vector2 size;
            if (this.overrideSprite != null)
            {
                outerUV = DataUtility.GetOuterUV(this.overrideSprite);
                innerUV = DataUtility.GetInnerUV(this.overrideSprite);
                adjustedBorders = this.overrideSprite.border;
                size = this.overrideSprite.rect.size;
            }
            else
            {
                outerUV = Vector4.zero;
                innerUV = Vector4.zero;
                adjustedBorders = Vector4.zero;
                size = (Vector2) (Vector2.one * 100f);
            }
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            float num = ((size.x - adjustedBorders.x) - adjustedBorders.z) / this.pixelsPerUnit;
            float num2 = ((size.y - adjustedBorders.y) - adjustedBorders.w) / this.pixelsPerUnit;
            adjustedBorders = this.GetAdjustedBorders((Vector4) (adjustedBorders / this.pixelsPerUnit), pixelAdjustedRect);
            Vector2 uvMin = new Vector2(innerUV.x, innerUV.y);
            Vector2 vector6 = new Vector2(innerUV.z, innerUV.w);
            UIVertex.simpleVert.color = base.color;
            float x = adjustedBorders.x;
            float num4 = pixelAdjustedRect.width - adjustedBorders.z;
            float y = adjustedBorders.y;
            float num6 = pixelAdjustedRect.height - adjustedBorders.w;
            if (((num4 - x) > (num * 100f)) || ((num6 - y) > (num2 * 100f)))
            {
                num = (num4 - x) / 100f;
                num2 = (num6 - y) / 100f;
            }
            using (VertexHelper helper = new VertexHelper())
            {
                Vector2 uvMax = vector6;
                if (this.m_FillCenter)
                {
                    for (float i = y; i < num6; i += num2)
                    {
                        float num8 = i + num2;
                        if (num8 > num6)
                        {
                            uvMax.y = uvMin.y + (((vector6.y - uvMin.y) * (num6 - i)) / (num8 - i));
                            num8 = num6;
                        }
                        uvMax.x = vector6.x;
                        for (float j = x; j < num4; j += num)
                        {
                            float num10 = j + num;
                            if (num10 > num4)
                            {
                                uvMax.x = uvMin.x + (((vector6.x - uvMin.x) * (num4 - j)) / (num10 - j));
                                num10 = num4;
                            }
                            AddQuad(helper, new Vector2(j, i) + pixelAdjustedRect.position, new Vector2(num10, num8) + pixelAdjustedRect.position, base.color, uvMin, uvMax);
                        }
                    }
                }
                if (this.hasBorder)
                {
                    uvMax = vector6;
                    for (float k = y; k < num6; k += num2)
                    {
                        float num12 = k + num2;
                        if (num12 > num6)
                        {
                            uvMax.y = uvMin.y + (((vector6.y - uvMin.y) * (num6 - k)) / (num12 - k));
                            num12 = num6;
                        }
                        AddQuad(helper, new Vector2(0f, k) + pixelAdjustedRect.position, new Vector2(x, num12) + pixelAdjustedRect.position, base.color, new Vector2(outerUV.x, uvMin.y), new Vector2(uvMin.x, uvMax.y));
                        AddQuad(helper, new Vector2(num4, k) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, num12) + pixelAdjustedRect.position, base.color, new Vector2(vector6.x, uvMin.y), new Vector2(outerUV.z, uvMax.y));
                    }
                    uvMax = vector6;
                    for (float m = x; m < num4; m += num)
                    {
                        float num14 = m + num;
                        if (num14 > num4)
                        {
                            uvMax.x = uvMin.x + (((vector6.x - uvMin.x) * (num4 - m)) / (num14 - m));
                            num14 = num4;
                        }
                        AddQuad(helper, new Vector2(m, 0f) + pixelAdjustedRect.position, new Vector2(num14, y) + pixelAdjustedRect.position, base.color, new Vector2(uvMin.x, outerUV.y), new Vector2(uvMax.x, uvMin.y));
                        AddQuad(helper, new Vector2(m, num6) + pixelAdjustedRect.position, new Vector2(num14, pixelAdjustedRect.height) + pixelAdjustedRect.position, base.color, new Vector2(uvMin.x, vector6.y), new Vector2(uvMax.x, outerUV.w));
                    }
                    AddQuad(helper, new Vector2(0f, 0f) + pixelAdjustedRect.position, new Vector2(x, y) + pixelAdjustedRect.position, base.color, new Vector2(outerUV.x, outerUV.y), new Vector2(uvMin.x, uvMin.y));
                    AddQuad(helper, new Vector2(num4, 0f) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, y) + pixelAdjustedRect.position, base.color, new Vector2(vector6.x, outerUV.y), new Vector2(outerUV.z, uvMin.y));
                    AddQuad(helper, new Vector2(0f, num6) + pixelAdjustedRect.position, new Vector2(x, pixelAdjustedRect.height) + pixelAdjustedRect.position, base.color, new Vector2(outerUV.x, vector6.y), new Vector2(uvMin.x, outerUV.w));
                    AddQuad(helper, new Vector2(num4, num6) + pixelAdjustedRect.position, new Vector2(pixelAdjustedRect.width, pixelAdjustedRect.height) + pixelAdjustedRect.position, base.color, new Vector2(vector6.x, vector6.y), new Vector2(outerUV.z, outerUV.w));
                }
                helper.FillMesh(toFill);
            }
        }

        private unsafe Vector4 GetAdjustedBorders(Vector4 border, Rect rect)
        {
            for (int i = 0; i <= 1; i++)
            {
                float num2 = border[i] + border[i + 2];
                if ((rect.size[i] < num2) && (num2 != 0f))
                {
                    ref Vector4 vectorRef;
                    int num4;
                    ref Vector4 vectorRef2;
                    float num3 = rect.size[i] / num2;
                    float num5 = vectorRef[num4];
                    (vectorRef = (Vector4) &border)[num4 = i] = num5 * num3;
                    num5 = vectorRef2[num4];
                    (vectorRef2 = (Vector4) &border)[num4 = i + 2] = num5 * num3;
                }
            }
            return border;
        }

        private Vector4 GetDrawingDimensions(bool shouldPreserveAspect)
        {
            Vector4 vector = (this.overrideSprite != null) ? DataUtility.GetPadding(this.overrideSprite) : Vector4.zero;
            Vector2 vector2 = (this.overrideSprite != null) ? new Vector2(this.overrideSprite.rect.width, this.overrideSprite.rect.height) : Vector2.zero;
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            int num = Mathf.RoundToInt(vector2.x);
            int num2 = Mathf.RoundToInt(vector2.y);
            Vector4 vector3 = new Vector4(vector.x / ((float) num), vector.y / ((float) num2), (num - vector.z) / ((float) num), (num2 - vector.w) / ((float) num2));
            if (shouldPreserveAspect && (vector2.sqrMagnitude > 0f))
            {
                float num3 = vector2.x / vector2.y;
                float num4 = pixelAdjustedRect.width / pixelAdjustedRect.height;
                if (num3 > num4)
                {
                    float height = pixelAdjustedRect.height;
                    pixelAdjustedRect.height = pixelAdjustedRect.width * (1f / num3);
                    pixelAdjustedRect.y += (height - pixelAdjustedRect.height) * base.rectTransform.pivot.y;
                }
                else
                {
                    float width = pixelAdjustedRect.width;
                    pixelAdjustedRect.width = pixelAdjustedRect.height * num3;
                    pixelAdjustedRect.x += (width - pixelAdjustedRect.width) * base.rectTransform.pivot.x;
                }
            }
            return new Vector4(pixelAdjustedRect.x + (pixelAdjustedRect.width * vector3.x), pixelAdjustedRect.y + (pixelAdjustedRect.height * vector3.y), pixelAdjustedRect.x + (pixelAdjustedRect.width * vector3.z), pixelAdjustedRect.y + (pixelAdjustedRect.height * vector3.w));
        }

        public virtual bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            Vector2 vector;
            if (this.m_EventAlphaThreshold >= 1f)
            {
                return true;
            }
            Sprite overrideSprite = this.overrideSprite;
            if (overrideSprite == null)
            {
                return true;
            }
            RectTransformUtility.ScreenPointToLocalPointInRectangle(base.rectTransform, screenPoint, eventCamera, out vector);
            Rect pixelAdjustedRect = base.GetPixelAdjustedRect();
            vector.x += base.rectTransform.pivot.x * pixelAdjustedRect.width;
            vector.y += base.rectTransform.pivot.y * pixelAdjustedRect.height;
            vector = this.MapCoordinate(vector, pixelAdjustedRect);
            Rect textureRect = overrideSprite.textureRect;
            Vector2 vector2 = new Vector2(vector.x / textureRect.width, vector.y / textureRect.height);
            float u = Mathf.Lerp(textureRect.x, textureRect.xMax, vector2.x) / ((float) overrideSprite.texture.width);
            float v = Mathf.Lerp(textureRect.y, textureRect.yMax, vector2.y) / ((float) overrideSprite.texture.height);
            try
            {
                return (overrideSprite.texture.GetPixelBilinear(u, v).a >= this.m_EventAlphaThreshold);
            }
            catch (UnityException exception)
            {
                Debug.LogError("Using clickAlphaThreshold lower than 1 on Image whose sprite texture cannot be read. " + exception.Message + " Also make sure to disable sprite packing for this sprite.", this);
                return true;
            }
        }

        private unsafe Vector2 MapCoordinate(Vector2 local, Rect rect)
        {
            Rect rect2 = this.sprite.rect;
            if ((this.type == Type.Simple) || (this.type == Type.Filled))
            {
                return new Vector2((local.x * rect2.width) / rect.width, (local.y * rect2.height) / rect.height);
            }
            Vector4 border = this.sprite.border;
            Vector4 adjustedBorders = this.GetAdjustedBorders((Vector4) (border / this.pixelsPerUnit), rect);
            for (int i = 0; i < 2; i++)
            {
                if (local[i] > adjustedBorders[i])
                {
                    int num3;
                    float num4;
                    if ((rect.size[i] - local[i]) <= adjustedBorders[i + 2])
                    {
                        ref Vector2 vectorRef;
                        num4 = vectorRef[num3];
                        (vectorRef = (Vector2) &local)[num3 = i] = num4 - (rect.size[i] - rect2.size[i]);
                    }
                    else if (this.type == Type.Sliced)
                    {
                        float t = Mathf.InverseLerp(adjustedBorders[i], rect.size[i] - adjustedBorders[i + 2], local[i]);
                        local[i] = Mathf.Lerp(border[i], rect2.size[i] - border[i + 2], t);
                    }
                    else
                    {
                        ref Vector2 vectorRef2;
                        ref Vector2 vectorRef3;
                        num4 = vectorRef2[num3];
                        (vectorRef2 = (Vector2) &local)[num3 = i] = num4 - adjustedBorders[i];
                        local[i] = Mathf.Repeat(local[i], (rect2.size[i] - border[i]) - border[i + 2]);
                        num4 = vectorRef3[num3];
                        (vectorRef3 = (Vector2) &local)[num3 = i] = num4 + border[i];
                    }
                }
            }
            return local;
        }

        public virtual void OnAfterDeserialize()
        {
            if (this.m_FillOrigin < 0)
            {
                this.m_FillOrigin = 0;
            }
            else if ((this.m_FillMethod == FillMethod.Horizontal) && (this.m_FillOrigin > 1))
            {
                this.m_FillOrigin = 0;
            }
            else if ((this.m_FillMethod == FillMethod.Vertical) && (this.m_FillOrigin > 1))
            {
                this.m_FillOrigin = 0;
            }
            else if (this.m_FillOrigin > 3)
            {
                this.m_FillOrigin = 0;
            }
            this.m_FillAmount = Mathf.Clamp(this.m_FillAmount, 0f, 1f);
        }

        public virtual void OnBeforeSerialize()
        {
        }

        protected override void OnPopulateMesh(Mesh toFill)
        {
            if (this.overrideSprite == null)
            {
                base.OnPopulateMesh(toFill);
            }
            else
            {
                switch (this.type)
                {
                    case Type.Simple:
                        this.GenerateSimpleSprite(toFill, this.m_PreserveAspect);
                        break;

                    case Type.Sliced:
                        this.GenerateSlicedSprite(toFill);
                        break;

                    case Type.Tiled:
                        this.GenerateTiledSprite(toFill);
                        break;

                    case Type.Filled:
                        this.GenerateFilledSprite(toFill, this.m_PreserveAspect);
                        break;
                }
            }
        }

        private static bool RadialCut(Vector3[] xy, Vector3[] uv, float fill, bool invert, int corner)
        {
            if (fill < 0.001f)
            {
                return false;
            }
            if ((corner & 1) == 1)
            {
                invert = !invert;
            }
            if (invert || (fill <= 0.999f))
            {
                float f = Mathf.Clamp01(fill);
                if (invert)
                {
                    f = 1f - f;
                }
                f *= 1.570796f;
                float cos = Mathf.Cos(f);
                float sin = Mathf.Sin(f);
                RadialCut(xy, cos, sin, invert, corner);
                RadialCut(uv, cos, sin, invert, corner);
            }
            return true;
        }

        private static void RadialCut(Vector3[] xy, float cos, float sin, bool invert, int corner)
        {
            int index = corner;
            int num2 = (corner + 1) % 4;
            int num3 = (corner + 2) % 4;
            int num4 = (corner + 3) % 4;
            if ((corner & 1) == 1)
            {
                if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;
                    if (invert)
                    {
                        xy[num2].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                        xy[num3].x = xy[num2].x;
                    }
                }
                else if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;
                    if (!invert)
                    {
                        xy[num3].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                        xy[num4].y = xy[num3].y;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }
                if (!invert)
                {
                    xy[num4].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                }
                else
                {
                    xy[num2].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                }
            }
            else
            {
                if (cos > sin)
                {
                    sin /= cos;
                    cos = 1f;
                    if (!invert)
                    {
                        xy[num2].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                        xy[num3].y = xy[num2].y;
                    }
                }
                else if (sin > cos)
                {
                    cos /= sin;
                    sin = 1f;
                    if (invert)
                    {
                        xy[num3].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                        xy[num4].x = xy[num3].x;
                    }
                }
                else
                {
                    cos = 1f;
                    sin = 1f;
                }
                if (invert)
                {
                    xy[num4].y = Mathf.Lerp(xy[index].y, xy[num3].y, sin);
                }
                else
                {
                    xy[num2].x = Mathf.Lerp(xy[index].x, xy[num3].x, cos);
                }
            }
        }

        public override void SetNativeSize()
        {
            if (this.overrideSprite != null)
            {
                float x = this.overrideSprite.rect.width / this.pixelsPerUnit;
                float y = this.overrideSprite.rect.height / this.pixelsPerUnit;
                base.rectTransform.anchorMax = base.rectTransform.anchorMin;
                base.rectTransform.sizeDelta = new Vector2(x, y);
                this.SetAllDirty();
            }
        }

        public float eventAlphaThreshold
        {
            get
            {
                return this.m_EventAlphaThreshold;
            }
            set
            {
                this.m_EventAlphaThreshold = value;
            }
        }

        public float fillAmount
        {
            get
            {
                return this.m_FillAmount;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<float>(ref this.m_FillAmount, Mathf.Clamp01(value)))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        public bool fillCenter
        {
            get
            {
                return this.m_FillCenter;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_FillCenter, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        public bool fillClockwise
        {
            get
            {
                return this.m_FillClockwise;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_FillClockwise, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        public FillMethod fillMethod
        {
            get
            {
                return this.m_FillMethod;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<FillMethod>(ref this.m_FillMethod, value))
                {
                    this.SetVerticesDirty();
                    this.m_FillOrigin = 0;
                }
            }
        }

        public int fillOrigin
        {
            get
            {
                return this.m_FillOrigin;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<int>(ref this.m_FillOrigin, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        public virtual float flexibleHeight
        {
            get
            {
                return -1f;
            }
        }

        public virtual float flexibleWidth
        {
            get
            {
                return -1f;
            }
        }

        public bool hasBorder
        {
            get
            {
                return ((this.overrideSprite != null) && (this.overrideSprite.border.sqrMagnitude > 0f));
            }
        }

        public virtual int layoutPriority
        {
            get
            {
                return 0;
            }
        }

        public override Texture mainTexture
        {
            get
            {
                return ((this.overrideSprite != null) ? this.overrideSprite.texture : Graphic.s_WhiteTexture);
            }
        }

        public virtual float minHeight
        {
            get
            {
                return 0f;
            }
        }

        public virtual float minWidth
        {
            get
            {
                return 0f;
            }
        }

        public Sprite overrideSprite
        {
            get
            {
                return ((this.m_OverrideSprite != null) ? this.m_OverrideSprite : this.sprite);
            }
            set
            {
                if (SetPropertyUtility.SetClass<Sprite>(ref this.m_OverrideSprite, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        public float pixelsPerUnit
        {
            get
            {
                float pixelsPerUnit = 100f;
                if (this.sprite != null)
                {
                    pixelsPerUnit = this.sprite.pixelsPerUnit;
                }
                float referencePixelsPerUnit = 100f;
                if (base.canvas != null)
                {
                    referencePixelsPerUnit = base.canvas.referencePixelsPerUnit;
                }
                return (pixelsPerUnit / referencePixelsPerUnit);
            }
        }

        public virtual float preferredHeight
        {
            get
            {
                if (this.overrideSprite == null)
                {
                    return 0f;
                }
                if ((this.type != Type.Sliced) && (this.type != Type.Tiled))
                {
                    return (this.overrideSprite.rect.size.y / this.pixelsPerUnit);
                }
                return (DataUtility.GetMinSize(this.overrideSprite).y / this.pixelsPerUnit);
            }
        }

        public virtual float preferredWidth
        {
            get
            {
                if (this.overrideSprite == null)
                {
                    return 0f;
                }
                if ((this.type != Type.Sliced) && (this.type != Type.Tiled))
                {
                    return (this.overrideSprite.rect.size.x / this.pixelsPerUnit);
                }
                return (DataUtility.GetMinSize(this.overrideSprite).x / this.pixelsPerUnit);
            }
        }

        public bool preserveAspect
        {
            get
            {
                return this.m_PreserveAspect;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<bool>(ref this.m_PreserveAspect, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        public Sprite sprite
        {
            get
            {
                return this.m_Sprite;
            }
            set
            {
                if (SetPropertyUtility.SetClass<Sprite>(ref this.m_Sprite, value))
                {
                    this.SetAllDirty();
                }
            }
        }

        public Type type
        {
            get
            {
                return this.m_Type;
            }
            set
            {
                if (SetPropertyUtility.SetStruct<Type>(ref this.m_Type, value))
                {
                    this.SetVerticesDirty();
                }
            }
        }

        public enum FillMethod
        {
            Horizontal,
            Vertical,
            Radial90,
            Radial180,
            Radial360
        }

        public enum Origin180
        {
            Bottom,
            Left,
            Top,
            Right
        }

        public enum Origin360
        {
            Bottom,
            Right,
            Top,
            Left
        }

        public enum Origin90
        {
            BottomLeft,
            TopLeft,
            TopRight,
            BottomRight
        }

        public enum OriginHorizontal
        {
            Left,
            Right
        }

        public enum OriginVertical
        {
            Bottom,
            Top
        }

        public enum Type
        {
            Simple,
            Sliced,
            Tiled,
            Filled
        }
    }
}

