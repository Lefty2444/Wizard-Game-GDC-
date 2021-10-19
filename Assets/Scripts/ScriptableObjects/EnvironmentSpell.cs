using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum SpawnAreas
{
    Anywhere,
    AllEdges,
    TopAndBottomEdge,
    SideEdges,
    TopEdge,
    BottomEdge,
    LeftEdge,
    RightEdge
}

public enum SpawnRotation
{
    FacingCenter,
    StraightFromEdge,
    Random
}

[CreateAssetMenu(fileName = "New Shooting Spell", menuName = "Spells/Environment", order = 1)]
public class EnvironmentSpell : Spell
{
    [Header("Spawning Zone Settings")]

    [Tooltip("Where projectiles will be able to spawn")]
    public SpawnAreas spawnArea = SpawnAreas.AllEdges;
    [Tooltip("The starting rotation of projectiles")]
    public SpawnRotation spawnRotation = SpawnRotation.StraightFromEdge;
    [Tooltip("The max offset (in degrees) of starting rotation (0 means no randomness will be applied)")]
    public float rotationOffset = 0;
    [Tooltip("The number of projectiles to be fired every wave")]
    [Range(1, 30)]
    public int projectilesPerWave = 1;


    public ProjectileSettings projectileSettings;



    private void OnEnable()
    {
        //sprites = new Sprite[1];
        //sprites = Resources.Load<Texture2D>("Default/Projectile");
    }
}
  
