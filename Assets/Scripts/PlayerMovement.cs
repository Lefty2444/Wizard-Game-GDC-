using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public GameObject gameOverScreen;

    // Public variables can be seen by other scripts are are visible in the editor
    public float movementSpeed = 150;
    public int hearts = 5;

    // Private variables can only be seen by this script
    private Rigidbody2D rigidBody;
    private SpriteRenderer sr;
    private Color originalColor;
    private bool invulnerable = false;

    // Start is called before the first frame update
    private void Start()
    {
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
    }


    public void TakeDamage()
    {
        if (!invulnerable) { 
            hearts--;
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
