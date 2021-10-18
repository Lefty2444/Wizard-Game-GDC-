using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Element
{
    Fire,  // May catch foes on fire
    Ice,   // May freeze (slow down) foes
    Earth, // May 
    Storm  // May stun enemies 
}
[System.Serializable]
public class SoundEffect
{
    public AudioClip sound;
    [Range(0f, 1f)]
    public float volume = .5f;
    [Range(0f, 3f)]
    public float pitch = 1f;
    public float pitchVariation = .1f;
}

public class Spell : ScriptableObject
{
    [Header("Spell Settings")]


    [Tooltip("Time in seconds for spell to recharge")] [Range(.1f, 30)]
    public float cooldown = 1.0f;

    [Tooltip("Number of times the spell is repeated")] [Range(0, 30)]
    public int repetitions = 0;

    [Tooltip("The number of seconds to cast (for every repetition)")] [Range(.1f, 5)]
    public float castingTime = .5f;

    [Tooltip("The sound effect that is played when the spell is cast")]
    public SoundEffect soundEffect;

    [Tooltip("Element classification for spell")]
    public Element element;
}
