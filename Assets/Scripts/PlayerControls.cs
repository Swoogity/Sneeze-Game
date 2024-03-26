using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControls : MonoBehaviour
{
    // ----------------------------------------------------------------VARIABLE---------------------------------------------------------------//
    // Keybinds
    public KeyCode MoveLeft;
    public KeyCode MoveRight;
    public KeyCode StartJump;
    public KeyCode StraightJump;

    // horizontal movement
    public float speed = 10.0f;
    public float midAirSpeed = .5f;

    // jumping
    public float jumpHeightV = 50.0f;
    public float jumpHeightH = 25.0f;
    Rigidbody2D rBody;
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
    public AudioSource chargeAudio;

    public AudioClip jumpSound;
    public AudioClip landSound;
    public AudioClip pipeSound;
    public AudioClip yipeeSound;

    // animation
    public Animator anim;
    public SpriteRenderer sprite;

    // to get boolean from timer script
    public Timer timer;

    // 
    public string DeathScene;

    // -----------------------------------------------------------START---------------------------------------------------------//
    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody2D>();
    }

    // -----------------------------------------------------------UPDATE---------------------------------------------------------//
    // Update is called once per frame
    void Update()
    {
        // ----------------------------------------WALKING-------------------------------------------//
        // walk right
        if (Input.GetKey(MoveRight) && isOnGround && chargingJump == false && timer.TimerOn)
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
            anim.SetBool("Is Walking", true);
            sprite.flipX = false;
        }
        // stop walking animation
        if (Input.GetKeyUp(MoveRight) && isOnGround && chargingJump == false && timer.TimerOn)
        {
            anim.SetBool("Is Walking", false);
        }
        // walk left
        if (Input.GetKey(MoveLeft) && isOnGround && chargingJump == false && timer.TimerOn)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
            anim.SetBool("Is Walking", true);
            sprite.flipX = true;
        }
        // stop walking animation
        if (Input.GetKeyUp(MoveLeft) && isOnGround && chargingJump == false && timer.TimerOn)
        {
            anim.SetBool("Is Walking", false);
        }

        // ----------------------------------------STRAIFING-------------------------------------------//
        if (Input.GetKey(MoveRight) && isOnGround == false && chargingJump == false && timer.TimerOn)
        {
            transform.Translate(Vector3.right * Time.deltaTime * midAirSpeed);
        }

        if (Input.GetKey(MoveLeft) && isOnGround == false && chargingJump == false && timer.TimerOn)
        {
            transform.Translate(Vector3.left * Time.deltaTime * midAirSpeed);
        }

        // ----------------------------------------JUMPING-------------------------------------------//
        // Charge Jump
        if (Input.GetKeyDown(StartJump) && isOnGround && timer.TimerOn)
        {
            // begin charging jump
            chargingJump = true;
            anim.SetBool("Is chargeing ", true);
            chargeAudio.Play();
            // playerAudio.clip = chargeSound;
            //playerAudio.Play();
        }

        // charge jump variable
        if (Input.GetKey(StartJump) && isOnGround && timer.TimerOn)
        {
            charge += chargePerSec * Time.deltaTime;
            if (charge > maxCharge)
            {
                charge = maxCharge;
                anim.SetBool("Charge Sneeze Full", true);
            }
        }

        // release charge
        if (Input.GetKeyUp(StartJump))
        {
            // begin charging jump
            chargingJump = false;
            charge = 0;
            anim.SetBool("Is chargeing ", false);
            anim.SetBool("Charge Sneeze Full", false);
            chargeAudio.Stop();
            //playerAudio.Play();
        }

        // jump up
        if (Input.GetKeyUp(StraightJump) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode2D.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
            anim.SetBool("Is Sneezeing up", true);
            playerAudio.PlayOneShot(yipeeSound, 1.5f);
            chargeAudio.Stop();
        }

        // jump right
        if (Input.GetKeyUp(MoveLeft) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode2D.Impulse);
            rBody.AddForce(transform.right * charge * jumpHeightH, ForceMode2D.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
            anim.SetBool("Is sneezing left", true);
            sprite.flipX = true;
            playerAudio.PlayOneShot(yipeeSound, 1.5f);
            chargeAudio.Stop();
        }

        // jump left
        if (Input.GetKeyUp(MoveRight) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode2D.Impulse);
            rBody.AddForce(transform.right * charge * -jumpHeightH, ForceMode2D.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
            anim.SetBool("Is sneezing left", true);
            sprite.flipX = false;
            playerAudio.PlayOneShot(yipeeSound, 1.5f);
            chargeAudio.Stop();
        }

        // ----------------------------------------PLAYER DEATH-------------------------------------------//
        if (timer.TimerOn == false)
        {
            SceneManager.LoadScene(DeathScene);
        }
    }
    // ----------------------------------------PLAYER ENVIROMENT COLLISION-------------------------------------------//
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        // resets jump after landing
        if (collision2D.gameObject.CompareTag("Ground"))
        {
            playerAudio.PlayOneShot(landSound, 1.0f);
            charge = 0;
            isOnGround = true;
            anim.SetBool("Is Sneezeing up", false);
            anim.SetBool("Is sneezing left", false);
        }

        // makes player bounce off walls
        if (collision2D.gameObject.CompareTag("Left_Wall"))
        {
            rBody.AddForce(Vector3.right * wallBonk, ForceMode2D.Impulse);
            anim.SetBool("Is sneezing left", true);
            anim.SetBool("Is Sneezeing up", false);
            sprite.flipX = true;
            playerAudio.PlayOneShot(landSound, 2.0f);
        }
        if (collision2D.gameObject.CompareTag("Right_Wall"))
        {
            rBody.AddForce(Vector3.right * -wallBonk, ForceMode2D.Impulse);
            anim.SetBool("Is sneezing left", true);
            anim.SetBool("Is Sneezeing up", false);
            sprite.flipX = false;
            playerAudio.PlayOneShot(landSound, 2.0f);
        }
        if (collision2D.gameObject.CompareTag("Top_Wall"))
        {
            rBody.AddForce(Vector3.down * wallBonk, ForceMode2D.Impulse);
            anim.SetBool("Is Sneezeing up", true);
            anim.SetBool("Is sneezing left", false);
            playerAudio.PlayOneShot(landSound, 2.0f);
        }
    }

    // ----------------------------------------TISSUE PICKUP-------------------------------------------//
    private void OnTriggerEnter2D(Collider2D other)
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
