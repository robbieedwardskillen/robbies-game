using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingObjects : MonoBehaviour
{
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision c) {
        if (c.gameObject.tag != "Player"){
            rb.isKinematic=true;
            transform.parent = c.transform;
        }
    }
}
