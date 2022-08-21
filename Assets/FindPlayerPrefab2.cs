using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
public class FindPlayerPrefab2 : MonoBehaviourPunCallbacks
{
	private CinemachineVirtualCamera virtualLookCam;
    private Launcher launcherScript;
    // Start is called before the first frame update
    void Start()
    {
        launcherScript = GameObject.Find("Launcher").GetComponent<Launcher>();
        virtualLookCam = gameObject.GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {

        if (launcherScript.connected){
            
            //print(photonView.)
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players)
            {
                if (PhotonView.Get(player).IsMine)
                {
                    virtualLookCam.GetComponent<CinemachineVirtualCamera>().m_Follow = player.transform;
                    virtualLookCam.GetComponent<CinemachineVirtualCamera>().m_LookAt = player.transform;
                    break;
                }
            }

        }


    }
}
