namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    public class VertexHelper : IDisposable
    {
        private List<Color32> m_Colors;
        private List<int> m_Indicies;
        private List<Vector3> m_Normals;
        private List<Vector3> m_Positions;
        private List<Vector4> m_Tangents;
        private List<Vector2> m_Uv0S;
        private List<Vector2> m_Uv1S;
        private static readonly Vector3 s_DefaultNormal = Vector3.back;
        private static readonly Vector4 s_DefaultTangent = new Vector4(1f, 0f, 0f, -1f);

        public VertexHelper()
        {
            this.m_Positions = ListPool<Vector3>.Get();
            this.m_Colors = ListPool<Color32>.Get();
            this.m_Uv0S = ListPool<Vector2>.Get();
            this.m_Uv1S = ListPool<Vector2>.Get();
            this.m_Normals = ListPool<Vector3>.Get();
            this.m_Tangents = ListPool<Vector4>.Get();
            this.m_Indicies = ListPool<int>.Get();
        }

        public VertexHelper(Mesh m)
        {
            this.m_Positions = ListPool<Vector3>.Get();
            this.m_Colors = ListPool<Color32>.Get();
            this.m_Uv0S = ListPool<Vector2>.Get();
            this.m_Uv1S = ListPool<Vector2>.Get();
            this.m_Normals = ListPool<Vector3>.Get();
            this.m_Tangents = ListPool<Vector4>.Get();
            this.m_Indicies = ListPool<int>.Get();
            this.m_Positions.AddRange(m.vertices);
            this.m_Colors.AddRange(m.colors32);
            this.m_Uv0S.AddRange(m.uv);
            this.m_Uv1S.AddRange(m.uv2);
            this.m_Normals.AddRange(m.normals);
            this.m_Tangents.AddRange(m.tangents);
            this.m_Indicies.AddRange(m.GetIndices(0));
        }

        public void AddTriangle(int idx0, int idx1, int idx2)
        {
            this.m_Indicies.Add(idx0);
            this.m_Indicies.Add(idx1);
            this.m_Indicies.Add(idx2);
        }

        public void AddUIVertexQuad(UIVertex[] verts)
        {
            int currentVertCount = this.currentVertCount;
            for (int i = 0; i < 4; i++)
            {
                this.AddVert(verts[i].position, verts[i].color, verts[i].uv0, verts[i].uv1, verts[i].normal, verts[i].tangent);
            }
            this.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            this.AddTriangle(currentVertCount + 2, currentVertCount + 3, currentVertCount);
        }

        public void AddUIVertexTriangleStream(List<UIVertex> verts)
        {
            for (int i = 0; i < verts.Count; i += 3)
            {
                int currentVertCount = this.currentVertCount;
                for (int j = 0; j < 3; j++)
                {
                    UIVertex vertex = verts[i + j];
                    UIVertex vertex2 = verts[i + j];
                    UIVertex vertex3 = verts[i + j];
                    UIVertex vertex4 = verts[i + j];
                    UIVertex vertex5 = verts[i + j];
                    UIVertex vertex6 = verts[i + j];
                    this.AddVert(vertex.position, vertex2.color, vertex3.uv0, vertex4.uv1, vertex5.normal, vertex6.tangent);
                }
                this.AddTriangle(currentVertCount, currentVertCount + 1, currentVertCount + 2);
            }
        }

        public void AddVert(UIVertex v)
        {
            this.AddVert(v.position, v.color, v.uv0, v.uv1, v.normal, v.tangent);
        }

        public void AddVert(Vector3 position, Color32 color, Vector2 uv0)
        {
            this.AddVert(position, color, uv0, Vector2.zero, s_DefaultNormal, s_DefaultTangent);
        }

        public void AddVert(Vector3 position, Color32 color, Vector2 uv0, Vector2 uv1, Vector3 normal, Vector4 tangent)
        {
            this.m_Positions.Add(position);
            this.m_Colors.Add(color);
            this.m_Uv0S.Add(uv0);
            this.m_Uv1S.Add(uv1);
            this.m_Normals.Add(normal);
            this.m_Tangents.Add(tangent);
        }

        public void Dispose()
        {
            ListPool<Vector3>.Release(this.m_Positions);
            ListPool<Color32>.Release(this.m_Colors);
            ListPool<Vector2>.Release(this.m_Uv0S);
            ListPool<Vector2>.Release(this.m_Uv1S);
            ListPool<Vector3>.Release(this.m_Normals);
            ListPool<Vector4>.Release(this.m_Tangents);
            ListPool<int>.Release(this.m_Indicies);
            this.m_Positions = null;
            this.m_Colors = null;
            this.m_Uv0S = null;
            this.m_Uv1S = null;
            this.m_Normals = null;
            this.m_Tangents = null;
            this.m_Indicies = null;
        }

        public void FillMesh(Mesh mesh)
        {
            mesh.Clear();
            mesh.SetVertices(this.m_Positions);
            mesh.SetColors(this.m_Colors);
            mesh.SetUVs(0, this.m_Uv0S);
            mesh.SetUVs(1, this.m_Uv1S);
            mesh.SetNormals(this.m_Normals);
            mesh.SetTangents(this.m_Tangents);
            mesh.SetTriangles(this.m_Indicies, 0);
            mesh.RecalculateBounds();
        }

        public void GetUIVertexStream(List<UIVertex> stream)
        {
            stream.Clear();
            UIVertex vertex = new UIVertex();
            for (int i = 0; i < this.m_Indicies.Count; i++)
            {
                this.PopulateUIVertex(ref vertex, this.m_Indicies[i]);
                stream.Add(vertex);
            }
        }

        public void PopulateUIVertex(ref UIVertex vertex, int i)
        {
            vertex.position = this.m_Positions[i];
            vertex.color = this.m_Colors[i];
            vertex.uv0 = this.m_Uv0S[i];
            vertex.uv1 = this.m_Uv1S[i];
            vertex.normal = this.m_Normals[i];
            vertex.tangent = this.m_Tangents[i];
        }

        public int currentVertCount
        {
            get
            {
                return this.m_Positions.Count;
            }
        }
    }
}

