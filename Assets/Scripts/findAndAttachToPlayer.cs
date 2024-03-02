using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using com.zibra.liquid.Manipulators;
using UnityEngine.UI;

//All water manipulator management

public class findAndAttachToPlayer : MonoBehaviourPunCallbacks, IPunObservable, IPunOwnershipCallbacks
{
    GameObject[] players;
    GameObject player;
    public bool attached = false;
    private int instantiationId;
    private int playerID;
    private bool doneSearching = false;
    private bool player1ColliderAttached = true;
    private bool player2ColliderAttached = true;
    private float elapsedTime = 0f;

    private ZibraLiquidEmitter zle;
    public GameObject playerLiquidDetector1;
    public GameObject playerLiquidDetector2;
    private GameObject playerLiquidCollider1;
    private GameObject playerLiquidCollider2;


    private List<int> playerList = new List<int>();
    // Start is called before the first frame update
    private PhotonView pv;
    private Player photonPlayer;
    private GameObject canvasInGame;
    private ManageCanvas canvasManager;
    private Text debugText;
    private Text debugText2;
    private bool transferredOwnership1 = false;
    private bool transferredOwnership2 = false;




    public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
    {
        if (targetView != base.photonView)
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
			if (zle != null){
                stream.SendNext((bool)player1ColliderAttached);
                stream.SendNext((bool)player2ColliderAttached);
                stream.SendNext((bool)zle.enabled);
            }
				
		}
		else
		{
			if (this.zle != null){
                this.player1ColliderAttached = (bool) stream.ReceiveNext();
                this.player2ColliderAttached = (bool) stream.ReceiveNext();
                this.zle.enabled = (bool) stream.ReceiveNext();
            }


		}

    } 


    void Start()
    {
        PhotonNetwork.AddCallbackTarget(this);
        StartCoroutine(WaitThenFindPlayer());

    }

    private void OnDestroy()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    IEnumerator WaitThenFindPlayer(){
        canvasManager = GameObject.Find("Canvas Manager").GetComponent<ManageCanvas>();
        canvasManager.attached = false;
        yield return new WaitForSeconds(1.75f);
        canvasManager.attached = true;
        players = GameObject.FindGameObjectsWithTag("Player");
        canvasInGame = GameObject.Find("Canvas In Game");

        debugText = canvasInGame.transform.Find("Debugging").GetComponent<Text>();
        debugText2 = canvasInGame.transform.Find("Debugging2").GetComponent<Text>();
        playerLiquidCollider1 = GameObject.Find("PlayerLiquidCollider1");
        playerLiquidDetector1 = GameObject.Find("PlayerLiquidDetector1");
        playerLiquidCollider2 = GameObject.Find("PlayerLiquidCollider2");
        playerLiquidDetector2 = GameObject.Find("PlayerLiquidDetector2");
        doneSearching = true;
    }

	[PunRPC]
	void player1Attached (bool attached){
        player1ColliderAttached = attached;
    }
    [PunRPC]
    void player2Attached (bool attached){
        player2ColliderAttached = attached;
    }

    void Update() 
    {
        if (GetComponent<ZibraLiquidEmitter>() != null){
            if (zle == null){
                zle = GetComponent<ZibraLiquidEmitter>();
            }
        }


       if(doneSearching){
            //var output2 = JsonUtility.ToJson(PhotonView.Get(players[i]), true);
            playerID = PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().playerID;

            if (!transferredOwnership1){
                if (this.gameObject.name[gameObject.name.Length-1] == '1' && playerID == 1){
                    OnOwnershipRequest(photonView, PhotonNetwork.LocalPlayer);

                    if (this.gameObject.name[gameObject.name.Length-1] == '1'){
                        transferredOwnership1 = true;
                    }   
                    else if (this.gameObject.name[gameObject.name.Length-1] == '2'){
                        transferredOwnership2 = true;
                    }
                }
                    
            }
            if (!transferredOwnership2){
                if (this.gameObject.name[gameObject.name.Length-1] == '2' && playerID == 2){
                    OnOwnershipRequest(photonView, PhotonNetwork.LocalPlayer);
                    
                    if (this.gameObject.name[gameObject.name.Length-1] == '1'){
                        transferredOwnership1 = true;
                    }  
                    else if (this.gameObject.name[gameObject.name.Length-1] == '2'){
                        transferredOwnership2 = true;
                    }
                }
                    
            }




            if (this.gameObject.name == "HealWave1" || this.gameObject.name == "HealWave2"){

                if (this.gameObject.name == "HealWave1" && playerID == 1){
                    this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                    PlayerManager.LocalPlayerInstance.transform.position.y + 1f, PlayerManager.LocalPlayerInstance.transform.position.z);

                } 
                if (this.gameObject.name == "HealWave2" && playerID == 2){
                    this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                    PlayerManager.LocalPlayerInstance.transform.position.y + 1f, PlayerManager.LocalPlayerInstance.transform.position.z);
                }

            } 


            if (this.gameObject.name == "AttackWave1" || this.gameObject.name == "AttackWave2"){

                if (this.gameObject.name == "AttackWave1" && playerID == 1){
                    this.transform.parent.gameObject.transform.position = PlayerManager.LocalPlayerInstance.transform.position + PlayerManager.LocalPlayerInstance.transform.forward * 1.5f;
                    this.transform.parent.gameObject.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;//doens't help just change initial velocity of z instead in herarchy Edit: no idea what I meant here
                    this.transform.parent.gameObject.transform.rotation = Quaternion.Euler(PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.x, PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.y, 90f); 
                }
                else if (this.gameObject.name == "AttackWave2" && playerID == 2){
                    this.transform.parent.gameObject.transform.position = PlayerManager.LocalPlayerInstance.transform.position + PlayerManager.LocalPlayerInstance.transform.forward * 1.5f;
                    this.transform.parent.gameObject.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;//doens't help just change initial velocity of z instead in herarchy Edit: no idea what I meant here
                    this.transform.parent.gameObject.transform.rotation = Quaternion.Euler(PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.x, PlayerManager.LocalPlayerInstance.transform.localRotation.eulerAngles.y, 90f); 
                }

            
                

            } 
            if (this.gameObject.name == "PlayerLiquidCollider1" || this.gameObject.name == "PlayerLiquidCollider2"){
                
                if (this.gameObject.name == "PlayerLiquidCollider1" && playerID == 1){
                    if (player1ColliderAttached){
                        //bad idea
/*                         this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
                        this.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f,0f,0f); */
                        this.transform.position = new Vector3 (PlayerManager.LocalPlayerInstance.transform.position.x,
                            PlayerManager.LocalPlayerInstance.transform.position.y + 0.25f, PlayerManager.LocalPlayerInstance.transform.position.z);
                    } 

                }
                if (this.gameObject.name == "PlayerLiquidCollider2" && playerID == 2){
                    if (player2ColliderAttached){
/*                         this.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f,0f,0f);
                        this.gameObject.GetComponent<Rigidbody>().angularVelocity = new Vector3(0f,0f,0f); */
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
                if (this.gameObject.name == "PlayerLiquidDetector2" && playerID == 2){
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
                if ((this.gameObject.name == "WaterBall2" || this.gameObject.name == "WaterBallForceField2") && playerID == 2){
                    this.transform.position = new Vector3(PlayerManager.LocalPlayerInstance.transform.position.x, PlayerManager.LocalPlayerInstance.transform.position.y + 1.5f,
                    PlayerManager.LocalPlayerInstance.transform.position.z) + PlayerManager.LocalPlayerInstance.transform.forward * 6f;
                    this.transform.rotation = PlayerManager.LocalPlayerInstance.transform.rotation;
                }
                
            }

            if (this.gameObject.name == "PlayerLiquidDetector1" || this.gameObject.name == "PlayerLiquidDetector2"){




                debugText.text = player1ColliderAttached.ToString();
                debugText2.text = player2ColliderAttached.ToString();


                ///***************************
                //LEFT OFF HERE

                //player1Collider does not move
                //even when not attached it is still at player position


                if (this.gameObject.name == "PlayerLiquidDetector1" && playerID == 1){
                    
                    if (this.gameObject.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0){


                        if (!PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().castingHealingWater){ // healing spells prevent pushback because too much work otherwise
                            photonView.RPC("player1Attached", RpcTarget.All, false);
                            player1ColliderAttached = false;
                            
                            //player1ColliderAttached = false;
                            PlayerManager.LocalPlayerInstance.GetComponent<Rigidbody>().AddForce(new Vector3(playerLiquidCollider1.transform.position.x, 2f, playerLiquidCollider1.transform.position.z)
                                - new Vector3(PlayerManager.LocalPlayerInstance.transform.position.x, 2f, PlayerManager.LocalPlayerInstance.transform.position.z) * 5);
                        }   
                        
                    }
                    else {
                        photonView.RPC("player1Attached", RpcTarget.All, true);
                        player1ColliderAttached = true;
                    }
                }

                if (this.gameObject.name == "PlayerLiquidDetector2" && playerID == 2){
                    if (this.gameObject.GetComponent<ZibraLiquidDetector>().ParticlesInside > 0){


                        if (!PlayerManager.LocalPlayerInstance.GetComponent<PlayerManager>().castingHealingWater){ // healing spells prevent pushback because too much work otherwise
                            photonView.RPC("player2Attached", RpcTarget.All, false);
                            player2ColliderAttached = false;
                            //player2ColliderAttached = false;
                            PlayerManager.LocalPlayerInstance.GetComponent<Rigidbody>().AddForce((new Vector3(playerLiquidCollider2.transform.position.x, 0f, playerLiquidCollider2.transform.position.z)
                                - new Vector3(PlayerManager.LocalPlayerInstance.transform.position.x, 0f, PlayerManager.LocalPlayerInstance.transform.position.z)) * 500);
                        }
                                            
                    }
                    else {
                        photonView.RPC("player2Attached", RpcTarget.All, true);
                        player2ColliderAttached = true;
                    }
                }

            }


            
        }


    }

}
