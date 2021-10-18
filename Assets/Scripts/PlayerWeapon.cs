using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum Area { allEdge, upperEdge, lowerEdge, leftEdge, rightEdge }
[System.Serializable]
public class SceneBounds
{
    public float upper;
    public float lower;
    public float left;
    public float right;

    

    public Vector2 GetPosition(Area area)
    {
        float x = Random.Range(left, right);
        float y = Random.Range(lower, upper);
        switch (area)
        {
            case Area.upperEdge:
                return new Vector2(x, y);
            default:
                return Vector2.zero;
        }

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
        firePointPos = firePoint.position;
        spellCooldowns = new float[spellBook.Length];
        for (int i = 0; i < spellCooldowns.Length; i++)
            spellCooldowns[i] = 0;
        audioSource = GetComponent<AudioSource>();
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
            CreateBullet(spell, spell.projectileSettings, Random.Range(-spell.arc / 2, spell.arc / 2));
        else
        {
            for (int i = 0; i < spell.bulletsPerShot; i++)
                CreateBullet(spell, spell.projectileSettings, Mathf.Lerp(-spell.arc / 2, spell.arc / 2, ((float)i + .5f) / spell.bulletsPerShot));
        }
    }

    private void CastEnvironmentSpell()
    {
        EnvironmentSpell spell = (EnvironmentSpell)currentSpell;
        for (int i = 0; i < spell.projectilesPerWave; i++)
        {
            if (spell.spawnArea == SpawnAreas.LeftEdge)
            {
                
            }
            CreateBullet(spell, spell.projectileSettings, Random.Range(-spell.rotationOffset / 2, spell.rotationOffset / 2));
        }
        firePoint.position = firePointPos;
        firePoint.rotation = Quaternion.identity;
    }

    void DecrementCooldowns()
    {
        for (int i = 0; i < spellCooldowns.Length; i++)
            spellCooldowns[i] -= Time.deltaTime;
    }

    void CreateBullet(Spell spell, ProjectileSettings projectileSettings, float angle)
    {
        GameObject bullet = ObjectPool.SharedInstance.GetReadyObject("projectile");
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.transform.eulerAngles += new Vector3(0, 0, angle);
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
