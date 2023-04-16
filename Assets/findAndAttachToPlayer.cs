using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

using com.zibra.liquid.Manipulators;

public class findAndAttachToPlayer : MonoBehaviour
{
    GameObject[] players;
    private int playerId;
    private bool doneSearching = false;
    private bool player1ColliderAttached = true;
    private bool player2ColliderAttached = true;
    private Vector3 startPositionPlayer;
    private Vector3 endPositionLiquidCollider;
    private float elapsedTime = 0f;

    private GameObject playerLiquidCollider;
    private GameObject playerLiquidDetector;
    // Start is called before the first frame update
    IEnumerator WaitThenFindPlayer(){
        yield return new WaitForSeconds(1);
        players = GameObject.FindGameObjectsWithTag("Player");
        playerId = PhotonNetwork.LocalPlayer.ActorNumber;
        playerLiquidCollider = GameObject.Find("PlayerLiquidCollider" + playerId);
        playerLiquidDetector = GameObject.Find("PlayerLiquidDetector" + playerId);
        if (this.GetComponent<ZibraLiquidCollider>() != null){
            this.transform.position = new Vector3 (players[playerId-1].transform.position.x,
                players[playerId-1].transform.position.y + 0.25f, players[playerId-1].transform.position.z);

        }
        doneSearching = true;
    }
    void Start()
    {
        StartCoroutine(WaitThenFindPlayer());
        
    }
    void OnCollisionEnter(Collision col){

    }

    // Update is called once per frame
    void Update()
    {
        if(doneSearching){
        
            if (this.gameObject.name == "HealWave" + playerId){
                this.transform.position = new Vector3 (players[playerId-1].transform.position.x,
                players[playerId-1].transform.position.y + 1f, players[playerId-1].transform.position.z);
            }

            if (this.gameObject.name == "AttackWave" + playerId){
                this.transform.position = players[playerId-1].transform.position + players[playerId-1].transform.forward * 1f;
                this.transform.rotation = players[playerId-1].transform.rotation;//doens't help just change initial velocity of z instead in herarchy
                this.transform.rotation = Quaternion.Euler(players[playerId-1].transform.localRotation.eulerAngles.x, players[playerId-1].transform.localRotation.eulerAngles.y, 90f); 
            }
            
            if (this.gameObject.name == "PlayerLiquidCollider" + playerId){
                if (playerId - 1 == 0 && player1ColliderAttached){
                    
                    this.transform.position = new Vector3 (players[playerId-1].transform.position.x,
                        players[playerId-1].transform.position.y + 0.25f, players[playerId-1].transform.position.z);

                } else if (playerId - 1 == 1 && player2ColliderAttached) {
                    this.transform.position = new Vector3 (players[playerId-1].transform.position.x,
                        players[playerId-1].transform.position.y + 0.25f , players[playerId-1].transform.position.z);
                }
/*                 this.transform.position = new Vector3 (players[playerId-1].transform.position.x,
                players[playerId-1].transform.position.y, players[playerId-1].transform.position.z); */
            }
              if (this.gameObject.name == "PlayerLiquidDetector" + playerId){
              this.transform.position = new Vector3 (players[playerId-1].transform.position.x,
                players[playerId-1].transform.position.y + 0.25f, players[playerId-1].transform.position.z); 
            }
            if (this.gameObject.name == "WaterBall" + playerId || this.gameObject.name == "WaterBallForceField" + playerId){
                this.transform.position = new Vector3(players[playerId-1].transform.position.x, players[playerId-1].transform.position.y + 1.5f,
                players[playerId-1].transform.position.z) + players[playerId-1].transform.forward * 6f;
                this.transform.rotation = players[playerId-1].transform.rotation;
            }





            if (playerLiquidDetector.GetComponent<ZibraLiquidDetector>() != null){
                if (playerLiquidDetector.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0 && !players[playerId-1].GetComponent<PlayerManager>().castingHealingWater){
                    elapsedTime += Time.deltaTime;

                    //detach liquid collider so it can move then push character towards that position
                    
                    if (playerId - 1 == 0){
                        player1ColliderAttached = false;
                    } else if (playerId - 1 == 1) {
                        player2ColliderAttached = false;
                    }

                    if (GetComponent<ZibraLiquidCollider>() != null && elapsedTime > 0.1f){
                        players[playerId-1].GetComponent<Rigidbody>().AddForce((playerLiquidCollider.transform.position - players[playerId-1].transform.position) * 10);

                        //players[playerId-1].transform.position = Vector3.Lerp(players[playerId-1].transform.position, playerLiquidCollider.transform.position, fractionOfDistance);
                    }


                } else {
                    elapsedTime = 0f;
                    if (playerId - 1 == 0){
                        player1ColliderAttached = true;
                    } else if (playerId - 1 == 1) {
                        player2ColliderAttached = true;
                    }
                }
            }

        }

    }


}
