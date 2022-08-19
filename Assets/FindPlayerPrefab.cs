using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
public class FindPlayerPrefab : MonoBehaviourPunCallbacks
{	
    private CinemachineFreeLook freeLookCam;
    private Launcher launcherScript;
    // Start is called before the first frame update
    void Start()
    {

        launcherScript = GameObject.Find("Launcher").GetComponent<Launcher>();
        freeLookCam = gameObject.GetComponent<CinemachineFreeLook>();
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
                    freeLookCam.GetComponent<CinemachineFreeLook>().m_Follow = player.transform;
                    freeLookCam.GetComponent<CinemachineFreeLook>().m_LookAt = player.transform;
                    break;
                }
            }

        }
    }
}
