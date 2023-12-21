using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using com.zibra.liquid.Manipulators;
using UnityEngine.UI;

public class findAndAttachToPlayer : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks
{
    GameObject[] players;
    GameObject player;
    private int instantiationId;
    private int playerID;
    private bool doneSearching = false;
    private bool player1ColliderAttached = true;
    private bool player2ColliderAttached = true;
    private Vector3 startPositionPlayer;
    private Vector3 endPositionLiquidCollider;
    private float elapsedTime = 0f;

    private ZibraLiquidEmitter zle;
    private GameObject playerLiquidCollider;
    private GameObject playerLiquidDetector;
    private GameObject playerLiquidCollider1;
    private GameObject playerLiquidDetector1;
    private GameObject playerLiquidCollider2;
    private GameObject playerLiquidDetector2;

    private List<int> playerList = new List<int>();
    // Start is called before the first frame update
    private PhotonView pv;
    private Player photonPlayer;
    private GameObject canvasInGame;
    private Text debugText;

//just ends up pushing both of them or neither of them
/*  [PunRPC]
	void PushPlayer (int i){
        players[i].GetComponent<Rigidbody>().AddForce((playerLiquidCollider.transform.position - players[i].transform.position) * 25);
    } */

    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != base.photonView) // just an optional step not sure why this helps
            return;
        
        //add checks here


        base.photonView.TransferOwnership(requestingPlayer);
    }
    public void OnOwnershipTransfered(PhotonView targetView, Player previousPlayer)
    {
        if (targetView != base.photonView)
            return;
    }
    public void OnOwnershipTransferFailed(PhotonView targetView, Player player)
    {
        if (targetView != base.photonView)
            return;
    }







    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

		if (stream.IsWriting)
		{
			if (zle != null)
				stream.SendNext((bool)zle.enabled);

		}
		else
		{

			if (this.zle != null)
				this.zle.enabled = (bool) stream.ReceiveNext();

		}

    } 






    void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
        pv = GetComponent<PhotonView>();
        StartCoroutine(WaitThenFindPlayer());

    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    IEnumerator WaitThenFindPlayer(){

        yield return new WaitForSeconds(1);

        players = GameObject.FindGameObjectsWithTag("Player");
        canvasInGame = GameObject.Find("Canvas In Game");
        debugText = canvasInGame.transform.Find("Debugging").GetComponent<Text>();
        playerLiquidCollider1 = GameObject.Find("PlayerLiquidCollider1");
        playerLiquidDetector1 = GameObject.Find("PlayerLiquidDetector1");
        playerLiquidCollider2 = GameObject.Find("PlayerLiquidCollider2");
        playerLiquidDetector2 = GameObject.Find("PlayerLiquidDetector2");
        doneSearching = true;
    }

	public static void DumpToConsole(object obj){
		var output = JsonUtility.ToJson(obj, true);
		Debug.Log(output);
	}


    void Update() //changed from FixedUpdate don't know if it made it worse or better.
    {
        if (GetComponent<ZibraLiquidEmitter>() != null){
            if (zle == null){
                zle = GetComponent<ZibraLiquidEmitter>();
            }

        }

        //int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

       if(doneSearching){
        


                //for ownership transfer later

                //photonPlayer = PhotonView.Get(players[i]) as Player;

            

            playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            //var output2 = JsonUtility.ToJson(PhotonView.Get(players[i]), true);
            //print(PhotonView.Get(players[i]).localPlayerIndex);

            //FIX THIS NONE OF THESE ARE CONNECTING
            //Need to manually set 1 and 2 for each player
            
            playerLiquidCollider = GameObject.Find("PlayerLiquidCollider" + playerID);
            playerLiquidDetector = GameObject.Find("PlayerLiquidDetector" + playerID);

            if (this.gameObject.name == "HealWave1" || this.gameObject.name == "HealWave2"){

                if (this.gameObject.name == "HealWave1" && playerID == 1){
                    this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                    PlayerManager.LocalPlayerInstance.transform.position.y + 1f, PlayerManager.LocalPlayerInstance.transform.position.z);

                } 
                if (this.gameObject.name == "HealWave2" && players.Length > 0 && playerID == 2){
                    this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                    PlayerManager.LocalPlayerInstance.transform.position.y + 1f, PlayerManager.LocalPlayerInstance.transform.position.z);
                }
                
                


            } 
            if (this.gameObject.name == "AttackWave1" || this.gameObject.name == "AttackWave2"){
                
                debugText.text = this.gameObject.name + " - " + playerID;
                //prints attackwave1 - 1 for player 1 attackwave1 - 2 for player 2 should be attackwave2 -2 
                if (this.gameObject.name == "AttackWave1" && playerID == 1){
                    this.transform.parent.gameObject.transform.position = PlayerManager.LocalPlayerInstance.transform.position + PlayerManager.LocalPlayerInstance.transform.forward * 1.5f;
                    this.transform.parent.gameObject.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;//doens't help just change initial velocity of z instead in herarchy Edit: no idea what I meant here
                    this.transform.parent.gameObject.transform.rotation = Quaternion.Euler(PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.x, PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.y, 90f); 
                }
                if (this.gameObject.name == "AttackWave2" && players.Length > 1 && playerID == 2){
                    this.transform.parent.gameObject.transform.position = PlayerManager.LocalPlayerInstance.transform.position + PlayerManager.LocalPlayerInstance.transform.forward * 1.5f;
                    this.transform.parent.gameObject.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
                    this.transform.parent.gameObject.transform.rotation = Quaternion.Euler(PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.x, PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.y, 90f); 
                }


            } 
            if (this.gameObject.name == "PlayerLiquidCollider1" || this.gameObject.name == "PlayerLiquidCollider2"){
                
                if (this.gameObject.name == "PlayerLiquidCollider1" && playerID == 1){
                    if (player1ColliderAttached){
                        this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                            PlayerManager.LocalPlayerInstance.transform.position.y + 0.25f, PlayerManager.LocalPlayerInstance.transform.position.z);
                    } 

                }
                if (this.gameObject.name == "PlayerLiquidCollider2" && players.Length > 1 && playerID == 2){
                    if (player2ColliderAttached){
                        this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                            PlayerManager.LocalPlayerInstance.transform.position.y + 0.25f, PlayerManager.LocalPlayerInstance.transform.position.z);
                    }
                }



            } 
            if (this.gameObject.name == "PlayerLiquidDetector1" || this.gameObject.name == "PlayerLiquidDetector2"){

                if (this.gameObject.name == "PlayerLiquidDetector1" && playerID == 1){
                    this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                    PlayerManager.LocalPlayerInstance.transform.position.y + 0.25f, PlayerManager.LocalPlayerInstance.transform.position.z); 
                } 
                if (this.gameObject.name == "PlayerLiquidDetector2" && players.Length > 1 && playerID == 2){
                    this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                    PlayerManager.LocalPlayerInstance.transform.position.y + 0.25f, PlayerManager.LocalPlayerInstance.transform.position.z); 
                }
               


            } 
            if (this.gameObject.name == "WaterBall1" || this.gameObject.name == "WaterBallForceField1" ||
                       this.gameObject.name == "WaterBall2" || this.gameObject.name == "WaterBallForceField2") {
                if ((this.gameObject.name == "WaterBall1" || this.gameObject.name == "WaterBallForceField1") && playerID == 1){
                    this.transform.position = new Vector3(PlayerManager.LocalPlayerInstance.transform.position.x, PlayerManager.LocalPlayerInstance.transform.position.y + 1.5f,
                    PlayerManager.LocalPlayerInstance.transform.position.z) + PlayerManager.LocalPlayerInstance.transform.forward * 6f;
                    this.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
                } 
                if ((this.gameObject.name == "WaterBall2" || this.gameObject.name == "WaterBallForceField2") && players.Length > 1 && playerID == 2){
                    this.transform.position = new Vector3(PlayerManager.LocalPlayerInstance.transform.position.x, PlayerManager.LocalPlayerInstance.transform.position.y + 1.5f,
                    PlayerManager.LocalPlayerInstance.transform.position.z) + PlayerManager.LocalPlayerInstance.transform.forward * 6f;
                    this.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
                }
                
            }


            if (this.gameObject.name == "PlayerLiquidDetector1" || this.gameObject.name == "PlayerLiquidDetector2"){
                
                if (this.gameObject.name == "PlayerLiquidDetector1"){
                    
                    if (this.gameObject.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0){

                        if (!PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().castingHealingWater){ // healing spells prevent pushback because too much work otherwise
                            player1ColliderAttached = false;
                            PlayerManager.LocalPlayerInstance.GetComponent<Rigidbody>().AddForce((playerLiquidCollider1.transform.position - PlayerManager.LocalPlayerInstance.transform.position) * 25);
                        }
                    }
                    else {
                        player1ColliderAttached = true;
                    }
                }
                if (this.gameObject.name == "PlayerLiquidDetector2"){
                    if (this.gameObject.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0){
                        if (!PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().castingHealingWater){ // healing spells prevent pushback because too much work otherwise
                            player2ColliderAttached = false;
                            PlayerManager.LocalPlayerInstance.GetComponent<Rigidbody>().AddForce((playerLiquidCollider2.transform.position - PlayerManager.LocalPlayerInstance.transform.position) * 25);
                        }
                    }
                    else {
                        player2ColliderAttached = true;
                    }
                }

            }

