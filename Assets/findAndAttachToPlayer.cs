using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using com.zibra.liquid.Manipulators;

public class findAndAttachToPlayer : MonoBehaviour
{
    GameObject[] players;
    private int instantiationId;
    private bool doneSearching = false;
    private bool player1ColliderAttached = true;
    private bool player2ColliderAttached = true;
    private Vector3 startPositionPlayer;
    private Vector3 endPositionLiquidCollider;
    private float elapsedTime = 0f;

    private GameObject playerLiquidCollider;
    private GameObject playerLiquidDetector;

    private List<int> playerList = new List<int>();
    // Start is called before the first frame update

    void Start()
    {
        StartCoroutine(WaitThenFindPlayer());
    }

    IEnumerator WaitThenFindPlayer(){

        yield return new WaitForSeconds(1);

        players = GameObject.FindGameObjectsWithTag("Player");

      
        doneSearching = true;
    }

    // Update is called once per frame
    void Update()
    {
        //int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
       if(doneSearching){
            for (int i = 0; i < players.Length; i++){
/*                 if (!PhotonView.Get(players[i]).IsMine){
                    //don't need is mine here
                    continue;
                }    */
                instantiationId = int.Parse(PhotonView.Get(players[i]).InstantiationId.ToString().Substring(0, 1));   
                //var output2 = JsonUtility.ToJson(PhotonView.Get(players[i]), true);
                //print(PhotonView.Get(players[i]).localPlayerIndex);
                
                
                playerLiquidCollider = GameObject.Find("PlayerLiquidCollider" + instantiationId);
                playerLiquidDetector = GameObject.Find("PlayerLiquidDetector" + instantiationId);

                if (this.gameObject.name == "HealWave" + instantiationId){
                    this.transform.position = new Vector3 (players[i].transform.position.x,
                    players[i].transform.position.y + 1f, players[i].transform.position.z);
                } else if (this.gameObject.name == "AttackWave" + instantiationId){
                    this.transform.position = players[i].transform.position + players[i].transform.forward * 1f;
                    this.transform.rotation = players[i].transform.rotation;//doens't help just change initial velocity of z instead in herarchy
                    this.transform.rotation = Quaternion.Euler(players[i].transform.localRotation.eulerAngles.x, players[i].transform.localRotation.eulerAngles.y, 90f); 
                } else if (this.gameObject.name == "PlayerLiquidCollider" + instantiationId){
                    if (instantiationId == 1 && player1ColliderAttached){
                        
                        this.transform.position = new Vector3 (players[i].transform.position.x,
                            players[i].transform.position.y + 0.25f, players[i].transform.position.z);

                    } else if (instantiationId == 2 && player2ColliderAttached) {
                        this.transform.position = new Vector3 (players[i].transform.position.x,
                            players[i].transform.position.y + 0.25f , players[i].transform.position.z);
                    }

                } else if (this.gameObject.name == "PlayerLiquidDetector" + instantiationId){
                    this.transform.position = new Vector3 (players[i].transform.position.x,
                    players[i].transform.position.y + 0.25f, players[i].transform.position.z); 
                } else if (this.gameObject.name == "WaterBall" + instantiationId || this.gameObject.name == "WaterBallForceField" + instantiationId){
                    this.transform.position = new Vector3(players[i].transform.position.x, players[i].transform.position.y + 1.5f,
                    players[i].transform.position.z) + players[i].transform.forward * 6f;
                    this.transform.rotation = players[i].transform.rotation;
                }





                if (playerLiquidDetector != null){
                    if (playerLiquidDetector.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0 && !players[i].GetComponent<PlayerManager>().castingHealingWater){
                        elapsedTime += Time.deltaTime;

                        //detach liquid collider so it can move then push character towards that position
                        
                        if (instantiationId == 1){
                            player1ColliderAttached = false;
                        } else if (instantiationId == 2) {
                            player2ColliderAttached = false;
                        }

                        if (GetComponent<ZibraLiquidCollider>() != null && elapsedTime > 0.1f){
                            players[i].GetComponent<Rigidbody>().AddForce((playerLiquidCollider.transform.position - players[i].transform.position) * 10);

                            //players[i].transform.position = Vector3.Lerp(players[i].transform.position, playerLiquidCollider.transform.position, fractionOfDistance);
                        }


                    } else {
                        elapsedTime = 0f;
                        if (instantiationId == 1){
                            player1ColliderAttached = true;
                        } else if (instantiationId == 2) {
                            player2ColliderAttached = true;
                        }
                    }
                }


            }
        }

    }

}
