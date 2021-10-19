using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ProjectileSettings
{
    [Header("Visual Settings")]

    [Tooltip("List of sprites that will be cycled through")]
    public Sprite[] sprites;
    [Tooltip("Number of animation frames per second")]
    [Range(0, 60)]
    public float spriteRate = 20;
    [Tooltip("Color modifier of sprite(s). Leave as white to use base color of sprites")]
    public Color color = Color.white;
    [Tooltip("Scale modifier of bullet")]
    [Range(.1f, 10)]
    public float size = 1;


    [Header("Core Projectile Settings")]

    [Tooltip("Speed of the bullet")]
    [Range(0, 30)]
    public float bulletSpeed = 5;
    [Tooltip("Damage of bullets")]
    [Range(0, 200)]
    public float damage = 10;
    [Tooltip("Time in seconds for projectiles to despawn")]
    [Range(.1f, 5)]
    public float projectileLifetime = 1f;
    [Tooltip("Time in seconds before projectiles start checking for collisions")]
    [Range(.1f, 2f)]
    public float timeToBecomeActive = .2f;
    [Tooltip("Max number of ricochets before projectile despawns (if 0, it cannot bounce)")]
    [Range(0, 5)]
    public int bounces = 0;

}

[CreateAssetMenu(fileName = "New Shooting Spell", menuName = "Spells/Shooting", order = 1)]
public class ShootingSpell : Spell
{

    public ProjectileSettings projectileSettings;
    
    [Header("Shooting Settings")]
    [Tooltip("Bullets fired per shot. If greater than 1, bullets will be evenly spaced across arc")]
    [Range(1, 20)]
    public int bulletsPerShot = 1;
    [Tooltip("Spread of bullets (in degrees). Spread is random unless bulletsPerShot > 1")]
    [Range(0, 360)]
    public float arc = 5;


    private void OnEnable()
    {
        //sprites = new Sprite[1];
        //sprites = Resources.Load<Texture2D>("Default/Projectile");
    }
}