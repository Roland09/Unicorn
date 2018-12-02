using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Rainbow particle emitter for the Unicorn horn.
/// Parts of the particle code were taken from https://docs.unity3d.com/ScriptReference/ParticleSystem.GetParticles.html
/// </summary>
public class RainbowParticleEmitter : MonoBehaviour
{
    public enum RainbowType
    {
        /// <summary>
        /// Color depends on the particle lifetime
        /// </summary>
        LifeTime,

        /// <summary>
        /// Color depends on the inverse particle lifetime
        /// </summary>
        InverseLifeTime,

        /// <summary>
        /// Color depends on the total velocity of the particle. You need to adjust the velocity gradient interval to match your requirements
        /// </summary>
        TotalVelocity
    }

    #region Public Variables

    /// <summary>
    /// Enabled/disable the particles
    /// </summary>
    public bool particlesEnabled = false;

    /// <summary>
    /// The gradient that is being used to get the particle color
    /// </summary>
    public Gradient particleColorGradient;

    /// <summary>
    /// The algorithm that's being used to get the particle color from the gradient
    /// </summary>
    public RainbowType rainbowType = RainbowType.LifeTime;

    /// <summary>
    /// Used in combination with TotalVelocity RainbowType.
    /// Get the color from the gradient using the total velocity of the normalized y value.
    /// In a rainbow gradient you can basically determine how much of red or blue should be used.
    /// These values depend on the angle of the particle system (e. g. the horn angle of the Unicorn).
    /// </summary>
    public Vector2 velocityGradientInterval;

    #endregion Public Variables

    #region Private Variables

    private ParticleSystem ps;
    private ParticleSystem.Particle[] m_Particles;

    #endregion Private Variables

    void Start()
    {
        ps = GetComponent<ParticleSystem>();

        if( particlesEnabled)
        {
            ps.Play();
        }
        else
        {
            ps.Stop();
        }
    }

    private void LateUpdate()
    {
        if (!enabled)
            return;

        InitializeIfNeeded();

        // GetParticles is allocation free because we reuse the m_Particles buffer between updates
        int numParticlesAlive = ps.GetParticles(m_Particles);

        // Change only the particles that are alive
        for (int i = 0; i < numParticlesAlive; i++)
        {
            float t;

            switch (rainbowType)
            {
                case RainbowType.LifeTime:
                    t = Mathf.InverseLerp(0, m_Particles[i].startLifetime, m_Particles[i].startLifetime - m_Particles[i].remainingLifetime);
                    break;
                case RainbowType.InverseLifeTime:
                    t = Mathf.InverseLerp(0, m_Particles[i].startLifetime, m_Particles[i].remainingLifetime);
                    break;
                case RainbowType.TotalVelocity:
                    t = Mathf.InverseLerp(velocityGradientInterval.x, velocityGradientInterval.y, m_Particles[i].totalVelocity.normalized.y);
                    break;
                default:
                    t = 0f;
                    break;
            }

            m_Particles[i].startColor = particleColorGradient.Evaluate( t);
        }

        // Apply the particle changes to the particle system
        ps.SetParticles(m_Particles, numParticlesAlive);
    }

    void InitializeIfNeeded()
    {
        if (ps == null)
            ps = GetComponent<ParticleSystem>();

        if (m_Particles == null || m_Particles.Length < ps.main.maxParticles)
            m_Particles = new ParticleSystem.Particle[ps.main.maxParticles];
    }

    #region Code for Testing
    /*
    void Update()
    {
        // Toggle the particle system via P key
        if (Input.GetKeyDown(KeyCode.P))
        {
            particlesEnabled = !particlesEnabled;

            if(particlesEnabled)
            {
                ps.Play();
            }
            else
            {
                ps.Stop();
            }
        }
    }
    */
    #endregion Code for Testing
}
