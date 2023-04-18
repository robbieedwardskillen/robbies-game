using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
public class FindPlayerPrefab : MonoBehaviourPunCallbacks
{	
    //***DO NOT DO SINGLETONS FOR THIS AS IT IS USED IN MULTIPLE OCCASIONS***

    private CinemachineFreeLook freeLookCam;
    private CinemachineVirtualCamera virtualLookCam;
    private GameObject[] players;
    public bool playerFound = false;

    void Awake() {
		//DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        if (gameObject.GetComponent<CinemachineFreeLook>() != null)
        freeLookCam = gameObject.GetComponent<CinemachineFreeLook>();
        if (gameObject.GetComponent<CinemachineVirtualCamera>() != null)
        virtualLookCam = gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name != "Launcher" && SceneManager.GetActiveScene().name != "LauncherReturned" 
        && GameManager.Instance != null && PhotonNetwork.IsConnected) {
            if (!playerFound){
                players = GameObject.FindGameObjectsWithTag("Player");
                if (players != null){        
                    for (int i = 0; i < players.Length; i++){
                        //if (players.Length - 1 == i) {
                            if (PhotonView.Get(players[i]).IsMine){
                                setLookAt(players[i]); 
                                playerFound = true;
                            } else {
                                playerFound = false;
                            }
                        //}
/*                         else {
                            if (PhotonView.Get(players[i]).IsMine){
                                setLookAt(players[i]);  
                                playerFound = true;
                            }
                        } */
                    }
                }
            }
        }
    }

    void setLookAt(GameObject player) {
        if (freeLookCam != null){
            freeLookCam.m_Follow = player.transform;
            freeLookCam.m_LookAt = player.transform;

            //Absolutely no idea what this is or why I would ever put it here
            //freeLookCam.GetRig(2).LookAt = player.transform;
        }
        if (virtualLookCam != null){
            virtualLookCam.Follow = player.transform;
            virtualLookCam.LookAt = player.transform;
        }
    }
}
