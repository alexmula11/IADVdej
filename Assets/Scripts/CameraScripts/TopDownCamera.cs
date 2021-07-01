using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopDownCamera : MonoBehaviour
{
    [SerializeField]
    private bool final=false;
    [SerializeField]
    private Vector3 center, lowerLimits, higherLimits;

    [SerializeField]
    private float camSpeed;

    private int hInput, vInput;
    private float xInput;

    void Start()
    {
        transform.position = center;
    }

    //FOR BLOKE 1
    // Update is called once per frame
    /*void Update()
    {
        hInput = 0;
        vInput = 0;
        xInput = 0;
        if (Input.GetKey(KeyCode.A))
            hInput = -1;
        else if (Input.GetKey(KeyCode.D))
            hInput = 1;

        if (Input.GetKey(KeyCode.W))
            vInput = 1;
        else if(Input.GetKey(KeyCode.S))
            vInput = -1;

        if (Input.GetKey(KeyCode.Q))
            xInput = 1;
        else if (Input.GetKey(KeyCode.E))
            xInput = -1;
    }*/

    void Update()
    {
        hInput = 0;
        vInput = 0;
        xInput = 0;
        if (final)
        {
            if (Input.GetKey(KeyCode.A))
                vInput = -1;
            else if (Input.GetKey(KeyCode.D))
                vInput = 1;

            if (Input.GetKey(KeyCode.W))
                hInput = -1;
            else if (Input.GetKey(KeyCode.S))
                hInput = 1;

            if (Input.GetKey(KeyCode.Q))
                xInput = 1;
            else if (Input.GetKey(KeyCode.E))
                xInput = -1;
        }
        else
        {
            if (Input.GetKey(KeyCode.A))
                hInput = -1;
            else if (Input.GetKey(KeyCode.D))
                hInput = 1;

            if (Input.GetKey(KeyCode.W))
                vInput = 1;
            else if (Input.GetKey(KeyCode.S))
                vInput = -1;

            if (Input.GetKey(KeyCode.Q))
                xInput = 1;
            else if (Input.GetKey(KeyCode.E))
                xInput = -1;
        }
    }

    private void FixedUpdate()
    {
        Vector3 movement = Vector3.zero;
        if (hInput < 0 && transform.position.x - Time.fixedDeltaTime * camSpeed >= lowerLimits.x)
        {
            movement += Vector3.left;
        }else if (hInput > 0 && transform.position.x + Time.fixedDeltaTime * camSpeed <= higherLimits.x)
        {
            movement += Vector3.right;
        }
        if (vInput < 0 && transform.position.z - Time.fixedDeltaTime * camSpeed >= lowerLimits.z)
        {
            movement += Vector3.back;
        }
        else if (vInput > 0 && transform.position.z + Time.fixedDeltaTime * camSpeed <= higherLimits.z)
        {
            movement += Vector3.forward;
        }
        if (xInput < 0 && transform.position.y + Time.fixedDeltaTime * camSpeed <= higherLimits.y )
        {
            movement += Vector3.up;
        }
        else if (xInput > 0 && transform.position.y - Time.fixedDeltaTime * camSpeed >= lowerLimits.y)
        {
            movement += Vector3.down;
        }
        transform.position = transform.position + movement.normalized * Time.fixedDeltaTime * camSpeed;
    }
}
