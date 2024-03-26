using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playerC1 : MonoBehaviour
{
    // horizontal movement
    public float xRange = 10.8f;
    public float speed = 10.0f;
    public float midAirSpeed = .5f;

    // jumping
    public float jumpHeightV = 50.0f;
    public float jumpHeightH = 25.0f;
    Rigidbody rBody;
    public bool isOnGround = true;
    public bool chargingJump = false;

    // jumping charge
    public float chargePerSec = 4.0f;
    public float maxCharge = 25;
    public float charge = 0.0f;

    // wall bouncyness
    public float wallBonk = 750.0f;

    // sound
    public AudioSource playerAudio;
    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip pipeSound;
    public AudioClip yipeeSound;
    public AudioClip chargeSound;

    // animation
    public Animator anim;
    public SpriteRenderer sprite;

    // to get boolean from timer script
    public Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // bounds
        if (transform.position.x < -xRange)
        {
            transform.position = new Vector3(-xRange, transform.position.y, transform.position.z);
        }

        if (transform.position.x > xRange)
        {
            transform.position = new Vector3(xRange, transform.position.y, transform.position.z);
        }

        // ---------------walking while on ground------------------------
        // walk right
        if (Input.GetKey(KeyCode.D) && isOnGround && chargingJump == false && timer.TimerOn) 
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
            anim.SetBool("Is Walking", true);
            sprite.flipX = false;
        }
        // stop walking animation
        if (Input.GetKeyUp(KeyCode.D) && isOnGround && chargingJump == false && timer.TimerOn) 
        {
            anim.SetBool("Is Walking", false);
        }
        // walk left
            if (Input.GetKey(KeyCode.A) && isOnGround && chargingJump == false && timer.TimerOn)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
            anim.SetBool("Is Walking", true);
            sprite.flipX = true;
        }
        // stop walking animation
        if (Input.GetKeyUp(KeyCode.A) && isOnGround && chargingJump == false && timer.TimerOn)
        {
            anim.SetBool("Is Walking", false);
        }

        // strafing while mid air
        if (Input.GetKey(KeyCode.D) && isOnGround == false && chargingJump == false && timer.TimerOn)
        {
            transform.Translate(Vector3.right * Time.deltaTime * midAirSpeed);
        }

        if (Input.GetKey(KeyCode.A) && isOnGround == false && chargingJump == false && timer.TimerOn)
        {
            transform.Translate(Vector3.left * Time.deltaTime * midAirSpeed);
        }

        // ------------------JUMPING----------------------- // 

        // Charge Jump
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround && timer.TimerOn)
        {
            // begin charging jump
            chargingJump = true;
            anim.SetBool("Is chargeing ", true);
           // playerAudio.clip = chargeSound;
            //playerAudio.Play();
        }

        // charge jump variable
        if (Input.GetKey(KeyCode.Space) && isOnGround && timer.TimerOn) 
        {
            charge += chargePerSec * Time.deltaTime;
            if (charge > maxCharge)
            {
                charge = maxCharge;
                anim.SetBool("Charge Sneeze Full", true);
            }
        }

        // in case isOnGround gets glitched:
        if (transform.position.y < -2.5) 
        {
            isOnGround = true;
        }

        // release charge
        if (Input.GetKeyUp(KeyCode.Space))
        {
            // begin charging jump
            chargingJump = false;
            charge = 0;
            anim.SetBool("Is chargeing ", false);
            anim.SetBool("Charge Sneeze Full", false);
            //playerAudio.Play();
        }

        // jump up
        if (Input.GetKeyUp(KeyCode.S) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
            anim.SetBool("Is Sneezeing up", true);
            playerAudio.PlayOneShot(yipeeSound, 1.5f);
        }

        // jump right
        if (Input.GetKeyUp(KeyCode.A) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode.Impulse);
            rBody.AddForce(transform.right * charge * jumpHeightH, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
            anim.SetBool("Is sneezing left", true);
            sprite.flipX = true;
            playerAudio.PlayOneShot(yipeeSound, 1.5f);
        }

        // jump left
        if (Input.GetKeyUp(KeyCode.D) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode.Impulse);
            rBody.AddForce(transform.right * charge * -jumpHeightH, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
            anim.SetBool("Is sneezing left", true);
            sprite.flipX = false;
            playerAudio.PlayOneShot(yipeeSound, 1.5f);
        }

        // player death
        if (timer.TimerOn == false) 
        {
            SceneManager.LoadScene("Player1Death");
        }
    }
    // player enviroment collision
    private void OnCollisionEnter(Collision collision)
    {
        // resets jump after landing
        if (collision.gameObject.CompareTag("Ground"))
        {
            playerAudio.PlayOneShot(landSound, 1.0f);
            charge = 0;
            isOnGround = true;
            anim.SetBool("Is Sneezeing up", false);
            anim.SetBool("Is sneezing left", false);
        }

        // makes player bounce off walls
        if (collision.gameObject.CompareTag("Left_Wall")) 
        {
            rBody.AddForce(Vector3.right * wallBonk, ForceMode.Impulse);
            anim.SetBool("Is sneezing left", true);
            anim.SetBool("Is Sneezeing up", false);
            sprite.flipX = true;
            playerAudio.PlayOneShot(landSound, 2.0f);
        }
        if (collision.gameObject.CompareTag("Right_Wall"))
        {
            rBody.AddForce(Vector3.right * -wallBonk, ForceMode.Impulse);
            anim.SetBool("Is sneezing left", true);
            anim.SetBool("Is Sneezeing up", false);
            sprite.flipX = false;
            playerAudio.PlayOneShot(landSound, 2.0f);
        }
        if (collision.gameObject.CompareTag("Top_Wall"))
        {
            rBody.AddForce(Vector3.down * wallBonk, ForceMode.Impulse);
            anim.SetBool("Is Sneezeing up", true);
            anim.SetBool("Is sneezing left", false);
            playerAudio.PlayOneShot(landSound, 2.0f);
        }
    }

    // tissue pickup
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pickup"))
        {
            // add time to timer
            Destroy(other.gameObject);
            timer.PowerUp();
            // maybe use this for how long till next one spawns->
            // StartCoroutine(goBack());
        }
    }
}