/* 



            if (playerLiquidDetector1.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0){//not sure how to fix healing spell
                // debugText.text = "first players ID: " + PhotonNetwork.PlayerList[0].ActorNumber + "second players ID: " + PhotonNetwork.PlayerList[1].ActorNumber +  " local player: " + PhotonNetwork.LocalPlayer.ActorNumber
                //+ "Player1 instantiationID: " + int.Parse(PhotonView.Get(players[0]).InstantiationId.ToString().Substring(0, 1))
                //+ "Player2 instantiationID: " + int.Parse(PhotonView.Get(players[1]).InstantiationId.ToString().Substring(0, 1)) ; 

                if (players[0] != null){
                    if (!players[0].GetComponent<PlayerManager>().castingHealingWater){
                       
                        if (PhotonNetwork.PlayerList[0].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber){
                            player1ColliderAttached = false;
                            //debugText.text = " " + PhotonNetwork.PlayerList[0].ActorNumber;
                            //players[0].GetComponent<Rigidbody>().AddForce((playerLiquidCollider1.transform.position - players[0].transform.position) * 25);
                            //if (playerID == 1){
                                //player1ColliderAttached = false;
                            //} else if (playerID == 2) {
                                //player2ColliderAttached = false;
                            //} 
                        }
                    }
                }


                //detach liquid collider so it can move then push character towards that position
                
                //if (elapsedTime > 0.1f){

                //PlayerManager.LocalPlayerInstance.GetComponent<Rigidbody>().AddForce((playerLiquidCollider.transform.position - PlayerManager.LocalPlayerInstance.transform.position) * 25);

            } else {
                player1ColliderAttached = true;
                //elapsedTime = 0f;
                // if (playerID == 1){
                    //player1ColliderAttached = true;
                //} else if (playerID == 2) {
                    //player2ColliderAttached = true;
                //} 
            }
            
            if (playerLiquidDetector2 != null){
                if (playerLiquidDetector2.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0){//not sure how to fix healing spell
                     //debugText.text = "first players ID: " + PhotonNetwork.PlayerList[0].ActorNumber + "second players ID: " + PhotonNetwork.PlayerList[1].ActorNumber +  " local player: " + PhotonNetwork.LocalPlayer.ActorNumber
                    //+ "Player1 instantiationID: " + int.Parse(PhotonView.Get(players[0]).InstantiationId.ToString().Substring(0, 1))
                    //+ "Player2 instantiationID: " + int.Parse(PhotonView.Get(players[1]).InstantiationId.ToString().Substring(0, 1)) ; 

                    if (players[1] != null){
                        if (!players[1].GetComponent<PlayerManager>().castingHealingWater){
                            

                            //***set it to 0 manually need to fix
                            if (PhotonNetwork.PlayerList[1].ActorNumber != PhotonNetwork.LocalPlayer.ActorNumber){
                                debugText.text = "test3";

                                player2ColliderAttached = false;
                                //debugText.text = " " + PhotonNetwork.PlayerList[1].ActorNumber;
                                players[1].GetComponent<Rigidbody>().AddForce((playerLiquidCollider2.transform.position - players[1].transform.position) * 25);

                            }
                        }
                    }

                } else {
                    player2ColliderAttached = true;

                }
            }
 */







            
        }



