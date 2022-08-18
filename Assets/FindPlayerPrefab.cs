using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class FindPlayerPrefab : MonoBehaviour
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
            freeLookCam.m_Follow = GameObject.Find("Player(Clone)").transform;
            freeLookCam.m_LookAt = GameObject.Find("Player(Clone)").transform;
        }
            
    }
}
