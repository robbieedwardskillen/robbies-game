using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;
    

    public bool connected = false;
    
    private Transition gameTransition;

    void Awake() {
        gameTransition = GameObject.Find("Main Camera").GetComponent<Transition>();
    }

    void Start()
    {
        //try to connect
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        //we connected
        Debug.Log("Connected to Master");
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Joined a room successfully!");
        PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(5f, 41f, 5f), Quaternion.identity);
        //base.OnJoinedRoom();
        connected = true;
    }
}
