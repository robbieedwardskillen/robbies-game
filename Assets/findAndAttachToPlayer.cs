using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.zibra.liquid.Manipulators;

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

        if (this.gameObject.name == "HealWave" + "1"){
            this.transform.position = new Vector3 (players[0].transform.position.x,
            players[0].transform.position.y + 1f, players[0].transform.position.z);
        }

        if (this.gameObject.name == "AttackWave" + "1"){
            this.transform.position = players[0].transform.position + players[0].transform.forward * 2f;
            this.transform.rotation = players[0].transform.rotation;
        }
        
        if (this.gameObject.name == "PlayerLiquidCollider" + "1"){
            this.transform.position = new Vector3 (players[0].transform.position.x,
            players[0].transform.position.y, players[0].transform.position.z);
        }
        
        if (this.gameObject.name == "WaterBall" + "1" || this.gameObject.name == "WaterBallForceField" + "1"){
            this.transform.position = new Vector3(players[0].transform.position.x, players[0].transform.position.y + 2.5f,
            players[0].transform.position.z) + players[0].transform.forward * 6f;
            this.transform.rotation = players[0].transform.rotation;
        }
    }
}
