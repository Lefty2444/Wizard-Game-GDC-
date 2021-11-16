using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameObject gameOverScreen;
    public Image heartImage;

    public float dashForce;
    public float dashTime;
    public ParticleSystem dashDust;

    // Public variables can be seen by other scripts are are visible in the editor
    public float movementSpeed = 150;
    public int startingHearts = 5;


    // Private variables can only be seen by this script
    private int hearts = 0;
    private Rigidbody2D rigidBody;
    private SpriteRenderer sr;
    private Color originalColor;
    private bool invulnerable = false;
    private bool canDash = true;
    private float dashTimer = 1;

    // Start is called before the first frame update
    private void Start()
    {
        hearts = startingHearts;
        InitHearts();
        sr = GetComponent<SpriteRenderer>();
        originalColor = sr.color;
        // Gets the rigidbody component that is responsible for managing the 2D physics of this object
        rigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        // Caculate the movement vector using the horizontal and vertical input axies. Normalizing the input sets the magnitude of the vector as 1.
        Vector3 movementVector = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
        // Add a force based on the movement vector
        rigidBody.AddForce(movementVector * movementSpeed * Time.deltaTime * 120);
        if ((Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0) && (Input.GetButton("Dash") && dashTimer == 1))
        {
            if (canDash)
            {
                StartCoroutine(Dash());
                //Dash();
                canDash = false;
            }
        }
        if (canDash == false)
        {
            dashTimer -= Time.deltaTime / dashTime;
        }
        if (dashTimer <= 0)
        {
            canDash = true;
            dashTimer = 1;
        }

    }

    IEnumerator Dash()
    {
        rigidBody.velocity = new Vector2(rigidBody.velocity.x * dashForce, rigidBody.velocity.y * dashForce);
        {
            invulnerable = true;
            dashDust.Emit(20);
            sr.color = originalColor;
            Color transparentColor = Color.clear;

            for (float t = 0; t < 1; t += Time.deltaTime / (0.3f))
            {
                sr.color = (Mathf.Cos(t * 30) < 0 ? originalColor : transparentColor);
                yield return null;
            }
            sr.color = originalColor;
            invulnerable = false;
        }
    }
    public void UpdateHearts()
    {
        float width = 54 * hearts;
        RectTransform heartsTransform = heartImage.transform as RectTransform;
        heartImage.transform.localPosition = new Vector2(width / 2 - (startingHearts * 27), heartImage.transform.localPosition.y);
        heartsTransform.sizeDelta = new Vector2(width, heartsTransform.sizeDelta.y);
    }
    public void InitHearts()
    {
        float width = 54 * startingHearts;
        RectTransform heartsTransform = heartImage.transform.parent as RectTransform;
        heartImage.transform.parent.localPosition = new Vector2(width / 2 - 135, heartImage.transform.parent.localPosition.y);
        heartsTransform.sizeDelta = new Vector2(width, heartsTransform.sizeDelta.y);
        UpdateHearts();
    }


    public void TakeDamage()
    {
        if (!invulnerable) { 
            hearts--;
            invulnerable = true;
            UpdateHearts();
            if (hearts <= 0 ){
                gameOverScreen.SetActive(true);
                Time.timeScale = 0;
            }
            else
            {
                StartCoroutine(FlashSprite(1f));
            }
        }
    }

    IEnumerator FlashSprite(float flashingTime)
    {
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

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.layer == 8) // An enemy is hit
        {
            TakeDamage();
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        if (collision.gameObject.layer == 8) // An enemy is hit
        {
            TakeDamage();
        }
    }
}
