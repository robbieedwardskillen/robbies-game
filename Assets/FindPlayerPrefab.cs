using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
public class FindPlayerPrefab : MonoBehaviourPunCallbacks
{	
    private CinemachineFreeLook freeLookCam;
    private CinemachineVirtualCamera virtualLookCam;
    private GameObject[] players;
    private bool playerFound = false;
    
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
        if (GameManager.Instance != null && !playerFound){//playerFound issue
            if (PhotonNetwork.IsConnected){
                checkForPlayer();
            }
        }

    }

    void checkForPlayer() {
        players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            if (PhotonView.Get(player).IsMine && !playerFound) 
            {
                if (freeLookCam != null){
                    freeLookCam.m_Follow = player.transform;
                    freeLookCam.m_LookAt = player.transform;
                }
                if (virtualLookCam != null){
                    virtualLookCam.Follow = player.transform;
                    virtualLookCam.LookAt = player.transform;
                }
                playerFound = true;
            }
        }
    }
}
