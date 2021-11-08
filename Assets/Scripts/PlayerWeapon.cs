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
    public Spell currentPrimarySpell;
    public Spell currentSecondarySpell;
    public Spell[] primarySpellBook = new Spell[5];
    public Spell[] secondarySpellBook = new Spell[3];
    public float wandRotationSpeed = 15;
    private AudioSource audioSource;

    private int primarySpellIndex = 0;
    private int secondarySpellIndex = 0;
    private float[] primarySpellCooldowns;
    private float[] secondarySpellCooldowns;

    private Transform wand;
    private Transform firePoint;
    private Vector3 firePointPos;
    private bool isCastingPrimary = false;
    private bool isCastingSecondary = false;


    // Start is called before the first frame update
    void Awake()
    {
        wand = transform.GetChild(0);
        firePoint = wand.GetChild(0);
        firePointPos = firePoint.localPosition;
        primarySpellCooldowns = new float[primarySpellBook.Length];
        for (int i = 0; i < primarySpellCooldowns.Length; i++)
            primarySpellCooldowns[i] = 0;
        secondarySpellCooldowns = new float[secondarySpellBook.Length];
        for (int i = 0; i < secondarySpellCooldowns.Length; i++)
            secondarySpellCooldowns[i] = 0;

        audioSource = GetComponent<AudioSource>();
        currentPrimarySpell = primarySpellBook[0];
        currentSecondarySpell = secondarySpellBook[0];
    }



    // Update is called once per frame
    void Update()
    {
        MoveWand();
       
        DecrementPrimaryCooldowns();
        DecrementSecondaryCooldowns();

        if (!isCastingPrimary && primarySpellCooldowns[primarySpellIndex] <= 0 && Input.GetMouseButton(0))
        {
            isCastingPrimary = true;
            StartCoroutine(PrimarySpellCasting());
        }
        if (!isCastingSecondary && secondarySpellCooldowns[secondarySpellIndex] <= 0 && Input.GetMouseButton(1))
        {
            isCastingSecondary = true;
            StartCoroutine(SecondarySpellCasting());
        }
        //SetUI();

    }

    IEnumerator PrimarySpellCasting()
    {   
        for (int i = 0; i <= currentPrimarySpell.repetitions; i++)
        {
            CastSpell(currentPrimarySpell);
            yield return new WaitForSeconds(currentPrimarySpell.castingTime);
        }
        primarySpellCooldowns[primarySpellIndex] = currentPrimarySpell.cooldown;
        NextPrimarySpell();
        isCastingPrimary = false;
    }
    IEnumerator SecondarySpellCasting()
    {
        for (int i = 0; i <= currentSecondarySpell.repetitions; i++)
        {
            CastSpell(currentSecondarySpell);
            yield return new WaitForSeconds(currentSecondarySpell.castingTime);
        }
        secondarySpellCooldowns[secondarySpellIndex] = currentSecondarySpell.cooldown;
        NextSecondarySpell();
        isCastingSecondary = false;
    }

    void NextPrimarySpell()
    {
        primarySpellIndex++;
        if (primarySpellIndex >= primarySpellBook.Length)
        {
            primarySpellIndex = 0;
        }
        currentPrimarySpell = primarySpellBook[primarySpellIndex];
    }
    void NextSecondarySpell()
    {
        secondarySpellIndex++;
        if (secondarySpellIndex >= secondarySpellBook.Length)
        {
            secondarySpellIndex = 0;
        }
        currentSecondarySpell = secondarySpellBook[secondarySpellIndex];
    }

    void CastSpell(Spell spell)
    {
        audioSource.Stop();
        SoundEffect sfx = spell.soundEffect;
        audioSource.clip = sfx.sound;
        audioSource.volume = sfx.volume;
        float pitch = Random.Range(sfx.pitch - sfx.pitchVariation, sfx.pitch + sfx.pitchVariation);
        audioSource.pitch = pitch;
        audioSource.Play();


        if (spell is ShootingSpell)
        {
            CastShootingSpell(spell);
        } else if (spell is EnvironmentSpell)
        {
            CastEnvironmentSpell(spell);
        }
    }


    private void CastShootingSpell(Spell currentSpell)
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

    private void CastEnvironmentSpell(Spell currentSpell)
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
            else if (spell.spawnRotation == SpawnRotation.ParallelToCenter)
            {
                spawnRotation.z = Mathf.Atan2(-spawnPosition.y, -spawnPosition.x) * Mathf.Rad2Deg;
            }

            CreateBullet(spell, spell.projectileSettings, Quaternion.Euler(spawnRotation), Random.Range(-spell.rotationOffset / 2, spell.rotationOffset / 2));
        }
        firePoint.localPosition = firePointPos;
        firePoint.localRotation = Quaternion.identity;
    }

    void DecrementPrimaryCooldowns()
    {
        for (int i = 0; i < primarySpellCooldowns.Length; i++)
            primarySpellCooldowns[i] -= Time.deltaTime;
    }
    void DecrementSecondaryCooldowns()
    {
        for (int i = 0; i < secondarySpellCooldowns.Length; i++)
            secondarySpellCooldowns[i] -= Time.deltaTime;
    }

    void CreateBullet(Spell spell, ProjectileSettings projectileSettings, Quaternion rotation, float offset)
    {
        GameObject bullet = ObjectPool.SharedInstance.GetReadyObject("projectile");
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = rotation;
        bullet.transform.eulerAngles += new Vector3(0, 0, offset);
        bullet.transform.localScale = Vector3.one * projectileSettings.size;
        bullet.SetActive(true);
        bullet.GetComponent<ProjectileMover>().SetStats(projectileSettings.timeToBecomeActive, projectileSettings.projectileLifetime, projectileSettings.bulletSpeed, projectileSettings.color, projectileSettings.sprites, projectileSettings.spriteRate, projectileSettings.bounces, projectileSettings.knockback);
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
