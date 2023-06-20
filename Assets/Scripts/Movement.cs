using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private NodeGrid _nodeGrid;
    private Rigidbody rb;

    [Header("Movement Variables")]
    [SerializeField] private float speed;

    private void Awake()
    {
        _nodeGrid = FindObjectOfType<NodeGrid>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontalInput, 0, verticalInput).normalized;

        if (inputDirection.magnitude >= 0.1)
        {
            Vector3 moveDirection = inputDirection * speed;

            // Check if there is a valid turn in the desired direction
            if (moveDirection.magnitude >= 0.1)
            {
                rb.velocity = moveDirection;
            }
        }
    }
}