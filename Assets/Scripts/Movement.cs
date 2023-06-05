using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private Rigidbody rb;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputdirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (inputdirection.magnitude >= 0.1) 
        {
            Vector3 movedirection = inputdirection * speed;
            rb.velocity = movedirection;
        }
    }
}
