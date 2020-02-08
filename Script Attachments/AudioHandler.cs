using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


[System.Serializable]
[RequireComponent(typeof(AudioSource))]
public class AudioHandler
{
    [Header("Audio Settings")]
    [SerializeField] AudioSource m_combatAudioSource;
    [SerializeField] AudioSource m_movementAudioSource;

    [SerializeField] AudioClip m_attackSound;
    [SerializeField] AudioClip m_damageSound;
    [SerializeField] AudioClip m_deathSound;
    [SerializeField] AudioClip m_walkSound;


    public IEnumerator PlayAttackSound()
    {
        yield return null;
        if (!m_combatAudioSource.isPlaying)
        {
            m_combatAudioSource.clip = m_attackSound;
            m_combatAudioSource.Play();
        }

    }

    public IEnumerator PlayDamageSound()
    {

        if (!m_combatAudioSource.isPlaying)
        {
            m_combatAudioSource.clip = m_damageSound;
            m_combatAudioSource.Play();
        }
    yield return null;

    }

    public IEnumerator PlayDeathSound()
    {
        if (!m_combatAudioSource.isPlaying)
        {
            m_combatAudioSource.clip = m_deathSound;
            m_combatAudioSource.Play();
        }
        yield return null;
    }

    /////////////////////////////////////////////////////////////////// - Tried to get footsteps working but I wasn't able to. Only works when starting to move.

    public IEnumerator PlayFootstepSound()
    {
        yield return null;
        if (!m_movementAudioSource.isPlaying)
        {
            m_movementAudioSource.clip = m_walkSound;
            m_movementAudioSource.Play();
        }
    }

    public IEnumerator LoopFootstepSound()
    {
        yield return null;
        m_movementAudioSource.loop = true;
        m_movementAudioSource.clip = m_walkSound;
        m_movementAudioSource.Play();
    }

    public void SetLoop(bool newValue)
    {
        m_movementAudioSource.loop = newValue;
    }

    ///////////////////////////////////////////////////////////////////

}
