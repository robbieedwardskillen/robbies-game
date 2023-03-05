using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class findAndAttachToPlayer : MonoBehaviour
{
    GameObject[] players;
    // Start is called before the first frame update
    void Start()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = new Vector3 (players[0].transform.position.x,
        players[0].transform.position.y, players[0].transform.position.z + 3);
    }
}
