using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    public PhotonView playerPrefab;
    public PhotonView playerFreeCam;
    public bool connected = false;
    //private Transition gameTransition; set to false when loading room

/*     void Awake() {
        gameTransition = m_cam.GetComponent<Transition>();
    } */
    // Start is called before the first frame update
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
