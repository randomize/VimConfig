namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;

    public sealed class SpringJoint2D : AnchoredJoint2D
    {
        public Vector2 GetReactionForce(float timeStep)
        {
            Vector2 vector;
            SpringJoint2D_CUSTOM_INTERNAL_GetReactionForce(this, timeStep, out vector);
            return vector;
        }

        public float GetReactionTorque(float timeStep)
        {
            return INTERNAL_CALL_GetReactionTorque(this, timeStep);
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float INTERNAL_CALL_GetReactionTorque(SpringJoint2D self, float timeStep);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void SpringJoint2D_CUSTOM_INTERNAL_GetReactionForce(SpringJoint2D joint, float timeStep, out Vector2 value);

        public float dampingRatio { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float distance { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float frequency { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }
    }
}

