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
        doneSearching = true;
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

      /*   for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++){
            if (players[i]){

            }
        } */

                //for ownership transfer later

                //photonPlayer = PhotonView.Get(players[i]) as Player;

            

            playerID = PhotonNetwork.LocalPlayer.ActorNumber;
            //var output2 = JsonUtility.ToJson(PhotonView.Get(players[i]), true);
            //print(PhotonView.Get(players[i]).localPlayerIndex);
            
            
            playerLiquidCollider = GameObject.Find("PlayerLiquidCollider" + playerID);
            playerLiquidDetector = GameObject.Find("PlayerLiquidDetector" + playerID);

            if (this.gameObject.name == "HealWave" + playerID){
                this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                PlayerManager.LocalPlayerInstance.transform.position.y + 1f, PlayerManager.LocalPlayerInstance.transform.position.z);

                


            } else if (this.gameObject.name == "AttackWave" + playerID){
                this.transform.position = PlayerManager.LocalPlayerInstance.transform.position + PlayerManager.LocalPlayerInstance.transform.forward * 1.5f;
                this.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;//doens't help just change initial velocity of z instead in herarchy
                this.transform.rotation = Quaternion.Euler(PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.x, PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.y, 90f); 
            } else if (this.gameObject.name == "PlayerLiquidCollider" + playerID){
                if (playerID == 1 && player1ColliderAttached){
                    
                    this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                        PlayerManager.LocalPlayerInstance.transform.position.y + 0.25f, PlayerManager.LocalPlayerInstance.transform.position.z);

                } else if (playerID == 2 && player2ColliderAttached) {
                    this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                        PlayerManager.LocalPlayerInstance.transform.position.y + 0.25f , PlayerManager.LocalPlayerInstance.transform.position.z);
                }

            } else if (this.gameObject.name == "PlayerLiquidDetector" + playerID){
                this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                PlayerManager.LocalPlayerInstance.transform.position.y + 0.25f, PlayerManager.LocalPlayerInstance.transform.position.z); 
            } else if (this.gameObject.name == "WaterBall" + playerID || this.gameObject.name == "WaterBallForceField" + playerID){
                this.transform.position = new Vector3(PlayerManager.LocalPlayerInstance.transform.position.x, PlayerManager.LocalPlayerInstance.transform.position.y + 1.5f,
                PlayerManager.LocalPlayerInstance.transform.position.z) + PlayerManager.LocalPlayerInstance.transform.forward * 6f;
                this.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
            }





            if (playerLiquidDetector != null){
                if (playerLiquidDetector.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0 && !PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().castingHealingWater){//not sure how to fix healing spell
                    //elapsedTime += Time.deltaTime;



                    //*** TO FIX
                    //master (2) is unable to find water particles on player 1
                    debugText.text = "Players ID " + PhotonNetwork.LocalPlayer.ActorNumber;







                    //detach liquid collider so it can move then push character towards that position
                    
                    if (playerID == 1){
                        player1ColliderAttached = false;
                    } else if (playerID == 2) {
                        player2ColliderAttached = false;
                    }





                    //if (elapsedTime > 0.1f){
                        PlayerManager.LocalPlayerInstance.GetComponent<Rigidbody>().AddForce((playerLiquidCollider.transform.position - PlayerManager.LocalPlayerInstance.transform.position) * 25);

                        //players[i].transform.position = Vector3.Lerp(players[i].transform.position, playerLiquidCollider.transform.position, fractionOfDistance);
                    //}


                } else {
                    //elapsedTime = 0f;
                    if (playerID == 1){
                        player1ColliderAttached = true;
                    } else if (playerID == 2) {
                        player2ColliderAttached = true;
                    }
                }
            }


            
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
