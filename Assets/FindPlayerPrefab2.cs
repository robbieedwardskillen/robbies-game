using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class FindPlayerPrefab2 : MonoBehaviour
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
            virtualLookCam.Follow = GameObject.Find("Player(Clone)").transform;
            virtualLookCam.LookAt = GameObject.Find("Player(Clone)").transform;
        }

    }
}
