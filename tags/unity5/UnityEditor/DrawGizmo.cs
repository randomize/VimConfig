namespace UnityEditor
{
    using System;

    public sealed class DrawGizmo : Attribute
    {
        public System.Type drawnType;
        public GizmoType drawOptions;

        public DrawGizmo(GizmoType gizmo)
        {
            this.drawOptions = gizmo;
        }

        public DrawGizmo(GizmoType gizmo, System.Type drawnGizmoType)
        {
            this.drawnType = drawnGizmoType;
            this.drawOptions = gizmo;
        }
    }
}