/*        if(doneSearching){
            for (int i = 0; i < players.Length; i++){



                //for ownership transfer later

                //photonPlayer = PhotonView.Get(players[i]) as Player;

                

                instantiationId = int.Parse(PhotonView.Get(players[i]).InstantiationId.ToString().Substring(0, 1)); 
                //var output2 = JsonUtility.ToJson(PhotonView.Get(players[i]), true);
                //print(PhotonView.Get(players[i]).localPlayerIndex);
                
                
                playerLiquidCollider = GameObject.Find("PlayerLiquidCollider" + instantiationId);
                playerLiquidDetector = GameObject.Find("PlayerLiquidDetector" + instantiationId);

                if (this.gameObject.name == "HealWave" + instantiationId){
                    this.transform.position = new Vector3 (players[i].transform.position.x,
                    players[i].transform.position.y + 1f, players[i].transform.position.z);

                    
                    //pv.TransferOwnership(PhotonView.Get(players[i]));


                } else if (this.gameObject.name == "AttackWave" + instantiationId){
                    this.transform.position = players[i].transform.position + players[i].transform.forward * 1.5f;
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
                    if (playerLiquidDetector.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0 && !players[i].GetComponent<PlayerManager>().castingHealingWater){//not sure how to fix healing spell
                        //elapsedTime += Time.deltaTime;



                        //*** TO FIX
                        //master (2) is unable to find water particles on player 1
                        debugText.text = "Players ID " + PhotonNetwork.LocalPlayer.ActorNumber;







                        //detach liquid collider so it can move then push character towards that position
                        
                        if (instantiationId == 1){
                            player1ColliderAttached = false;
                        } else if (instantiationId == 2) {
                            player2ColliderAttached = false;
                        }





                        //if (elapsedTime > 0.1f){
                            print("instantiation id " + instantiationId);
                            print("i " + i);
                            players[i].GetComponent<Rigidbody>().AddForce((playerLiquidCollider.transform.position - players[i].transform.position) * 25);

                            //players[i].transform.position = Vector3.Lerp(players[i].transform.position, playerLiquidCollider.transform.position, fractionOfDistance);
                        //}


                    } else {
                        //elapsedTime = 0f;
                        if (instantiationId == 1){
                            player1ColliderAttached = true;
                        } else if (instantiationId == 2) {
                            player2ColliderAttached = true;
                        }
                    }
                }


            }
        } */

    }

}
