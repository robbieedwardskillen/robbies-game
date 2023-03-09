using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using com.zibra.liquid.Manipulators;

public class findAndAttachToPlayer : MonoBehaviour
{
    GameObject[] players;
    ZibraLiquidEmitter zle;
    // Start is called before the first frame update

    void Start()
    {

        zle = gameObject.GetComponent<ZibraLiquidEmitter>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (zle != null){
            zle.enabled = false;
        }

        if (this.gameObject.name == "Emitter1"){
            this.transform.position = new Vector3 (players[0].transform.position.x,
            players[0].transform.position.y, players[0].transform.position.z);
        }
        if (this.gameObject.name == "Emitter2"){
            this.transform.position = players[0].transform.position + players[0].transform.forward * 2f;
            this.transform.rotation = players[0].transform.rotation;
        }
        
    }
}
