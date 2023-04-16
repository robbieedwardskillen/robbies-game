using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;


using Photon.Pun;
using Photon.Realtime;


public class GameManager : MonoBehaviourPunCallbacks
{
    #region Public Fields
    public static GameManager Instance;
    public GameObject testPrefab;
    public GameObject knightPrefab;
    //
    public GameObject placeObjects;
    public bool connected = false;
    private FindPlayerPrefab findPlayerPrefab1;//bad coding but i don't care
    private FindPlayerPrefab findPlayerPrefab2;

    #endregion

    #region Photon Callbacks

    void Start() {
        Instance = this;
        findPlayerPrefab1 = GameObject.Find("CinemachineCam1").GetComponent<FindPlayerPrefab>();
        findPlayerPrefab2 = GameObject.Find("CinemachineCam2").GetComponent<FindPlayerPrefab>();

        if (PlayerManager.LocalPlayerInstance == null){

            if (1 == 1){
                //PhotonNetwork.Instantiate(this.knightPrefab.name, new Vector3(5f, 41f, 5f), Quaternion.identity);
                PhotonNetwork.Instantiate(this.knightPrefab.name, new Vector3(22f, - 3.8f, 7f), Quaternion.identity);
            }


            //PhotonNetwork.InstantiateRoomObject(this.placeObjects.name, new Vector3(5f, 40.65f, 5f), Quaternion.identity);
            //UnityEngine.Random.Range(0,7f)
            connected = true;
        }
        else 
        {
            Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
        }
           
        
    }

    /// <summary>
    /// Called when the local player left the room. We need to load the launcher scene.
    /// </summary>
    public override void OnLeftRoom()
    {
        findPlayerPrefab1.playerFound = false;
        findPlayerPrefab2.playerFound = false;
        SceneManager.LoadScene(5);
    }


    #endregion


    #region Public Methods
    

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }


    #endregion
    #region Private Methods


    void LoadArena()
    {
        print("LoadArena" + PhotonNetwork.CurrentRoom.PlayerCount);
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
        //PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount + " " + 2);
        
    }


    #endregion

    #region Photon Callbacks


    public override void OnPlayerEnteredRoom(Player other)
    {
        Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }


    public override void OnPlayerLeftRoom(Player other)
    {

        Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


            LoadArena();
        }
    }


    #endregion
        
}
