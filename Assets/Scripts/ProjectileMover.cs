using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    [HideInInspector]
    public float damage;
  
    private SpriteRenderer sr;
    private Rigidbody2D rb;

    private float speed;
    private Sprite[] sprites;
    private float timeLeft;
    private int currentIndex;
    private float spriteRate;
    private ParticleSystem particles;
    private int bounces;
    // Start is called before the first frame update
    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void SetStats(float lifetime, float speed, float damage, Color color, Sprite[] sprites, float spriteRate, int bounces)
    {
        StartCoroutine(StartFade(lifetime));
        rb.AddForce(transform.up * speed * 100);
        this.damage = damage;
        this.bounces = bounces;

        sr.color = color;

        
        if (sprites.Length > 0)
            sr.sprite = sprites[0];
        this.sprites = sprites;
        if (sprites.Length > 0)
            this.spriteRate = spriteRate;
        else
            this.spriteRate = 0;

        
        if (GetComponent<ParticleSystem>())
        {
            particles = GetComponent<ParticleSystem>();
            var main = particles.main;
            main.startColor = color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position -= transform.up * Time.deltaTime * speed;
       
        AnimateSprite();

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
        }
    }

    private void AnimateSprite()
    {
        if (spriteRate == 0)
            return;
        else
        {
            currentIndex++;
            if (currentIndex >= sprites.Length)
                currentIndex = 0;
            sr.sprite = sprites[currentIndex];
            timeLeft = 1 / spriteRate;
        }
    }

    IEnumerator StartFade(float time)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / (time * .75f))
            yield return null;
        Color startColor = sr.color;
        Color endColor = startColor;
        endColor.a = .2f;
        for (float t = 0; t < 1; t += Time.deltaTime / (time * .25f))
        {
            sr.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        KillMe();
    }
    void KillMe()
    {
        //GetComponent<Collider2D>().enabled = false;
        this.gameObject.SetActive(false);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        bounces--;
        if (bounces < 0)
        {
            //KillMe();X
        }
    }
}
