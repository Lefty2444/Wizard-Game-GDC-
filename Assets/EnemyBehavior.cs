using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public int lives;
    public float lungeStrength = 5;
    public float lungeDelay = 2;


    private Rigidbody2D rb;
    private Transform player;
    private SpriteRenderer sr;
    private Color originalColor;
    private bool invulnerable = false;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMovement>().transform;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        if (lungeDelay != 0)
            InvokeRepeating("Lunge", Random.Range(1f, 1f+lungeDelay), lungeDelay);
    }

    // Update is called once per frame
    void Lunge()
    {
        float variation = Random.Range(4f, 6f);
        rb.velocity = (player.position - transform.position).normalized * variation * lungeStrength;
    }

    public void TakeDamage()
    {
        if (!invulnerable)
        {
            lives--;
            if (lives <= 0)
            {
                rb.simulated = false;
                StartCoroutine(Death(.3f));
            }
            else
            {
                StartCoroutine(FlashSprite(1f));
            }
        }
    }

    IEnumerator Death(float time)
    {
        CancelInvoke();
        GetComponent<Collider2D>().enabled = false;
        Vector3 startScale = transform.localScale;
        for (float t = 0; t < 1; t += Time.deltaTime / (time))
        {
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            yield return null;
        }
    }

    IEnumerator FlashSprite(float flashingTime)
    {
        invulnerable = true;
        sr.color = originalColor;
        Color transparentColor = Color.red;

        for (float t = 0; t < 1; t += Time.deltaTime / (flashingTime))
        {
            sr.color = (Mathf.Cos(t * 30) < 0 ? originalColor : transparentColor);
            yield return null;
        }
        sr.color = originalColor;
        invulnerable = false;
    }
}
