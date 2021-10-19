using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SceneBounds
{
    public float upper;
    public float lower;
    public float left;
    public float right;
    public Vector2 GetRandomPos()
    {
        return new Vector2(Random.Range(left, right), Random.Range(lower, upper));
    }
}

public class PlayerWeapon : MonoBehaviour
{
    public SceneBounds bounds;
    public Spell currentSpell;
    public Spell[] spellBook = new Spell[3];
    public float wandRotationSpeed = 15;
    private AudioSource audioSource;

    private int spellIndex = 0;
    private float[] spellCooldowns;

    private Transform wand;
    private Transform firePoint;
    private Vector3 firePointPos;
    private bool isCasting = false;


    // Start is called before the first frame update
    void Awake()
    {
        wand = transform.GetChild(0);
        firePoint = wand.GetChild(0);
        firePointPos = firePoint.localPosition;
        spellCooldowns = new float[spellBook.Length];
        for (int i = 0; i < spellCooldowns.Length; i++)
            spellCooldowns[i] = 0;
        audioSource = GetComponent<AudioSource>();
        currentSpell = spellBook[0];
    }



    // Update is called once per frame
    void Update()
    {
        MoveWand();
       
        DecrementCooldowns();

        if (!isCasting && spellCooldowns[spellIndex] <= 0 && Input.GetMouseButton(0))
        {
            isCasting = true;
            StartCoroutine(SpellCasting());
        }
        //SetUI();
        
    }

    IEnumerator SpellCasting()
    {   
        for (int i = 0; i <= currentSpell.repetitions; i++)
        {
            CastSpell();
            yield return new WaitForSeconds(currentSpell.castingTime);
        }
        spellCooldowns[spellIndex] = currentSpell.cooldown;
        NextSpell();
        isCasting = false;
    }

    void NextSpell()
    {
        spellIndex++;
        if (spellIndex >= spellBook.Length)
        {
            spellIndex = 0;
        }
        currentSpell = spellBook[spellIndex];
    }

    void CastSpell()
    {
        audioSource.Stop();
        SoundEffect sfx = currentSpell.soundEffect;
        audioSource.clip = sfx.sound;
        audioSource.volume = sfx.volume;
        float pitch = Random.Range(sfx.pitch - sfx.pitchVariation, sfx.pitch + sfx.pitchVariation);
        audioSource.pitch = pitch;
        audioSource.Play();


        if (currentSpell is ShootingSpell)
        {
            CastShootingSpell();
        } else if (currentSpell is EnvironmentSpell)
        {
            CastEnvironmentSpell();
        }
    }


    private void CastShootingSpell()
    {
        
        ShootingSpell spell = (ShootingSpell)currentSpell;
        if (spell.bulletsPerShot <= 1)
            CreateBullet(spell, spell.projectileSettings, firePoint.rotation, transform.eulerAngles.z + Random.Range(-spell.arc / 2, spell.arc / 2));
        else
        {
            for (int i = 0; i < spell.bulletsPerShot; i++)
            {
                CreateBullet(spell, spell.projectileSettings, firePoint.rotation, Mathf.Lerp(-spell.arc / 2, spell.arc / 2, ((float)i + .5f) / spell.bulletsPerShot));
            }
        }
    }

    private void CastEnvironmentSpell()
    {
        EnvironmentSpell spell = (EnvironmentSpell)currentSpell;
        for (int i = 0; i < spell.projectilesPerWave; i++)
        {
            Vector3 spawnRotation = new Vector3(0, 0, 0);
            Vector2 spawnPosition = bounds.GetRandomPos();
            int side = Random.Range(0, 4);
            switch (spell.spawnArea)
            {
                case SpawnAreas.Anywhere:
                    break;
                case SpawnAreas.AllEdges:
                    if (side == 0)
                    {
                        spawnPosition.x = bounds.left;
                        spawnRotation.z = -90; // right
                    }
                    else if (side == 1)
                    {
                        spawnPosition.x = bounds.right;
                        spawnRotation.z = 90; // left
                    }
                    else if (side == 2)
                    {
                        spawnPosition.y = bounds.upper;
                        spawnRotation.z = 180; // down
                    }
                    else if (side == 3)
                    {
                        spawnPosition.y = bounds.lower;
                        spawnRotation.z = 0; // up
                    }
                    break;
                case SpawnAreas.BottomEdge:
                    spawnPosition.y = bounds.lower;
                    spawnRotation.z = 0; // up
                    break;
                case SpawnAreas.TopEdge:
                    spawnPosition.y = bounds.upper;
                    spawnRotation.z = 180; // down
                    break;
                case SpawnAreas.LeftEdge:
                    spawnPosition.x = bounds.left;
                    spawnRotation.z = -90; // right
                    break;
                case SpawnAreas.RightEdge:
                    spawnPosition.x = bounds.right;
                    spawnRotation.z = 90; // left
                    break;
                case SpawnAreas.TopAndBottomEdge:
                    if (side < 2)
                    {
                        spawnPosition.y = bounds.upper;
                        spawnRotation.z = 180; // down
                    }
                    else
                    {
                        spawnPosition.y = bounds.lower;
                        spawnRotation.z = 0; // up
                    }
                    break;
                case SpawnAreas.SideEdges:
                    if (side < 2)
                    {
                        spawnPosition.x = bounds.left;
                        spawnRotation.z = -90; // right
                    }
                    else
                    {
                        spawnPosition.x = bounds.right;
                        spawnRotation.z = 90; // left
                    }
                    break;
                default:
                    break;
            }
            firePoint.position = spawnPosition;
            if (spell.spawnRotation == SpawnRotation.Random)
            {
                spawnRotation.z = Random.Range(-180, 180);
            } else if (spell.spawnRotation == SpawnRotation.FacingCenter) {
                spawnRotation.z = Mathf.Atan2(-spawnPosition.y, -spawnPosition.x) * Mathf.Rad2Deg - 90;
            }

            CreateBullet(spell, spell.projectileSettings, Quaternion.Euler(spawnRotation), Random.Range(-spell.rotationOffset / 2, spell.rotationOffset / 2));
        }
        firePoint.localPosition = firePointPos;
        firePoint.localRotation = Quaternion.identity;
    }

    void DecrementCooldowns()
    {
        for (int i = 0; i < spellCooldowns.Length; i++)
            spellCooldowns[i] -= Time.deltaTime;
    }

    void CreateBullet(Spell spell, ProjectileSettings projectileSettings, Quaternion rotation, float offset)
    {
        GameObject bullet = ObjectPool.SharedInstance.GetReadyObject("projectile");
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = rotation;
        bullet.transform.eulerAngles += new Vector3(0, 0, offset);
        bullet.transform.localScale = Vector3.one * projectileSettings.size;
        bullet.SetActive(true);
        bullet.GetComponent<ProjectileMover>().SetStats(spell.castingTime, projectileSettings.projectileLifetime, projectileSettings.bulletSpeed, projectileSettings.color, projectileSettings.sprites, projectileSettings.spriteRate, projectileSettings.bounces);
    }

    private void MoveWand()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(wand.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg - 90;
        Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle));
        wand.rotation = Quaternion.Slerp(wand.rotation, targetRotation, wandRotationSpeed * Time.deltaTime);
    }
}
