using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public class Launcher : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;
    public GameObject placeObjects;
    private Transition gameTransition;
    public bool connected = false;
    bool isConnecting;
    string gameVersion = "1";

    #region stuffNotNetworkRelated
    private GameObject crossHair;
    private GameObject camRotateWithZeroY;
    private GameObject m_cam;
    private GameObject cinemachineCam1;
    private GameObject cinemachineCam2;
    private GameObject loading;
    private GameObject rotation;
    #endregion
    

    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    [SerializeField]
    private byte maxPlayersPerRoom = 4;
    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    [SerializeField]
    private GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    [SerializeField]
    private GameObject progressLabel;

    void Awake() {
        PhotonNetwork.AutomaticallySyncScene = true;
        gameTransition = GameObject.Find("Main Camera").GetComponent<Transition>();
    }

    void Start()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        PhotonNetwork.GameVersion = gameVersion;

        //not related to networking
        crossHair = GameObject.Find("UI");
        camRotateWithZeroY = GameObject.Find("CamRotateWithZeroY");
        m_cam = GameObject.Find("Main Camera");
        cinemachineCam1 = GameObject.Find("CinemachineCam1");
        cinemachineCam2 = GameObject.Find("CinemachineCam2");
        loading = GameObject.Find("Loading");
        rotation = GameObject.Find("Rotation");
            DontDestroyOnLoad(crossHair);
            DontDestroyOnLoad(camRotateWithZeroY);
            DontDestroyOnLoad(m_cam);
            DontDestroyOnLoad(rotation);
            DontDestroyOnLoad(cinemachineCam1);
            DontDestroyOnLoad(cinemachineCam2);
            //DontDestroyOnLoad(loading);
        //end
    }
    public void Connect() {
        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        //not related to networking
        //end

        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.JoinRandomRoom();
        } 
        else 
        {
            isConnecting = PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }

    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN Basics Tutorial/Launcher: OnConnectedToMaster() was called by PUN");
        if (isConnecting){
            PhotonNetwork.JoinRandomRoom();
            isConnecting = false;
        }
        
    } 
    public override void OnDisconnected(DisconnectCause cause) 
    {
        Debug.LogWarningFormat("PUN Basics Tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}", cause);
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
        isConnecting = false;
    }
    public override void OnJoinedRoom()
    {
        //testing
        progressLabel.SetActive(false);
        controlPanel.SetActive(false);

        Debug.Log("PUN Basics Tutorial/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1' ");
                // #Critical
                // Load the Room Level.
                PhotonNetwork.LoadLevel("Room for 1");
            }
            //PhotonNetwork.InstantiateSceneObject(placeObjects.name, new Vector3(0.6f, 40.65f, 5.44f), Quaternion.identity);
            //connected = true;
    }
 
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("PUN Basics Tutorial/Launcher:OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });

    }

}
