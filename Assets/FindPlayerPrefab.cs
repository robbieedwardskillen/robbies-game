using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
public class FindPlayerPrefab : MonoBehaviourPunCallbacks
{	
    private CinemachineFreeLook freeLookCam;
    // Start is called before the first frame update
    void Start()
    {
        freeLookCam = gameObject.GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null){
            if (GameManager.Instance.connected){
                
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
}
