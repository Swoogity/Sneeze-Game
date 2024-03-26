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
    public AudioClip pipe;

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

        // walking while on ground
        if (Input.GetKey(KeyCode.D) && isOnGround && chargingJump == false && timer.TimerOn) 
        {
            transform.Translate(Vector3.right * Time.deltaTime * speed);
        }

        if (Input.GetKey(KeyCode.A) && isOnGround && chargingJump == false && timer.TimerOn)
        {
            transform.Translate(Vector3.left * Time.deltaTime * speed);
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
        }

        // charge jump variable
        if (Input.GetKey(KeyCode.Space) && isOnGround && timer.TimerOn) 
        {
            charge += chargePerSec * Time.deltaTime;
            if (charge > maxCharge)
            {
                charge = maxCharge;
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
        }

        // jump up
        if (Input.GetKeyUp(KeyCode.S) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
        }

        // jump right
        if (Input.GetKeyUp(KeyCode.A) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode.Impulse);
            rBody.AddForce(transform.right * charge * jumpHeightH, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
        }

        // jump left
        if (Input.GetKeyUp(KeyCode.D) && isOnGround && chargingJump && timer.TimerOn)
        {
            rBody.AddForce(transform.up * charge * jumpHeightV, ForceMode.Impulse);
            rBody.AddForce(transform.right * charge * -jumpHeightH, ForceMode.Impulse);
            playerAudio.PlayOneShot(jumpSound, 1.0f);
            isOnGround = false;
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
        }

        // makes player bounce off walls
        if (collision.gameObject.CompareTag("Left_Wall")) 
        {
            rBody.AddForce(Vector3.right * wallBonk, ForceMode.Impulse);
        }
        if (collision.gameObject.CompareTag("Right_Wall"))
        {
            rBody.AddForce(Vector3.right * -wallBonk, ForceMode.Impulse);
        }
        if (collision.gameObject.CompareTag("Top_Wall"))
        {
            rBody.AddForce(Vector3.down * wallBonk, ForceMode.Impulse);
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
