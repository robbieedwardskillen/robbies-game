using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class debugCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider col) {
        print(col.gameObject.name);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
