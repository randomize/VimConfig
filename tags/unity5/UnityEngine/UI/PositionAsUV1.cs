namespace UnityEngine.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    [AddComponentMenu("UI/Effects/Position As UV1", 0x10)]
    public class PositionAsUV1 : BaseMeshEffect
    {
        protected PositionAsUV1()
        {
        }

        public override void ModifyMesh(Mesh mesh)
        {
            if (this.IsActive())
            {
                List<Vector3> list = mesh.vertices.ToList<Vector3>();
                List<Vector2> uvs = ListPool<Vector2>.Get();
                for (int i = 0; i < list.Count; i++)
                {
                    Vector3 vector = list[i];
                    Vector3 vector2 = list[i];
                    Vector3 vector3 = list[i];
                    uvs.Add(new Vector2(vector2.x, vector3.y));
                    list[i] = vector;
                }
                mesh.SetUVs(1, uvs);
                ListPool<Vector2>.Release(uvs);
            }
        }
    }
}

