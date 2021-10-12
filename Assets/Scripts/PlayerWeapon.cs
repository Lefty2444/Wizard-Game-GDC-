using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeapon : MonoBehaviour
{
    public Spell currentSpell;
    public Spell[] spellBook = new Spell[3];
    public float wandRotationSpeed = 15;

    private int spellIndex = 0;
    private float[] spellCooldowns;

    private Transform wand;
    private Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Start is called before the first frame update
    void Awake()
    {
        wand = transform.GetChild(0);
        firePoint = wand.GetChild(0);
        spellCooldowns = new float[spellBook.Length];
        for (int i = 0; i < spellCooldowns.Length; i++)
            spellCooldowns[i] = spellBook[i].cooldown;
    }



    // Update is called once per frame
    void Update()
    {
        MoveWand();
        DecrementCooldowns();

        if (spellCooldowns[spellIndex] <= 0 && Input.GetMouseButton(0))
        {
            CastSpell();
            NextSpell();
        }
        //SetUI();
        
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
        if (currentSpell is ShootingSpell)
        {
            CastShootingSpell();
        }
        
        // Set cooldowns
        spellCooldowns[spellIndex] = currentSpell.cooldown;
    }


    private void CastShootingSpell()
    {
        ShootingSpell spell = (ShootingSpell)currentSpell;
        if (spell.bulletsPerShot <= 1)
            CreateBullet(spell, Random.Range(-spell.arc / 2, spell.arc / 2));
        else
        {
            for (int i = 0; i < spell.bulletsPerShot; i++)
                CreateBullet(spell, Mathf.Lerp(-spell.arc / 2, spell.arc / 2, ((float)i + .5f) / spell.bulletsPerShot));
        }
    }

    void DecrementCooldowns()
    {
        //for (int i = 0; i < spellCooldowns.Length; i++)
        spellCooldowns[spellIndex] -= Time.deltaTime;
    }

    void CreateBullet(ShootingSpell shootingSpell, float angle)
    {
        GameObject bullet = ObjectPool.SharedInstance.GetReadyObject("projectile");
        bullet.transform.position = firePoint.position;
        bullet.transform.rotation = firePoint.rotation;
        bullet.transform.eulerAngles += new Vector3(0, 0, angle);
        bullet.transform.localScale = Vector3.one * shootingSpell.size;
        bullet.SetActive(true);
        bullet.GetComponent<ProjectileMover>().SetStats(shootingSpell.projectileLifetime, shootingSpell.bulletSpeed,
            shootingSpell.damage, shootingSpell.color, shootingSpell.sprites, shootingSpell.spriteRate, shootingSpell.bounces);
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
