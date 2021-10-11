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

public class Spell : ScriptableObject
{
    [Header("Spell Settings")]

    [Tooltip("Time in seconds for spell to recharge")] [Range(.1f, 30)]
    public float cooldown = 1.0f;
    [Tooltip("Element classification for spell")]
    public Element element;
}
