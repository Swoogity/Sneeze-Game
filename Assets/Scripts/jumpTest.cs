using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class jumpTest : MonoBehaviour
{
    public float jumpHeight = 50.0f;
    Rigidbody rBody;

    // Start is called before the first frame update
    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            // begin charging jump
            
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            // release jump
            rBody.AddForce(transform.up * jumpHeight, ForceMode.Impulse);
        }
    }
}
