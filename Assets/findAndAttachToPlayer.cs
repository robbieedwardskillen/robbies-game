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
    bool emitting = false;
    void Start()
    {
        zle = gameObject.GetComponent<ZibraLiquidEmitter>();
        players = GameObject.FindGameObjectsWithTag("Player");
    }

    IEnumerator healWaveTime() {
        if (zle != null){
            emitting = true;
            zle.enabled = true;
            yield return new WaitForSeconds(0.25f);
            emitting = false;
            zle.enabled = false;
        }
    }
    IEnumerator attackWaveTime() {
        if (zle != null){
            emitting = true;
            zle.enabled = true;
            yield return new WaitForSeconds(0.5f);
            emitting = false;
            zle.enabled = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (zle != null){
            if (emitting == false)
            zle.enabled = false;
        } // need to check if player1
        if (gameObject.name == "HealWave" + ""){
            StartCoroutine(healWaveTime());
        }
        if (gameObject.name == "AttackWave" + ""){
            StartCoroutine(attackWaveTime());
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
