namespace UnityEngine
{
    using System;
    using System.Runtime.CompilerServices;

    public sealed class Gyroscope
    {
        private int m_GyroIndex;

        internal Gyroscope(int index)
        {
            this.m_GyroIndex = index;
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Quaternion attitude_Internal(int idx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern bool getEnabled_Internal(int idx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern float getUpdateInterval_Internal(int idx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Vector3 gravity_Internal(int idx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Vector3 rotationRate_Internal(int idx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Vector3 rotationRateUnbiased_Internal(int idx);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void setEnabled_Internal(int idx, bool enabled);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void setUpdateInterval_Internal(int idx, float interval);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern Vector3 userAcceleration_Internal(int idx);

        public Quaternion attitude
        {
            get
            {
                return attitude_Internal(this.m_GyroIndex);
            }
        }

        public bool enabled
        {
            get
            {
                return getEnabled_Internal(this.m_GyroIndex);
            }
            set
            {
                setEnabled_Internal(this.m_GyroIndex, value);
            }
        }

        public Vector3 gravity
        {
            get
            {
                return gravity_Internal(this.m_GyroIndex);
            }
        }

        public Vector3 rotationRate
        {
            get
            {
                return rotationRate_Internal(this.m_GyroIndex);
            }
        }

        public Vector3 rotationRateUnbiased
        {
            get
            {
                return rotationRateUnbiased_Internal(this.m_GyroIndex);
            }
        }

        public float updateInterval
        {
            get
            {
                return getUpdateInterval_Internal(this.m_GyroIndex);
            }
            set
            {
                setUpdateInterval_Internal(this.m_GyroIndex, value);
            }
        }

        public Vector3 userAcceleration
        {
            get
            {
                return userAcceleration_Internal(this.m_GyroIndex);
            }
        }
    }
}

