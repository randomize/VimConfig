namespace UnityEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine.Internal;

    public sealed class ParticleSystem : Component
    {
        [ExcludeFromDocs]
        public void Clear()
        {
            bool withChildren = true;
            this.Clear(withChildren);
        }

        public void Clear([DefaultValue("true")] bool withChildren)
        {
            if (withChildren)
            {
                foreach (ParticleSystem system in GetParticleSystems(this))
                {
                    system.Internal_Clear();
                }
            }
            else
            {
                this.Internal_Clear();
            }
        }

        public void Emit(int count)
        {
            INTERNAL_CALL_Emit(this, count);
        }

        public void Emit(Particle particle)
        {
            this.Internal_Emit(ref particle);
        }

        public void Emit(Vector3 position, Vector3 velocity, float size, float lifetime, Color32 color)
        {
            Particle particle = new Particle {
                position = position,
                velocity = velocity,
                lifetime = lifetime,
                startLifetime = lifetime,
                size = size,
                rotation = 0f,
                angularVelocity = 0f,
                color = color,
                randomSeed = 5
            };
            this.Internal_Emit(ref particle);
        }

        private static void GetDirectParticleSystemChildrenRecursive(Transform transform, List<ParticleSystem> particleSystems)
        {
            IEnumerator enumerator = transform.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    Transform current = (Transform) enumerator.Current;
                    ParticleSystem component = current.gameObject.GetComponent<ParticleSystem>();
                    if (component != null)
                    {
                        particleSystems.Add(component);
                        GetDirectParticleSystemChildrenRecursive(current, particleSystems);
                    }
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable == null)
                {
                }
                disposable.Dispose();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern int GetParticles(Particle[] particles);
        internal static ParticleSystem[] GetParticleSystems(ParticleSystem root)
        {
            if (root == null)
            {
                return null;
            }
            List<ParticleSystem> particleSystems = new List<ParticleSystem> {
                root
            };
            GetDirectParticleSystemChildrenRecursive(root.transform, particleSystems);
            return particleSystems.ToArray();
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private static extern void INTERNAL_CALL_Emit(ParticleSystem self, int count);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Clear();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Emit(ref Particle particle);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_get_startColor(out Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern bool Internal_IsAlive();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Pause();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Play();
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void INTERNAL_set_startColor(ref Color value);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Simulate(float t, bool restart);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        private extern void Internal_Stop();
        [ExcludeFromDocs]
        public bool IsAlive()
        {
            bool withChildren = true;
            return this.IsAlive(withChildren);
        }

        public bool IsAlive([DefaultValue("true")] bool withChildren)
        {
            if (!withChildren)
            {
                return this.Internal_IsAlive();
            }
            foreach (ParticleSystem system in GetParticleSystems(this))
            {
                if (system.Internal_IsAlive())
                {
                    return true;
                }
            }
            return false;
        }

        [ExcludeFromDocs]
        public void Pause()
        {
            bool withChildren = true;
            this.Pause(withChildren);
        }

        public void Pause([DefaultValue("true")] bool withChildren)
        {
            if (withChildren)
            {
                foreach (ParticleSystem system in GetParticleSystems(this))
                {
                    system.Internal_Pause();
                }
            }
            else
            {
                this.Internal_Pause();
            }
        }

        [ExcludeFromDocs]
        public void Play()
        {
            bool withChildren = true;
            this.Play(withChildren);
        }

        public void Play([DefaultValue("true")] bool withChildren)
        {
            if (withChildren)
            {
                foreach (ParticleSystem system in GetParticleSystems(this))
                {
                    system.Internal_Play();
                }
            }
            else
            {
                this.Internal_Play();
            }
        }

        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        public extern void SetParticles(Particle[] particles, int size);
        [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall]
        internal extern void SetupDefaultType(int type);
        [ExcludeFromDocs]
        public void Simulate(float t)
        {
            bool restart = true;
            bool withChildren = true;
            this.Simulate(t, withChildren, restart);
        }

        [ExcludeFromDocs]
        public void Simulate(float t, bool withChildren)
        {
            bool restart = true;
            this.Simulate(t, withChildren, restart);
        }

        public void Simulate(float t, [DefaultValue("true")] bool withChildren, [DefaultValue("true")] bool restart)
        {
            if (withChildren)
            {
                foreach (ParticleSystem system in GetParticleSystems(this))
                {
                    system.Internal_Simulate(t, restart);
                }
            }
            else
            {
                this.Internal_Simulate(t, restart);
            }
        }

        [ExcludeFromDocs]
        public void Stop()
        {
            bool withChildren = true;
            this.Stop(withChildren);
        }

        public void Stop([DefaultValue("true")] bool withChildren)
        {
            if (withChildren)
            {
                foreach (ParticleSystem system in GetParticleSystems(this))
                {
                    system.Internal_Stop();
                }
            }
            else
            {
                this.Internal_Stop();
            }
        }

        public float duration { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float emissionRate { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool enableEmission { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float gravityModifier { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool isPaused { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isPlaying { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool isStopped { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public bool loop { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int maxParticles { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public int particleCount { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; }

        public float playbackSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public bool playOnAwake { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public uint randomSeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [Obsolete("safeCollisionEventSize has been deprecated. Use GetSafeCollisionEventSize() instead (UnityUpgradable) -> ParticlePhysicsExtensions.GetSafeCollisionEventSize(UnityEngine.ParticleSystem)", false)]
        public int safeCollisionEventSize
        {
            get
            {
                return ParticleSystemExtensionsImpl.GetSafeCollisionEventSize(this);
            }
        }

        public ParticleSystemSimulationSpace simulationSpace { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public Color startColor
        {
            get
            {
                Color color;
                this.INTERNAL_get_startColor(out color);
                return color;
            }
            set
            {
                this.INTERNAL_set_startColor(ref value);
            }
        }

        public float startDelay { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float startLifetime { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float startRotation { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float startSize { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float startSpeed { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        public float time { [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] get; [MethodImpl(MethodImplOptions.InternalCall), WrapperlessIcall] set; }

        [StructLayout(LayoutKind.Sequential, Size=1), Obsolete("ParticleSystem.CollisionEvent has been deprecated. Use ParticleCollisionEvent instead (UnityUpgradable) -> ParticleCollisionEvent", true)]
        public struct CollisionEvent
        {
            public Vector3 intersection
            {
                get
                {
                    return new Vector3();
                }
            }
            public Vector3 normal
            {
                get
                {
                    return new Vector3();
                }
            }
            public Vector3 velocity
            {
                get
                {
                    return new Vector3();
                }
            }
            public Collider collider
            {
                get
                {
                    return null;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct Particle
        {
            private Vector3 m_Position;
            private Vector3 m_Velocity;
            private Vector3 m_AnimatedVelocity;
            private Vector3 m_AxisOfRotation;
            private float m_Rotation;
            private float m_AngularVelocity;
            private float m_Size;
            private Color32 m_Color;
            private uint m_RandomSeed;
            private float m_Lifetime;
            private float m_StartLifetime;
            private float m_EmitAccumulator0;
            private float m_EmitAccumulator1;
            public Vector3 position
            {
                get
                {
                    return this.m_Position;
                }
                set
                {
                    this.m_Position = value;
                }
            }
            public Vector3 velocity
            {
                get
                {
                    return this.m_Velocity;
                }
                set
                {
                    this.m_Velocity = value;
                }
            }
            public float lifetime
            {
                get
                {
                    return this.m_Lifetime;
                }
                set
                {
                    this.m_Lifetime = value;
                }
            }
            public float startLifetime
            {
                get
                {
                    return this.m_StartLifetime;
                }
                set
                {
                    this.m_StartLifetime = value;
                }
            }
            public float size
            {
                get
                {
                    return this.m_Size;
                }
                set
                {
                    this.m_Size = value;
                }
            }
            public Vector3 axisOfRotation
            {
                get
                {
                    return this.m_AxisOfRotation;
                }
                set
                {
                    this.m_AxisOfRotation = value;
                }
            }
            public float rotation
            {
                get
                {
                    return (this.m_Rotation * 57.29578f);
                }
                set
                {
                    this.m_Rotation = value * 0.01745329f;
                }
            }
            public float angularVelocity
            {
                get
                {
                    return (this.m_AngularVelocity * 57.29578f);
                }
                set
                {
                    this.m_AngularVelocity = value * 0.01745329f;
                }
            }
            public Color32 color
            {
                get
                {
                    return this.m_Color;
                }
                set
                {
                    this.m_Color = value;
                }
            }
            [Obsolete("randomValue property is deprecated. Use randomSeed instead to control random behavior of particles.")]
            public float randomValue
            {
                get
                {
                    return BitConverter.ToSingle(BitConverter.GetBytes(this.m_RandomSeed), 0);
                }
                set
                {
                    this.m_RandomSeed = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
                }
            }
            public uint randomSeed
            {
                get
                {
                    return this.m_RandomSeed;
                }
                set
                {
                    this.m_RandomSeed = value;
                }
            }
        }
    }
}

